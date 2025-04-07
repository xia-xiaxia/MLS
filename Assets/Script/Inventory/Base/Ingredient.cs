using System;
using System.Collections.Generic;
using UnityEngine;


//升级成长曲线可选公式
public enum UpgradeFormula
{
    Linear,      // 线性：cost = param1 * level
    Exponential, // 指数：cost = param1 * (param2^level)
    Polynomial,  // 多项式：cost = param1 * level^param2
    CustomCurve  // 自定义曲线（需配合AnimationCurve）
}

// 食材配置数据
[CreateAssetMenu(fileName = "New Ingredient", menuName = "Inventory/Ingredient")]

public class IngredientConfig : ScriptableObject
{
    [Header("Ingredient Info")]
    public string IngredientName;//食材名字
    public Sprite IngredientImage;//图像
    [TextArea]
    public string IngredientDescription;//描述
    public int IngredientID;//食材ID
    public int IngredientLevel;//食材等级
    public float IngredientPrice;//食材采购价格
    public Rarity baseRarity = Rarity.white;//稀有度

    [Header("升稀有的配置")]
    public int[] rarityUpgradeCosts = { 10, 50, 200, 500, 1500 };
    public int maxLevel = 10;
    public int levelUpgradeBaseCost = 100;

    [Header("评分系数")]
    public int scorePerLevel = 15;
    public int scorePerRarity = 20;

    [Header("等级升级")]
    public UpgradeFormula levelFormula = UpgradeFormula.Linear;
    public int levelBaseCost = 100;
    public float levelGrowthFactor = 1.2f;

    [Header("稀有度系数")]
    public int rarityProfitMultiplier = 30; // 每级稀有度的收益加成
    public int rarityCostMultiplier = 15;   // 每级稀有度的采购费加成

    [Header("等级系数")]
    public int baseProfit = 15;      // 每等级的收益增加值
    public int basePurchaseCost = 30;// 基础采购费

    [Header("可用配方")]
    public List<Recipe> compatibleRecipes;
}

// 食材核心逻辑
public class Ingredient
{
    public string ID { get; }
    public string IngredientName { get; }
    public Rarity Rarity;//由CurrentRarity改为Rarity
    public int IngredientLevel { get; private set; }//由CurrentLevel改为IngredientLevel
    public int TotalScore { get; private set; }
    public Sprite IngredientImage { get; }
    [TextArea]
    public string IngredientDescription;

    public readonly IngredientConfig _config;

    public Ingredient(string id, string displayname, Sprite ingredientImage, IngredientConfig config)
    {
        ID = id;
        IngredientName = displayname;
        IngredientImage = ingredientImage;
        _config = config;
        ResetState();
    }

    public void ResetState()
    {
        Rarity = _config.baseRarity;
        IngredientLevel = 1;
        CalculateIngredientScore();
    }
    public void UpgradeRarity(Rarity newRarity)
    {
        if (newRarity <= Rarity) return;

        Rarity = newRarity;
        CalculateIngredientScore();
        IngredientEvents.RaiseRarityUpgraded(this);
    }

    public void UpgradeLevel()
    {
        if (IngredientLevel >= _config.maxLevel) return;

        IngredientLevel++;
        CalculateIngredientScore();
        IngredientEvents.RaiseLevelUpgraded(this);
    }

    public void CalculateIngredientScore()
    {
        int rarityScore = Mathf.Min(
            (int)Rarity * _config.scorePerRarity,
            100 // 稀有度上限100分
        );

        int levelScore = Mathf.Min(
            IngredientLevel * _config.scorePerLevel,
            150 // 等级上限150分
        );

        TotalScore = rarityScore + levelScore;
    }
}

// 成本计算服务
public static class UpgradeCostCalculator
{
    public static int CalculateCost(UpgradeFormula type, int currentLevel, float param1, float param2)
    {
        return type switch
        {
            UpgradeFormula.Linear =>
                Mathf.RoundToInt(param1 * currentLevel),

            UpgradeFormula.Exponential =>
                Mathf.RoundToInt(param1 * Mathf.Pow(param2, currentLevel)),

            UpgradeFormula.Polynomial =>
                Mathf.RoundToInt(param1 * Mathf.Pow(currentLevel, param2)),

            _ => Mathf.RoundToInt(param1 * currentLevel) // 默认线性
        };
    }

    // 自定义曲线版本（需在config中添加AnimationCurve字段）
    public static int CalculateCostByCurve(AnimationCurve curve, int currentLevel)
    {
        return Mathf.RoundToInt(curve.Evaluate(currentLevel));
    }
}

// 食材升级服务
public class AutoIngredientUpgrader
{
    private readonly IngredientUpgradeService _upgradeService;
    private readonly PlayerInventory _inventory;
    private readonly int _goldBudgetPerTick; // 每帧/每周期分配多少金币来自动升级

    public AutoIngredientUpgrader(IngredientUpgradeService upgradeService, PlayerInventory inventory, int goldBudgetPerTick)
    {
        _upgradeService = upgradeService;
        _inventory = inventory;
        _goldBudgetPerTick = goldBudgetPerTick;
    }

    public void AutoUpgradeAllIngredients(List<Ingredient> ingredients, Dictionary<string, IngredientConfig> configs, ref int gold)
    {
        foreach (var ingredient in ingredients)
        {
            var config = configs[ingredient.ID];

            // 自动升稀有度：一次升到最高
            while (_upgradeService.TryUpgradeRarity(ingredient, _inventory, config)) { }

            // 自动升等级：循环升级直到金币不足或到上限
            while (true)
            {
                int cost = UpgradeCostCalculator.CalculateCost(
                    config.levelFormula,
                    ingredient.IngredientLevel,
                    config.levelBaseCost,
                    config.levelGrowthFactor
                );

                if (ingredient.IngredientLevel >= config.maxLevel || gold < cost) break;

                gold -= cost;
                ingredient.UpgradeLevel();
            }
        }
    }
}

public class IngredientUpgradeService
{
    public bool TryUpgradeRarity(
        Ingredient ingredient,
        PlayerInventory inventory,
        IngredientConfig config)
    {
        // 获取下一稀有度
        var nextRarity = GetNextRarity(ingredient.Rarity);

        // 验证条件
        if (nextRarity == ingredient.Rarity) return false;
        if (!GetRarityUpgradeCost(ingredient.Rarity, config, out var cost)) return false;
        if (!inventory.ingredients.TryGetValue(ingredient.ID, out var data)) return false;
        if (data.fragments < cost) return false;

        // 执行升级
        data.fragments -= cost;
        ingredient.UpgradeRarity(nextRarity); // 调用新增方法
        return true;
    }
    public bool TryUpgradeLevel(
       Ingredient ingredient,
       PlayerInventory inventory,
       IngredientConfig config,
       ref int gold)
    {
        if (ingredient.IngredientLevel >= config.maxLevel)
            return false;

        int cost = config.levelUpgradeBaseCost * ingredient.IngredientLevel;
        if (gold < cost)
            return false;

        gold -= cost;
        ingredient.UpgradeLevel(); // 调用已有方法
        return true;
    }
    // 获取下一稀有度的方法
    private Rarity GetNextRarity(Rarity current) => current switch
    {
        Rarity.white => Rarity.Green,
        Rarity.Green => Rarity.Blue,
        Rarity.Blue => Rarity.Purple,
        Rarity.Purple => Rarity.Gold,
        Rarity.Gold => Rarity.Rainbow,
        Rarity.Rainbow => Rarity.Rainbow,
        _ => current
    };
    private bool GetRarityUpgradeCost(Rarity current, IngredientConfig config, out int cost)
    {
        int index = current switch
        {
            Rarity.white => 0,
            Rarity.Green => 1,
            Rarity.Blue => 2,
            Rarity.Purple => 3,
            Rarity.Gold => 4,
            _ => -1
        };

        if (index >= 0 && index < config.rarityUpgradeCosts.Length)
        {
            cost = config.rarityUpgradeCosts[index];
            return true;
        }

        cost = int.MaxValue;
        return false;
    }

    private bool IsMaxRarity(Rarity rarity) => rarity >= Rarity.Rainbow;
}

// 事件系统
public static class IngredientEvents
{
    public static event Action<Ingredient> OnLevelUpgraded;
    public static event Action<Ingredient> OnRarityUpgraded;

    public static void RaiseLevelUpgraded(Ingredient ingredient) =>
        OnLevelUpgraded?.Invoke(ingredient);

    public static void RaiseRarityUpgraded(Ingredient ingredient) =>
        OnRarityUpgraded?.Invoke(ingredient);
}
using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeFormula
{
    Linear,      // 线性：cost = param1 * level
    Exponential, // 指数：cost = param1 * (param2^level)
    Polynomial,  // 多项式：cost = param1 * level^param2
    CustomCurve  // 自定义曲线（需配合AnimationCurve）
}

// 食材配置数据
[CreateAssetMenu(menuName = "Data/IngredientConfig")]
public class IngredientConfig : ScriptableObject
{
    public int id;
    public string displayName;
    public Rarity baseRarity = Rarity.white;

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
    public float rarityProfitMultiplier = 30f; // 每级稀有度的收益加成
    public float rarityCostMultiplier = 15f;   // 每级稀有度的采购费加成

    [Header("等级系数")]
    public int baseProfit = 15;      // 每等级的收益增加值
    public int basePurchaseCost = 30;// 基础采购费

    [Header("可用配方")]
    public List<RecipeConfig> compatibleRecipes;
}

// 食材核心逻辑
public class Ingredient
{
    public string ID { get; }
    public Rarity CurrentRarity { get; private set; }
    public int CurrentLevel { get; private set; }
    public int TotalScore { get; private set; }

    private readonly IngredientConfig _config;

    public Ingredient(string id, IngredientConfig config)
    {
        ID = id;
        _config = config;
        ResetState();
    }

    public void ResetState()
    {
        CurrentRarity = _config.baseRarity;
        CurrentLevel = 1;
        CalculateScore();
    }
    public void UpgradeRarity(Rarity newRarity)
    {
        if (newRarity <= CurrentRarity) return;

        CurrentRarity = newRarity;
        CalculateScore();
        IngredientEvents.RaiseRarityUpgraded(this);
    }

    public void UpgradeLevel()
    {
        if (CurrentLevel >= _config.maxLevel) return;

        CurrentLevel++;
        CalculateScore();
        IngredientEvents.RaiseLevelUpgraded(this);
    }

    private void CalculateScore()
    {
        TotalScore = CurrentLevel * _config.scorePerLevel +
                    ((int)CurrentRarity * _config.scorePerRarity);
    }
}

// 成本计算
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

// 食材升级
public class IngredientUpgradeService
{
    public bool TryUpgradeRarity(
        Ingredient ingredient,
        PlayerInventory inventory,
        IngredientConfig config)
    {
        // 获取下一稀有度
        var nextRarity = GetNextRarity(ingredient.CurrentRarity);

        // 验证条件
        if (nextRarity == ingredient.CurrentRarity) return false;
        if (!GetRarityUpgradeCost(ingredient.CurrentRarity, config, out var cost)) return false;
        if (!inventory.ingredients.TryGetValue(ingredient.ID, out var data)) return false;
        if (data.fragments < cost) return false;

        // 执行升级
        data.fragments -= cost;
        ingredient.UpgradeRarity(nextRarity); 
        return true;
    }
    public bool TryUpgradeLevel(
       Ingredient ingredient,
       PlayerInventory inventory,
       IngredientConfig config,
       ref int gold)
    {
        if (ingredient.CurrentLevel >= config.maxLevel) 
            return false;

        int cost = config.levelUpgradeBaseCost * ingredient.CurrentLevel;
        if (gold < cost) 
            return false;

        gold -= cost;
        ingredient.UpgradeLevel(); 
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

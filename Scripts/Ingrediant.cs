using System;
using System.Collections.Generic;
using UnityEngine;

// 食材配置数据
[CreateAssetMenu(menuName = "Data/IngredientConfig")]
public class IngredientConfig : ScriptableObject
{
    public string displayName;
    public Rarity baseRarity = Rarity.white;
    [Header("升级配置")]
    public int[] rarityUpgradeCosts = { 10, 50, 200, 500, 1500 };
    public int maxLevel = 10;
    public int levelUpgradeBaseCost = 100;
    [Header("评分系数")]
    public int scorePerLevel = 15;
    public int scorePerRarity = 20;
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
        IngredientEvents.RaiseUpgraded(this);
    }

    public void UpgradeLevel()
    {
        if (CurrentLevel >= _config.maxLevel) return;

        CurrentLevel++;
        CalculateScore();
        IngredientEvents.RaiseUpgraded(this);
    }

    private void CalculateScore()
    {
        TotalScore = CurrentLevel * _config.scorePerLevel +
                    ((int)CurrentRarity * _config.scorePerRarity);
    }
}

// 食材升级服务
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
        ingredient.UpgradeRarity(nextRarity); // 调用新增方法
        return true;
    }
    public bool TryUpgradeLevel(
       Ingredient ingredient,
       PlayerInventory inventory,
       IngredientConfig config,
       ref int gold)
    {
        if (ingredient.CurrentLevel >= config.maxLevel) return false;

        int cost = config.levelUpgradeBaseCost * ingredient.CurrentLevel;
        if (gold < cost) return false;

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
    public static event Action<Ingredient> OnUpgraded;

    public static void RaiseUpgraded(Ingredient ingredient) =>
        OnUpgraded?.Invoke(ingredient);
}

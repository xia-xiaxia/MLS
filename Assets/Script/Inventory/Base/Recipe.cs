using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe")]
public class Recipe : ScriptableObject
{
    [Header("Recipe Info")]
    public string RecipeName;
    public Sprite RecipeImage;
    [TextArea]
    public string RecipeDescription;
    public int RecipeID;
    public int RecipeLevel;
    public float RecipePrice;
    public float RecipeCost;
    public Rarity Rarity = Rarity.white;//稀有度

    public List<IngredientRequirement> ingredients = new List<IngredientRequirement>();//requirements改为ingredients

    [Header("稀有度收益")]
    public int rarityBaseProfit = 50;    // 配方稀有度基础收益

    [Header("等级收益")]
    public int levelProfitIncrement = 40;// 每级增加的收益

    [Header("基础属性")]
    public int baseProfit = 200;        // 基础收益

    [System.Serializable]
    public class IngredientRequirement
    {
        public IngredientConfig ingredient;
        public int amount;
        public Ingredient ingredients;
        public string IngredientName;
    }
    [Header("升级配置")]
    public int maxLevel = 5;
    public int scorePerLevel = 20;
    public int scorePerRarity = 30;
    public int[] expRequirements = { 1, 10, 40, 100, 200 };

    [Header("稀有度系数")]
    public float rarityMultiplier = 1.5f;
    public int baseUnlockCost = 100;

    public int GetRequiredExp(int currentLevel)
    {
        return currentLevel < expRequirements.Length ?
            expRequirements[currentLevel] : int.MaxValue;
    }
}
// 成长数据
[System.Serializable]
public class RecipeProgressData
{
    public int recipeId;
    public Rarity rarity;
    public int level;
    public int exp;
}


// Recipe数值系统
public class RecipeSystem
{
    #region 核心数据
    private readonly Recipe _baseRecipe; // 读配置
    private readonly Recipe _config;

    // 独立运行
    public Rarity CurrentRarity { get; private set; }
    public int CurrentLevel { get; private set; }
    public int TotalScore { get; private set; }

    public bool IsUnlocked => CurrentRarity > Rarity.white;
    #endregion

    #region 初始化
    public RecipeSystem(Recipe baseRecipe, Recipe config)
    {
        _baseRecipe = baseRecipe;
        _config = config;

        // 初始状态
        CurrentRarity = _baseRecipe.Rarity;
        CurrentLevel = 1;
        CalculateScore();
    }
    #endregion

    public EconomySystem economySystem;
    #region 核心逻辑
    //配方升级
    public bool TryUpgrade(int availableExp)
    {
       
        if (!CanUpgrade(availableExp)) return false;
        int cost=GetRequiredExp();
        if (!economySystem.SpendGold(cost, FinanceType.RecipeUpgrade, $"升级菜谱{DisplayName}"))
            return false;
        availableExp -= GetRequiredExp();
        CurrentLevel = Mathf.Min(CurrentLevel + 1, _config.maxLevel);
        CalculateScore();

        RecipeEvents.RaiseUpgraded(this);
        return true;
    }

    public bool TryUnlock(int availableGold)
    {
        if (IsUnlocked || availableGold < _config.baseUnlockCost)
            return false;

        CurrentRarity = Rarity.Green;
        CalculateScore();

        RecipeEvents.RaiseUnlocked(this);
        return true;
    }

    public bool CanUpgrade(int availableExp)
    {
        return CurrentLevel < _config.maxLevel && availableExp >= GetRequiredExp();
    }
    #endregion

    // 数值计算
    //计算配方贡献分数
    private void CalculateScore()
    {
        float rarityBonus = Mathf.Pow((int)CurrentRarity, _config.rarityMultiplier);
        TotalScore = Mathf.RoundToInt(
            (CurrentLevel * _config.scorePerLevel) +
            (rarityBonus * _config.scorePerRarity)
        );
    }

    private int GetRequiredExp()
    {
        return _config.GetRequiredExp(CurrentLevel);
    }

    //UI桥接
    public string DisplayName => _baseRecipe.RecipeName;
    public Sprite DisplayIcon => _baseRecipe.RecipeImage;
    public string StatusText => $"Lv.{CurrentLevel} [{CurrentRarity}] Score: {TotalScore}";
}

// 事件通信系统
public static class RecipeEvents
{
    public static event System.Action<RecipeSystem> OnUpgraded;
    public static event System.Action<RecipeSystem> OnUnlocked;//解锁新菜谱

    public static void RaiseUpgraded(RecipeSystem recipe) => OnUpgraded?.Invoke(recipe);
    public static void RaiseUnlocked(RecipeSystem recipe) => OnUnlocked?.Invoke(recipe);
}
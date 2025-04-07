using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

// 菜肴配置
[CreateAssetMenu(menuName = "Data/Dish")]
public class DishConfig : ScriptableObject
{
    public string dishName;
    public Recipe baseRecipe;

    [Header("采购费范围")]
    public int minPurchaseCost;
    public int maxPurchaseCost;

    [Header("收益范围")]
    public int minProfit;
    public int maxProfit;
}

// 菜肴系统
public class Dish
{
    #region 基础属性
    public string Name { get; private set; }
    public Rarity Rarity { get; private set; }
    public int BasePrice { get; private set; }
    public int TotalProfit { get; private set; }

    public int TotalScore { get; private set; }  // 总分用于确定等级
    public int Level { get; private set; }       // 根据总分自动计算
    #endregion

    #region 配置参数
    private const int MaxLevel = 5;         // 最大等级
    private const int ScorePerLevel = 100;  // 每级需要100分
    #endregion

    #region 初始化
    public Dish(
        string name,
        Rarity rarity,
        int basePrice,
        RecipeSystem recipeSystem, // 传入配方系统
        List<Ingredient> ingredients // 传入食材列表
    )
    {
        Name = name;
        Rarity = rarity;
        BasePrice = basePrice;
        TotalProfit = TotalProfit;
        Level = 1; // 初始等级
        TotalScore = 0;  // 初始评分
    }
    
    #endregion

    #region 核心逻辑
    // 计算总分并更新等级
    public void UpdateScoreAndLevel(RecipeSystem recipeSystem, List<Ingredient> ingredients)
    {
        // 计算配方分
        int recipeScore = recipeSystem.TotalScore;

        // 计算食材平均分
        int totalIngredientScore = 0;
        foreach (var ingredient in ingredients)
        {
            totalIngredientScore += ingredient.TotalScore;
        }
        int ingredientScore = ingredients.Count > 0
            ? totalIngredientScore / ingredients.Count
            : 0;

        // 更新总分（不超过500）
        TotalScore = Mathf.Min(recipeScore + ingredientScore, 500);

        // 根据总分自动确定等级
        int newLevel = Mathf.Min(Mathf.FloorToInt(TotalScore / (float)ScorePerLevel) + 1, MaxLevel);
        if (newLevel != Level)
        {
            Level = newLevel;
            Debug.Log($"菜肴 {Name} 等级更新至 Lv.{Level}");
        }
    }
    #endregion

    #region 工具方法
    // 获取当前总收益
    public int CalculateProfit(RecipeSystem recipeSystem, List<Ingredient> ingredients)
    {
        // 配方基础收益
        int recipeProfit = recipeSystem.TotalScore / 10;

        // 食材总成本
        int ingredientCost = 0;
        foreach (var ingredient in ingredients)
        {
            ingredientCost += ingredient._config.basePurchaseCost *
                             (int)ingredient.Rarity *
                             ingredient.IngredientLevel;
        }

        // 最终收益 = 配方收益 - 食材成本
        return Mathf.Max(recipeProfit - ingredientCost, 0);
    }
    public int GetTotalProfit() => TotalProfit;

    public bool IsMaxLevel() => Level >= MaxLevel;
    #endregion
}
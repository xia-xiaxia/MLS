using Unity.VisualScripting;
using UnityEngine;
using System;

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
    public int Profit { get; private set; }
    public int Mark { get; private set; }
    public int Level { get; private set; }
    #endregion

    #region 配置参数
    private const int ProfitPerLevel = 15; // 每级增加的利润
    private const int MaxLevel = 10;       // 最大等级
    private const int MarkThreshold = 100; // 每100分升一级
    #endregion

    #region 初始化
    public Dish(string name, Rarity rarity, int basePrice, int profit)
    {
        Name = name;
        Rarity = rarity;
        BasePrice = basePrice;
        Profit = profit;
        Level = 1; // 初始等级
        Mark = 0;  // 初始评分
    }
    #endregion

    #region 核心逻辑
    // 升级菜肴
    public void Upgrade()
    {
        if (Level >= MaxLevel)
        {
            Debug.LogWarning($"菜肴 {Name} 已达到最大等级 {MaxLevel}，无法继续升级。");
            return;
        }

        Level++;
        Profit += ProfitPerLevel;
        Debug.Log($"菜肴 {Name} 升级至 Lv.{Level}，当前利润：{Profit}");
    }

    // 根据总评分自动升级
    public void AutoUpgradeBasedOnMark(int allMark)
    {
        if (allMark < MarkThreshold)
        {
            Debug.Log($"总评分 {allMark} 不足 {MarkThreshold}，无法升级。");
            return;
        }

        int levelsToUpgrade = Math.Min(allMark / MarkThreshold, MaxLevel - Level);
        for (int i = 0; i < levelsToUpgrade; i++)
        {
            Upgrade();
        }

        Debug.Log($"菜肴 {Name} 根据评分 {allMark} 自动升级 {levelsToUpgrade} 级，当前等级：{Level}");
    }
    #endregion

    #region 状态更新
    // 更新评分
    public void AddMark(int mark)
    {
        if (mark < 0)
        {
            throw new ArgumentException("评分不能为负数。");
        }

        Mark += mark;
        Debug.Log($"菜肴 {Name} 增加评分 {mark}，当前总评分：{Mark}");
    }
    #endregion

    #region 工具方法
    // 获取当前菜肴的总价值
    public int GetTotalValue()
    {
        return BasePrice + Profit;
    }

    // 检查是否达到最大等级
    public bool IsMaxLevel()
    {
        return Level >= MaxLevel;
    }
    #endregion
}
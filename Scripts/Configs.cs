using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;

public class CustomerSystem
{
    public int seats = 5;
    public int diningTime = 300;
    public int cleaningTime = 60;

    public int CalculateDailyCustomers()
    {
        return seats * 50400 / (diningTime + cleaningTime);//计算顾客数
    }
}
// 存储每日统计数据
public struct DailyStats
{
    public int day;
    public int customers;
    public int revenue;
    public int expenses;
    public int profit;
    public int totalGold; // 结算后的金币余额
}
//基建
public class Building
{
    public int seatCount = 5;
    public int maxChefs = 1;
    public int gold = 10000; // 玩家金币

    //升级————座位扩张
    public void UpgradeSeats()
    {
        if (gold >= 2000)
        {
            seatCount += 1;
            gold -= 2000;
            Debug.Log($"座位增加至 {seatCount} 桌，剩余金币: {gold}");
        }
        else
        {
            Debug.Log("金币不足，无法升级座位！");
        }
    }

    //升级————厨房扩容
    public void UpgradeKitchen()
    {
        if (gold >= 5000)
        {
            maxChefs += 1;
            gold -= 5000;
            Debug.Log($"厨房扩容，最多可雇佣 {maxChefs} 名厨师，剩余金币: {gold}");
        }
        else
        {
            Debug.Log("金币不足，无法扩容厨房！");
        }
    }
}

// 物品稀有度
public enum Rarity
{
    Green,  // 绿
    Blue,   // 蓝
    Purple, // 紫
    Gold,   // 金
    Rainbow // 彩
}

// 物品类别
public enum ItemType
{
    Recipe, // 配方
    Ingredient, // 食材
    Partner, // 伙伴
    //PartnerFragments//伙伴碎片
}

// 物品数据
public class ShoppingItem
{
    public readonly string itemID;    // 物品唯一标识
    public readonly ItemType itemType;
    public readonly Rarity rarity;
    public readonly int conversionValue;

    public ShoppingItem(ItemType type, Rarity rarity, int value, string id)
    {
        itemType = type;
        this.rarity = rarity;
        conversionValue = value;
        itemID = id;
    }
}


// 菜肴系统
public class Dish
{
    public string Name;
    public Rarity Rarity;
    public int BasePrice;
    public int Profit;

    public Dish(string name, Rarity rarity, int basePrice, int profit)
    {
        Name = name;
        Rarity = rarity;
        BasePrice = basePrice;
        Profit = profit;
    }
}

// 配方系统
public class Recipe
{
    public string Name;
    public Rarity Rarity;//稀有度
    public int Level;
    public int BaseProfit;
    public List<Ingredient> RequiredIngredients;

    public Recipe(string name, Rarity rarity, int baseProfit)
    {
        Name = name;
        Rarity = rarity;
        Level = 1;
        BaseProfit = baseProfit;
        RequiredIngredients = new List<Ingredient>();
    }

    //配方升级
    public void Upgrade()
    {
        Level++;
        BaseProfit += 5; // 每次升级提升基础收益
    }
}

// 食材系统
public class Ingredient
{
    public string Name;
    public Rarity Rarity;
    public int Level;
    public int PurchaseCost;
    public int SellPrice;

    public Ingredient(string name, Rarity rarity, int purchaseCost, int sellPrice)
    {
        Name = name;
        Rarity = rarity;
        Level = 1;
        PurchaseCost = purchaseCost;
        SellPrice = sellPrice;
    }

    //食材升级
    public void Upgrade()
    {
        Level++;
        SellPrice += 2;//食材升级有什么效果？
    }
}

//食材升星
public class IngredientSystem
{
    public bool TryUpgradeRarity(string ingredientID, PlayerInventory inventory)
    {
        if (!inventory.ingredients.TryGetValue(ingredientID, out var data))
            return false;

        // 获取下一稀有度
        Rarity nextRarity = GetNextRarity(data.highestRarity);
        if (nextRarity == data.highestRarity) return false;

        // 检查碎片是否足够
        int requiredFragments = GetUpgradeCost(data.highestRarity);
        if (data.fragments < requiredFragments) return false;

        // 执行升星
        data.fragments -= requiredFragments;
        data.highestRarity = nextRarity;
        return true;
    }

    private Rarity GetNextRarity(Rarity current)
    {
        return current switch
        {
            Rarity.Green => Rarity.Blue,
            Rarity.Blue => Rarity.Purple,
            Rarity.Purple => Rarity.Gold,
            Rarity.Gold => Rarity.Rainbow,
            _ => current
        };
    }

    private int GetUpgradeCost(Rarity current)
    {
        return current switch
        {
            Rarity.Green => 15,
            Rarity.Blue => 30,
            Rarity.Purple => 60,
            Rarity.Gold => 120,
            _ => int.MaxValue
        };
    }
}

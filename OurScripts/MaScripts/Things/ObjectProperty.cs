using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;

// 物品稀有度
public enum Rarity
{
    white,   //白，实质上是没有获得
    Green,  // 绿
    Blue,   // 蓝
    Purple, // 紫
    Gold,   // 金
    Rainbow // 彩
}

// 物品类别
public enum CardType
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
    public readonly CardType itemType;
    public readonly Rarity rarity;
    public readonly int conversionValue;

    public ShoppingItem(CardType type, Rarity rarity, int value, string id)
    {
        itemType = type;
        this.rarity = rarity;
        conversionValue = value;
        itemID = id;
    }
}

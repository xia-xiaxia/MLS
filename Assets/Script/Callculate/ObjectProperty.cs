using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;

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

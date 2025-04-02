using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;

// ��Ʒ����
public class ShoppingItem
{
    public readonly string itemID;    // ��ƷΨһ��ʶ
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

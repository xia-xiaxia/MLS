using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;

// ��Ʒϡ�ж�
public enum Rarity
{
    white,   //�ף�ʵ������û�л��
    Green,  // ��
    Blue,   // ��
    Purple, // ��
    Gold,   // ��
    Rainbow // ��
}

// ��Ʒ���
public enum ItemType
{
    Recipe, // �䷽
    Ingredient, // ʳ��
    Partner, // ���
    //PartnerFragments//�����Ƭ
}

// ��Ʒ����
public class ShoppingItem
{
    public readonly string itemID;    // ��ƷΨһ��ʶ
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

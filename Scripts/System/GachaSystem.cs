using System.Collections.Generic;
using UnityEngine;

// �鿨ϵͳ
public class GachaSystem
{
    // ���ʱ��ܸ�����Ϊ100��
    private Dictionary<Rarity, float> quickBuyTable = new Dictionary<Rarity, float>
    {
        {Rarity.Green, 15f}, {Rarity.Blue, 4f}, {Rarity.Purple, 0.8f},
        {Rarity.Gold, 0.175f}, {Rarity.Rainbow, 0.025f}
    };

    private Dictionary<Rarity, float> premiumBuyTable = new Dictionary<Rarity, float>
    {
        {Rarity.Green, 10f}, {Rarity.Blue, 7f}, {Rarity.Purple, 2.3f},
        {Rarity.Gold, 0.575f}, {Rarity.Rainbow, 0.125f}
    };

    // �������ϡ�ж�
    private Rarity GetRandomRarity(Dictionary<Rarity, float> probabilityTable)
    {
        float total = 0;
        foreach (var pair in probabilityTable) total += pair.Value;

        float randomPoint = UnityEngine.Random.Range(0f, total);
        float current = 0;

        foreach (var pair in probabilityTable)
        {
            current += pair.Value;
            if (randomPoint <= current) return pair.Key;
        }

        return Rarity.Green; // ���׷������ϡ�ж�
    }

    // ����ʾ����Ʒ����
    private string GenerateItemName(ItemType type, Rarity rarity)
    {
        string prefix = type switch
        {
            ItemType.Recipe => "REC",
            ItemType.Ingredient => "ING",
            ItemType.Partner => "PTN",
            _ => "ITM"
        };

        return $"{prefix}_{rarity}_{UnityEngine.Random.Range(1000, 9999)}";
    }

    // ��Դת������
    private int GetConversionValue(ItemType type, Rarity rarity)
    {
        switch (type)
        {
            //�䷽
            case ItemType.Recipe:
                return rarity switch
                {
                    Rarity.Green => 1,
                    Rarity.Blue => 2,
                    Rarity.Purple => 3,
                    Rarity.Gold => 5,
                    Rarity.Rainbow => 10,
                    _ => 0
                };
            //ʳ��
            case ItemType.Ingredient:
                return rarity switch
                {
                    Rarity.Green => 1,
                    Rarity.Blue => 2,
                    Rarity.Purple => 4,
                    Rarity.Gold => 8,
                    Rarity.Rainbow => 15,
                    _ => 0
                };
            //���
            case ItemType.Partner:
                return (rarity >= Rarity.Gold) ? 10 : 1;

            default:
                return 0;
        }
    }

    // �鿨����
    public ShoppingItem DrawItem(bool isPremium)
    {
        var probTable = isPremium ? premiumBuyTable : quickBuyTable;//�жϳ鿨��ʽ�Ƿ�Ϊ�߼��鿨
        Rarity rarity = GetRandomRarity(probTable);
        ItemType type = GetRandomItemType(isPremium);
        string name = GenerateItemName(type, rarity);
        int value = GetConversionValue(type, rarity);

        return new ShoppingItem(type, rarity, value, name);
    }

    // �����Ʒ����
    private ItemType GetRandomItemType(bool isPremium)
    {
        ///���ж��ǲ��ǻ�飬����ϡ�жȾ�������Ƭ���ǽ�ɫ
        float rand = UnityEngine.Random.value;
        if (true)
            return rand switch

            {
                < 0.2f => ItemType.Recipe,   // 20%�����䷽
                < 0.68f => ItemType.Ingredient, // 73%����ʳ��
                _ => ItemType.Partner        // 7%���ʻ��
            };

    }

}

using Unity.VisualScripting;
using UnityEngine;
using System;

// ��������
[CreateAssetMenu(menuName = "Data/Dish")]
public class DishConfig : ScriptableObject
{
    public string dishName;
    public Recipe baseRecipe;

    [Header("�ɹ��ѷ�Χ")]
    public int minPurchaseCost;
    public int maxPurchaseCost;

    [Header("���淶Χ")]
    public int minProfit;
    public int maxProfit;
}

// ����ϵͳ
public class Dish
{
    #region ��������
    public string Name { get; private set; }
    public Rarity Rarity { get; private set; }
    public int BasePrice { get; private set; }
    public int Profit { get; private set; }
    public int Mark { get; private set; }
    public int Level { get; private set; }
    #endregion

    #region ���ò���
    private const int ProfitPerLevel = 15; // ÿ�����ӵ�����
    private const int MaxLevel = 10;       // ���ȼ�
    private const int MarkThreshold = 100; // ÿ100����һ��
    #endregion

    #region ��ʼ��
    public Dish(string name, Rarity rarity, int basePrice, int profit)
    {
        Name = name;
        Rarity = rarity;
        BasePrice = basePrice;
        Profit = profit;
        Level = 1; // ��ʼ�ȼ�
        Mark = 0;  // ��ʼ����
    }
    #endregion

    #region �����߼�
    // ��������
    public void Upgrade()
    {
        if (Level >= MaxLevel)
        {
            Debug.LogWarning($"���� {Name} �Ѵﵽ���ȼ� {MaxLevel}���޷�����������");
            return;
        }

        Level++;
        Profit += ProfitPerLevel;
        Debug.Log($"���� {Name} ������ Lv.{Level}����ǰ����{Profit}");
    }

    // �����������Զ�����
    public void AutoUpgradeBasedOnMark(int allMark)
    {
        if (allMark < MarkThreshold)
        {
            Debug.Log($"������ {allMark} ���� {MarkThreshold}���޷�������");
            return;
        }

        int levelsToUpgrade = Math.Min(allMark / MarkThreshold, MaxLevel - Level);
        for (int i = 0; i < levelsToUpgrade; i++)
        {
            Upgrade();
        }

        Debug.Log($"���� {Name} �������� {allMark} �Զ����� {levelsToUpgrade} ������ǰ�ȼ���{Level}");
    }
    #endregion

    #region ״̬����
    // ��������
    public void AddMark(int mark)
    {
        if (mark < 0)
        {
            throw new ArgumentException("���ֲ���Ϊ������");
        }

        Mark += mark;
        Debug.Log($"���� {Name} �������� {mark}����ǰ�����֣�{Mark}");
    }
    #endregion

    #region ���߷���
    // ��ȡ��ǰ���ȵ��ܼ�ֵ
    public int GetTotalValue()
    {
        return BasePrice + Profit;
    }

    // ����Ƿ�ﵽ���ȼ�
    public bool IsMaxLevel()
    {
        return Level >= MaxLevel;
    }
    #endregion
}
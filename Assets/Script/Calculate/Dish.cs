using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

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
    public int TotalProfit { get; private set; }

    public int TotalScore { get; private set; }  // �ܷ�����ȷ���ȼ�
    public int Level { get; private set; }       // �����ܷ��Զ�����
    #endregion

    #region ���ò���
    private const int MaxLevel = 5;         // ���ȼ�
    private const int ScorePerLevel = 100;  // ÿ����Ҫ100��
    #endregion

    #region ��ʼ��
    public Dish(
        string name,
        Rarity rarity,
        int basePrice,
        RecipeSystem recipeSystem, // �����䷽ϵͳ
        List<Ingredient> ingredients // ����ʳ���б�
    )
    {
        Name = name;
        Rarity = rarity;
        BasePrice = basePrice;
        TotalProfit = TotalProfit;
        Level = 1; // ��ʼ�ȼ�
        TotalScore = 0;  // ��ʼ����
    }
    
    #endregion

    #region �����߼�
    // �����ֲܷ����µȼ�
    public void UpdateScoreAndLevel(RecipeSystem recipeSystem, List<Ingredient> ingredients)
    {
        // �����䷽��
        int recipeScore = recipeSystem.TotalScore;

        // ����ʳ��ƽ����
        int totalIngredientScore = 0;
        foreach (var ingredient in ingredients)
        {
            totalIngredientScore += ingredient.TotalScore;
        }
        int ingredientScore = ingredients.Count > 0
            ? totalIngredientScore / ingredients.Count
            : 0;

        // �����ܷ֣�������500��
        TotalScore = Mathf.Min(recipeScore + ingredientScore, 500);

        // �����ܷ��Զ�ȷ���ȼ�
        int newLevel = Mathf.Min(Mathf.FloorToInt(TotalScore / (float)ScorePerLevel) + 1, MaxLevel);
        if (newLevel != Level)
        {
            Level = newLevel;
            Debug.Log($"���� {Name} �ȼ������� Lv.{Level}");
        }
    }
    #endregion

    #region ���߷���
    // ��ȡ��ǰ������
    public int CalculateProfit(RecipeSystem recipeSystem, List<Ingredient> ingredients)
    {
        // �䷽��������
        int recipeProfit = recipeSystem.TotalScore / 10;

        // ʳ���ܳɱ�
        int ingredientCost = 0;
        foreach (var ingredient in ingredients)
        {
            ingredientCost += ingredient._config.basePurchaseCost *
                             (int)ingredient.Rarity *
                             ingredient.IngredientLevel;
        }

        // �������� = �䷽���� - ʳ�ĳɱ�
        return Mathf.Max(recipeProfit - ingredientCost, 0);
    }
    public int GetTotalProfit() => TotalProfit;

    public bool IsMaxLevel() => Level >= MaxLevel;
    #endregion
}
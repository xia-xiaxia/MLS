using System;
using System.Collections.Generic;
using UnityEngine;

//�����ɳ����߿�ѡ��ʽ
public enum UpgradeFormula
{
    Linear,      // ���ԣ�cost = param1 * level
    Exponential, // ָ����cost = param1 * (param2^level)
    Polynomial,  // ����ʽ��cost = param1 * level^param2
    CustomCurve  // �Զ������ߣ������AnimationCurve��
}

// ʳ����������
[CreateAssetMenu(menuName = "Data/IngredientConfig")]
public class IngredientConfig : ScriptableObject
{
    public int id;
    public string displayName;
    public Rarity baseRarity = Rarity.white;
    public Sprite ingredientImage;

    [Header("��ϡ�е�����")]
    public int[] rarityUpgradeCosts = { 10, 50, 200, 500, 1500 };
    public int maxLevel = 10;
    public int levelUpgradeBaseCost = 100;

    [Header("����ϵ��")]
    public int scorePerLevel = 15;
    public int scorePerRarity = 20;

    [Header("�ȼ�����")]
    public UpgradeFormula levelFormula = UpgradeFormula.Linear;
    public int levelBaseCost = 100;
    public float levelGrowthFactor = 1.2f;

    [Header("ϡ�ж�ϵ��")]
    public float rarityProfitMultiplier = 30f; // ÿ��ϡ�жȵ�����ӳ�
    public float rarityCostMultiplier = 15f;   // ÿ��ϡ�жȵĲɹ��Ѽӳ�

    [Header("�ȼ�ϵ��")]
    public int baseProfit = 15;      // ÿ�ȼ�����������ֵ
    public int basePurchaseCost = 30;// �����ɹ���

    [Header("�����䷽")]
    public List<Recipe> compatibleRecipes;
}

// ʳ�ĺ����߼�
public class Ingredient
{
    public string ID { get; }
    public string IngredientName { get; }
    public Rarity CurrentRarity { get; private set; }
    public int CurrentLevel { get; private set; }
    public int TotalScore { get; private set; }
    public Sprite IngredientImage{ get; }
    [TextArea]
    public string IngredientDescription;

    private readonly IngredientConfig _config;

    public Ingredient(string id, string displayname,Sprite ingredientImage,IngredientConfig config)
    {
        ID = id;
        IngredientName = displayname;
        IngredientImage = ingredientImage;
        _config = config;
        ResetState();
    }

    public void ResetState()
    {
        CurrentRarity = _config.baseRarity;
        CurrentLevel = 1;
        CalculateScore();
    }
    public void UpgradeRarity(Rarity newRarity)
    {
        if (newRarity <= CurrentRarity) return;

        CurrentRarity = newRarity;
        CalculateScore();
        IngredientEvents.RaiseRarityUpgraded(this);
    }

    public void UpgradeLevel()
    {
        if (CurrentLevel >= _config.maxLevel) return;

        CurrentLevel++;
        CalculateScore();
        IngredientEvents.RaiseLevelUpgraded(this);
    }

    private void CalculateScore()
    {
        TotalScore = CurrentLevel * _config.scorePerLevel +
                    ((int)CurrentRarity * _config.scorePerRarity);
    }
}

// �ɱ��������
public static class UpgradeCostCalculator
{
    public static int CalculateCost(UpgradeFormula type, int currentLevel, float param1, float param2)
    {
        return type switch
        {
            UpgradeFormula.Linear =>
                Mathf.RoundToInt(param1 * currentLevel),

            UpgradeFormula.Exponential =>
                Mathf.RoundToInt(param1 * Mathf.Pow(param2, currentLevel)),

            UpgradeFormula.Polynomial =>
                Mathf.RoundToInt(param1 * Mathf.Pow(currentLevel, param2)),

            _ => Mathf.RoundToInt(param1 * currentLevel) // Ĭ������
        };
    }

    // �Զ������߰汾������config�����AnimationCurve�ֶΣ�
    public static int CalculateCostByCurve(AnimationCurve curve, int currentLevel)
    {
        return Mathf.RoundToInt(curve.Evaluate(currentLevel));
    }
}

// ʳ����������
public class IngredientUpgradeService
{
    public bool TryUpgradeRarity(
        Ingredient ingredient,
        PlayerInventory inventory,
        IngredientConfig config)
    {
        // ��ȡ��һϡ�ж�
        var nextRarity = GetNextRarity(ingredient.CurrentRarity);

        // ��֤����
        if (nextRarity == ingredient.CurrentRarity) return false;
        if (!GetRarityUpgradeCost(ingredient.CurrentRarity, config, out var cost)) return false;
        if (!inventory.ingredients.TryGetValue(ingredient.ID, out var data)) return false;
        if (data.fragments < cost) return false;

        // ִ������
        data.fragments -= cost;
        ingredient.UpgradeRarity(nextRarity); // ������������
        return true;
    }
    public bool TryUpgradeLevel(
       Ingredient ingredient,
       PlayerInventory inventory,
       IngredientConfig config,
       ref int gold)
    {
        if (ingredient.CurrentLevel >= config.maxLevel) 
            return false;

        int cost = config.levelUpgradeBaseCost * ingredient.CurrentLevel;
        if (gold < cost) 
            return false;

        gold -= cost;
        ingredient.UpgradeLevel(); // �������з���
        return true;
    }
    // ��ȡ��һϡ�жȵķ���
    private Rarity GetNextRarity(Rarity current) => current switch
    {
        Rarity.white => Rarity.Green,
        Rarity.Green => Rarity.Blue,
        Rarity.Blue => Rarity.Purple,
        Rarity.Purple => Rarity.Gold,
        Rarity.Gold => Rarity.Rainbow,
        Rarity.Rainbow =>Rarity.Rainbow,
        _ => current
    };
    private bool GetRarityUpgradeCost(Rarity current, IngredientConfig config, out int cost)
    {
        int index = current switch
        {
            Rarity.white => 0,
            Rarity.Green => 1,
            Rarity.Blue => 2,
            Rarity.Purple => 3,
            Rarity.Gold => 4,
            _ => -1
        };

        if (index >= 0 && index < config.rarityUpgradeCosts.Length)
        {
            cost = config.rarityUpgradeCosts[index];
            return true;
        }

        cost = int.MaxValue;
        return false;
    }

    private bool IsMaxRarity(Rarity rarity) => rarity >= Rarity.Rainbow;
}

// �¼�ϵͳ
public static class IngredientEvents
{
    public static event Action<Ingredient> OnLevelUpgraded;
    public static event Action<Ingredient> OnRarityUpgraded;

    public static void RaiseLevelUpgraded(Ingredient ingredient) =>
        OnLevelUpgraded?.Invoke(ingredient);

    public static void RaiseRarityUpgraded(Ingredient ingredient) =>
        OnRarityUpgraded?.Invoke(ingredient);
}
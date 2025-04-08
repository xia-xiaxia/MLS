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
[CreateAssetMenu(fileName = "New Ingredient", menuName = "Inventory/Ingredient")]

public class IngredientConfig : ScriptableObject
{
    [Header("Ingredient Info")]
    public string IngredientName;//ʳ������
    public Sprite IngredientImage;//ͼ��
    [TextArea]
    public string IngredientDescription;//����
    public int IngredientID;//ʳ��ID
    public int IngredientLevel;//ʳ�ĵȼ�
    public float IngredientPrice;//ʳ�Ĳɹ��۸�
    public Rarity baseRarity = Rarity.white;//ϡ�ж�

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
    public int rarityProfitMultiplier = 30; // ÿ��ϡ�жȵ�����ӳ�
    public int rarityCostMultiplier = 15;   // ÿ��ϡ�жȵĲɹ��Ѽӳ�

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
    public Rarity Rarity;//��CurrentRarity��ΪRarity
    public int IngredientLevel { get; private set; }//��CurrentLevel��ΪIngredientLevel
    public int TotalScore { get; private set; }
    public Sprite IngredientImage { get; }
    [TextArea]
    public string IngredientDescription;

    public readonly IngredientConfig _config;

    public Ingredient(string id, string displayname, Sprite ingredientImage, IngredientConfig config)
    {
        ID = id;
        IngredientName = displayname;
        IngredientImage = ingredientImage;
        _config = config;
        ResetState();
    }

    public void ResetState()
    {
        Rarity = _config.baseRarity;
        IngredientLevel = 1;
        CalculateIngredientScore();
    }
    public void UpgradeRarity(Rarity newRarity)
    {
        if (newRarity <= Rarity) return;

        Rarity = newRarity;
        CalculateIngredientScore();
        IngredientEvents.RaiseRarityUpgraded(this);
    }

    public void UpgradeLevel()
    {
        if (IngredientLevel >= _config.maxLevel) return;

        IngredientLevel++;
        CalculateIngredientScore();
        IngredientEvents.RaiseLevelUpgraded(this);
    }

    public void CalculateIngredientScore()
    {
        int rarityScore = Mathf.Min(
            (int)Rarity * _config.scorePerRarity,
            100 // ϡ�ж�����100��
        );

        int levelScore = Mathf.Min(
            IngredientLevel * _config.scorePerLevel,
            150 // �ȼ�����150��
        );

        TotalScore = rarityScore + levelScore;
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
public class AutoIngredientUpgrader
{
    private readonly IngredientUpgradeService _upgradeService;
    private readonly PlayerInventory _inventory;
    private readonly int _goldBudgetPerTick; // ÿ֡/ÿ���ڷ�����ٽ�����Զ�����

    public AutoIngredientUpgrader(IngredientUpgradeService upgradeService, PlayerInventory inventory, int goldBudgetPerTick)
    {
        _upgradeService = upgradeService;
        _inventory = inventory;
        _goldBudgetPerTick = goldBudgetPerTick;
    }

    public void AutoUpgradeAllIngredients(List<Ingredient> ingredients, Dictionary<string, IngredientConfig> configs, ref int gold)
    {
        foreach (var ingredient in ingredients)
        {
            var config = configs[ingredient.ID];

            // �Զ���ϡ�жȣ�һ���������
            while (_upgradeService.TryUpgradeRarity(ingredient, _inventory, config)) { }

            // �Զ����ȼ���ѭ������ֱ����Ҳ��������
            while (true)
            {
                int cost = UpgradeCostCalculator.CalculateCost(
                    config.levelFormula,
                    ingredient.IngredientLevel,
                    config.levelBaseCost,
                    config.levelGrowthFactor
                );

                if (ingredient.IngredientLevel >= config.maxLevel || gold < cost) break;

                gold -= cost;
                ingredient.UpgradeLevel();
            }
        }
    }
}

public class IngredientUpgradeService
{
    public bool TryUpgradeRarity(
        Ingredient ingredient,
        PlayerInventory inventory,
        IngredientConfig config)
    {
        // ��ȡ��һϡ�ж�
        var nextRarity = GetNextRarity(ingredient.Rarity);

        // ��֤����
        if (nextRarity == ingredient.Rarity) return false;
        if (!GetRarityUpgradeCost(ingredient.Rarity, config, out var cost)) return false;
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
        if (ingredient.IngredientLevel >= config.maxLevel)
            return false;

        int cost = config.levelUpgradeBaseCost * ingredient.IngredientLevel;
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
        Rarity.Rainbow => Rarity.Rainbow,
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
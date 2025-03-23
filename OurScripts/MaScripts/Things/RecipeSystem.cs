using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static RecipeConfig;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe")]
public class Recipe : ScriptableObject
{
    [Header("Recipe Info")]
    public string RecipeName;
    public Sprite RecipeImage;
    [TextArea]
    public string RecipeDescription;
    public int RecipeID;
    public int RecipeLevel;
    public float RecipePrice;
    public float RecipeCost;
    public List<IngredientRequirement> requirements = new List<IngredientRequirement>();
}
// �ɳ�����
[System.Serializable]
public class RecipeProgressData
{
    public int recipeId;
    public Rarity rarity;
    public int level;
    public int exp;
}
[CreateAssetMenu(fileName = "RecipeLevelConfig", menuName = "Data/RecipeLevelConfig")]
public class RecipeLevelConfig : ScriptableObject
{
    [Header("��������")]
    public int maxLevel = 5;
    public int scorePerLevel = 20;
    public int scorePerRarity = 30;
    public int[] expRequirements = { 1, 10, 40, 100, 200 };

    [Header("ϡ�ж�ϵ��")]
    public float rarityMultiplier = 1.5f;
    public int baseUnlockCost = 100;

    public int GetRequiredExp(int currentLevel)
    {
        return currentLevel < expRequirements.Length ?
            expRequirements[currentLevel] : int.MaxValue;
    }
}

[CreateAssetMenu(menuName = "Data/Recipe")]
public class RecipeConfig : ScriptableObject
{
    public int id;
    public string recipeName;

    [Header("ϡ�ж�����")]
    public int rarityBaseProfit = 50;    // �䷽ϡ�жȻ�������

    [Header("�ȼ�����")]
    public int levelProfitIncrement = 40;// ÿ�����ӵ�����

    [Header("��������")]
    public int baseProfit = 200;        // ��������

    [Header("����ʳ��")]
    public List<IngredientRequirement> requirements;

    [System.Serializable]
    public class IngredientRequirement
    {
        public IngredientConfig ingredient;
        public int amount;
        public Ingredient ingredients;
        public string IngredientName;
    }
}
// Recipe��ֵϵͳ
public class RecipeSystem
{
    #region ��������
    private readonly Recipe _baseRecipe; // ������
    private readonly RecipeLevelConfig _config;

    // ��������
    public Rarity CurrentRarity { get; private set; }
    public int CurrentLevel { get; private set; }
    public int TotalScore { get; private set; }
    public bool IsUnlocked => CurrentRarity > Rarity.white;
    #endregion

    #region ��ʼ��
    public RecipeSystem(Recipe baseRecipe, RecipeLevelConfig config)
    {
        _baseRecipe = baseRecipe;
        _config = config;

        // ��ʼ״̬
        CurrentRarity = Rarity.white;
        CurrentLevel = 1;
        CalculateScore();
    }
    #endregion

    #region �����߼�
    public bool TryUpgrade(int availableExp)
    {
        if (!CanUpgrade(availableExp)) return false;

        availableExp -= GetRequiredExp();
        CurrentLevel = Mathf.Min(CurrentLevel + 1, _config.maxLevel);
        CalculateScore();

        RecipeEvents.RaiseUpgraded(this);
        return true;
    }

    public bool TryUnlock(int availableGold)
    {
        if (IsUnlocked || availableGold < _config.baseUnlockCost)
            return false;

        CurrentRarity = Rarity.Green;
        CalculateScore();

        RecipeEvents.RaiseUnlocked(this);
        return true;
    }

    public bool CanUpgrade(int availableExp)
    {
        return CurrentLevel < _config.maxLevel && availableExp >= GetRequiredExp();
    }
    #endregion

    // ��ֵ����
    private void CalculateScore()
    {
        float rarityBonus = Mathf.Pow((int)CurrentRarity, _config.rarityMultiplier);
        TotalScore = Mathf.RoundToInt(
            (CurrentLevel * _config.scorePerLevel) +
            (rarityBonus * _config.scorePerRarity)
        );
    }

    private int GetRequiredExp()
    {
        return _config.GetRequiredExp(CurrentLevel);
    }

     //UI�Ž�
    public string DisplayName => _baseRecipe.RecipeName;
    public Sprite DisplayIcon => _baseRecipe.RecipeImage;
    public string StatusText =>$"Lv.{CurrentLevel} [{CurrentRarity}] Score: {TotalScore}";
}

// �¼�ͨ��ϵͳ
public static class RecipeEvents
{
    public static event System.Action<RecipeSystem> OnUpgraded;
    public static event System.Action<RecipeSystem> OnUnlocked;//�����²���

    public static void RaiseUpgraded(RecipeSystem recipe) => OnUpgraded?.Invoke(recipe);
    public static void RaiseUnlocked(RecipeSystem recipe) => OnUnlocked?.Invoke(recipe);
}


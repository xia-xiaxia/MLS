using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ����ϵͳ������ģʽ��
public class KitchenSystem : MonoBehaviour
{
    #region ����ʵ��
    public static KitchenSystem Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region ��������
    [System.Serializable]
    public class RecipeConfig
    {
        public string recipeID;
        public float baseCookTime = 10f;
        public Dictionary<string, int> requiredIngredients; // ʳ��ID:����
    }

    [Header("��������")]
    [SerializeField] private List<RecipeConfig> allRecipes = new List<RecipeConfig>();
    [SerializeField] private int maxConcurrentDishes = 3;
    #endregion

    #region ����ʱ����
    // ��ǰ���������Ĳ���
    public class CookingDish
    {
        public string recipeID;
        public float progress; // 0-1
        public float speedMultiplier = 1f;
    }
    private List<CookingDish> cookingQueue = new List<CookingDish>();

    // �ܻ��Ӱ��Ĳ���
    [Header("���Ч������")]
    public float cookSpeedMultiplier = 1f; // ������ٶȱ���
    public int extraConcurrentSlots = 0;   // ����ͬʱ��������
    #endregion

    #region ʳ�Ŀ��
    private Dictionary<string, int> ingredientStock = new Dictionary<string, int>();

    // ��ʼ����棨ʾ����
    public void InitializeIngredients(Dictionary<string, int> initialStock)
    {
        ingredientStock = new Dictionary<string, int>(initialStock);
    }
    #endregion

    #region �����߼�
    void Update()
    {
        UpdateCookingProgress();
    }

    // ��ʼ��⿲���
    public bool StartCooking(string recipeID)
    {
        RecipeConfig recipe = GetRecipe(recipeID);
        if (recipe == null)
        {
            Debug.LogError($"δ֪ʳ��: {recipeID}");
            return false;
        }

        // �������
        if (cookingQueue.Count >= GetMaxConcurrentDishes())
        {
            Debug.Log("��������");
            return false;
        }

        // ���ʳ��
        if (!HasEnoughIngredients(recipe))
        {
            Debug.Log("ʳ�Ĳ���");
            return false;
        }

        // �۳�ʳ��
        ConsumeIngredients(recipe);

        // ��ӵ�����
        cookingQueue.Add(new CookingDish
        {
            recipeID = recipeID,
            progress = 0f,
            speedMultiplier = cookSpeedMultiplier
        });

        OnCookingStarted?.Invoke(recipeID);
        return true;
    }

    // ������⿽���
    private void UpdateCookingProgress()
    {
        for (int i = cookingQueue.Count - 1; i >= 0; i--)
        {
            CookingDish dish = cookingQueue[i];
            dish.progress += Time.deltaTime / GetAdjustedCookTime(dish);

            if (dish.progress >= 1f)
            {
                CompleteDish(dish);
                cookingQueue.RemoveAt(i);
            }
        }
    }

    // ��ɲ���
    private void CompleteDish(CookingDish dish)
    {
        Debug.Log($"�������: {dish.recipeID}");
        OnDishCompleted?.Invoke(dish.recipeID);
    }
    #endregion

    #region ��ֵ����
    // ��ȡ��ǰ���ͬʱ��������
    public int GetMaxConcurrentDishes()
    {
        return maxConcurrentDishes + extraConcurrentSlots;
    }

    // ����ʵ�����ʱ�䣨�ܻ��Ч��Ӱ�죩
    private float GetAdjustedCookTime(CookingDish dish)
    {
        RecipeConfig recipe = GetRecipe(dish.recipeID);
        return recipe.baseCookTime / dish.speedMultiplier;
    }

    // ���ʳ���Ƿ��㹻
    private bool HasEnoughIngredients(RecipeConfig recipe)
    {
        foreach (var ingredient in recipe.requiredIngredients)
        {
            if (!ingredientStock.TryGetValue(ingredient.Key, out int stock) ||
                stock < ingredient.Value)
            {
                return false;
            }
        }
        return true;
    }

    // �۳�ʳ��
    private void ConsumeIngredients(RecipeConfig recipe)
    {
        foreach (var ingredient in recipe.requiredIngredients)
        {
            ingredientStock[ingredient.Key] -= ingredient.Value;
        }
    }
    #endregion

    #region �ⲿ�ӿ�
    // ��ȡʳ������
    public RecipeConfig GetRecipe(string recipeID)
    {
        return allRecipes.Find(r => r.recipeID == recipeID);
    }

    // ��ȡ��ǰ��⿶���
    public IReadOnlyList<CookingDish> GetCookingQueue() => cookingQueue;

    // ���»��Ч������PartnerSystem���ã�
    public void UpdatePartnerEffects(float speedMultiplier, int extraSlots)
    {
        cookSpeedMultiplier = speedMultiplier;
        extraConcurrentSlots = extraSlots;

        // �������в��ȵ�����ٶ�
        foreach (var dish in cookingQueue)
        {
            dish.speedMultiplier = cookSpeedMultiplier;
        }
    }
    #endregion

    #region �¼�ϵͳ
    public event System.Action<string> OnCookingStarted;
    public event System.Action<string> OnDishCompleted;
    #endregion
}

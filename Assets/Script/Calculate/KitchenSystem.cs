using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 厨房系统（单例模式）
public class KitchenSystem : MonoBehaviour
{
    #region 单例实例
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

    #region 数据配置
    [System.Serializable]
    public class RecipeConfig
    {
        public string recipeID;
        public float baseCookTime = 10f;
        public Dictionary<string, int> requiredIngredients; // 食材ID:数量
    }

    [Header("基础配置")]
    [SerializeField] private List<RecipeConfig> allRecipes = new List<RecipeConfig>();
    [SerializeField] private int maxConcurrentDishes = 3;
    #endregion

    #region 运行时数据
    // 当前正在制作的菜肴
    public class CookingDish
    {
        public string recipeID;
        public float progress; // 0-1
        public float speedMultiplier = 1f;
    }
    private List<CookingDish> cookingQueue = new List<CookingDish>();

    // 受伙伴影响的参数
    [Header("伙伴效果参数")]
    public float cookSpeedMultiplier = 1f; // 总烹饪速度倍率
    public int extraConcurrentSlots = 0;   // 额外同时制作数量
    #endregion

    #region 食材库存
    private Dictionary<string, int> ingredientStock = new Dictionary<string, int>();

    // 初始化库存（示例）
    public void InitializeIngredients(Dictionary<string, int> initialStock)
    {
        ingredientStock = new Dictionary<string, int>(initialStock);
    }
    #endregion

    #region 核心逻辑
    void Update()
    {
        UpdateCookingProgress();
    }

    // 开始烹饪菜肴
    public bool StartCooking(string recipeID)
    {
        RecipeConfig recipe = GetRecipe(recipeID);
        if (recipe == null)
        {
            Debug.LogError($"未知食谱: {recipeID}");
            return false;
        }

        // 检查容量
        if (cookingQueue.Count >= GetMaxConcurrentDishes())
        {
            Debug.Log("厨房已满");
            return false;
        }

        // 检查食材
        if (!HasEnoughIngredients(recipe))
        {
            Debug.Log("食材不足");
            return false;
        }

        // 扣除食材
        ConsumeIngredients(recipe);

        // 添加到队列
        cookingQueue.Add(new CookingDish
        {
            recipeID = recipeID,
            progress = 0f,
            speedMultiplier = cookSpeedMultiplier
        });

        OnCookingStarted?.Invoke(recipeID);
        return true;
    }

    // 更新烹饪进度
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

    // 完成菜肴
    private void CompleteDish(CookingDish dish)
    {
        Debug.Log($"菜肴完成: {dish.recipeID}");
        OnDishCompleted?.Invoke(dish.recipeID);
    }
    #endregion

    #region 数值计算
    // 获取当前最大同时制作数量
    public int GetMaxConcurrentDishes()
    {
        return maxConcurrentDishes + extraConcurrentSlots;
    }

    // 计算实际烹饪时间（受伙伴效果影响）
    private float GetAdjustedCookTime(CookingDish dish)
    {
        RecipeConfig recipe = GetRecipe(dish.recipeID);
        return recipe.baseCookTime / dish.speedMultiplier;
    }

    // 检查食材是否足够
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

    // 扣除食材
    private void ConsumeIngredients(RecipeConfig recipe)
    {
        foreach (var ingredient in recipe.requiredIngredients)
        {
            ingredientStock[ingredient.Key] -= ingredient.Value;
        }
    }
    #endregion

    #region 外部接口
    // 获取食谱配置
    public RecipeConfig GetRecipe(string recipeID)
    {
        return allRecipes.Find(r => r.recipeID == recipeID);
    }

    // 获取当前烹饪队列
    public IReadOnlyList<CookingDish> GetCookingQueue() => cookingQueue;

    // 更新伙伴效果（由PartnerSystem调用）
    public void UpdatePartnerEffects(float speedMultiplier, int extraSlots)
    {
        cookSpeedMultiplier = speedMultiplier;
        extraConcurrentSlots = extraSlots;

        // 更新现有菜肴的烹饪速度
        foreach (var dish in cookingQueue)
        {
            dish.speedMultiplier = cookSpeedMultiplier;
        }
    }
    #endregion

    #region 事件系统
    public event System.Action<string> OnCookingStarted;
    public event System.Action<string> OnDishCompleted;
    #endregion
}

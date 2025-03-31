using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    // 单例
    public static GameManager Instance { get; private set; }

    // 通过DataManager访问
    [Header("系统引用")]
    public QuickPurchase gachaSystem;
    public EconomySystem economySystem;

    [SerializeField] private PartnerSystem partnerSystem;
    [SerializeField] private List<PartnerConfig> partnerConfigs;

    private void Start()
    {
        // 实现资源服务接口
        IResourceService resourceService = new BasicResourceService();
        partnerSystem.Initialize(resourceService);
    }

    void Awake()
    {
        // 单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeSystems()
    {
        // 同步经济系统初始状态
        economySystem.Initialize(GameDataManager.Instance.totalGold);
    }

    // 抽卡处理
    public void ProcessGachaResult(ShoppingItem item)
    {
        var dataManager = GameDataManager.Instance;

        switch (item.itemType)
        {
            case CardType.Recipe:
                HandleRecipe(item, dataManager);
                break;

            case CardType.Ingredient:
                HandleIngredient(item, dataManager);
                break;

            case CardType.Partner:
                HandlePartner(item, dataManager);
                break;
        }

        // 统一数据同步
        dataManager.totalGold = economySystem._currentGold;
        dataManager.SaveGame();
    }

    // 配方处理
    private void HandleRecipe(ShoppingItem item, GameDataManager dataManager)
    {
        bool isNew = !dataManager.recipeMaxRarity.ContainsKey(item.itemID);
        Rarity currentRarity = isNew ? Rarity.white : dataManager.recipeMaxRarity[item.itemID];

        if (item.rarity > currentRarity)
        {
            // 稀有度升级逻辑
            int expValue = GetRecipeExpValue(currentRarity);
            dataManager.AddRecipeExperience(expValue);
            dataManager.UpdateRecipeMaxRarity(item.itemID, item.rarity);

            Debug.Log($"配方升级稀有度：{item.itemID} → {item.rarity} (+{expValue}经验)");
        }
        else
        {
            // 经验转换逻辑
            dataManager.AddRecipeExperience(item.conversionValue);
            Debug.Log($"获得配方经验：+{item.conversionValue}");
        }
    }
    // 碎片转换表
    private int GetFragmentValue(Rarity r) => r switch
    {
        Rarity.Green => 1,
        Rarity.Blue => 2,
        Rarity.Purple => 4,
        Rarity.Gold => 8,
        Rarity.Rainbow => 15,
        _ => 0 // 白色品质不产生碎片
    };
    // 食材处理
    private void HandleIngredient(ShoppingItem item, GameDataManager dataManager)
    {
        // 获取玩家库存引用
        var inventory = dataManager.playerInventory; 

        // 获取或初始化食材数据
        if (!inventory.ingredients.TryGetValue(item.itemID, out var data))
        {
            data = new PlayerInventory.IngredientData
            {
                highestRarity = Rarity.white,
                fragments = 0,
                currentLevel = 1
            };
            inventory.ingredients.Add(item.itemID, data);
        }

        // 更新最高稀有度
        if (item.rarity > data.highestRarity)
        {
            data.highestRarity = item.rarity;
            Debug.Log($"食材{item.itemID}最高稀有度提升至{item.rarity}");
        }

        // 累加碎片（根据稀有度转换表）
        int fragmentValue = GetFragmentValue(item.rarity);
        data.fragments += fragmentValue;

        Debug.Log($"获得{item.itemID}碎片+{fragmentValue}，当前：{data.fragments}");
    }
    // 伙伴处理
    private void HandlePartner(ShoppingItem item, GameDataManager dataManager)
    {
        string partnerID = ParsePartnerID(item.itemID);
        int total = dataManager.totalPartnerFragments + item.conversionValue;

        // 完整伙伴检测
        if (total >= 10)
        {
            dataManager.SaveFullPartner(partnerID);
            dataManager.totalPartnerFragments = total % 10;
            Debug.Log($"解锁完整伙伴：{partnerID}");
        }
        else
        {
            dataManager.totalPartnerFragments = total;
            Debug.Log($"伙伴碎片进度：{total}/10");
        }
    }
    private string ParsePartnerID(string rawID) => rawID.Split('_')[0];

    // 经验对照表
    private static readonly Dictionary<Rarity, int> RecipeExpTable = new()
    {
        [Rarity.Green] = 1,
        [Rarity.Blue] = 2,
        [Rarity.Purple] = 3,
        [Rarity.Gold] = 5,
        [Rarity.Rainbow] = 10
    };
    private int GetRecipeExpValue(Rarity r) =>RecipeExpTable.TryGetValue(r, out var value) ? value : 0;
}

// 存储每日统计数据
public struct DailyStats
{
    public int day;
    public int customers;
    public int revenue;
    public int expenses;
    public int profit;
    public int totalGold; // 结算后的金币余额
    public int maxDishes;    
    public int actualSales;  
}

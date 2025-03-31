using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    // ����
    public static GameManager Instance { get; private set; }

    // ͨ��DataManager����
    [Header("ϵͳ����")]
    public QuickPurchase gachaSystem;
    public EconomySystem economySystem;

    [SerializeField] private PartnerSystem partnerSystem;
    [SerializeField] private List<PartnerConfig> partnerConfigs;

    private void Start()
    {
        // ʵ����Դ����ӿ�
        IResourceService resourceService = new BasicResourceService();
        partnerSystem.Initialize(resourceService);
    }

    void Awake()
    {
        // ����
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
        // ͬ������ϵͳ��ʼ״̬
        economySystem.Initialize(GameDataManager.Instance.totalGold);
    }

    // �鿨����
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

        // ͳһ����ͬ��
        dataManager.totalGold = economySystem._currentGold;
        dataManager.SaveGame();
    }

    // �䷽����
    private void HandleRecipe(ShoppingItem item, GameDataManager dataManager)
    {
        bool isNew = !dataManager.recipeMaxRarity.ContainsKey(item.itemID);
        Rarity currentRarity = isNew ? Rarity.white : dataManager.recipeMaxRarity[item.itemID];

        if (item.rarity > currentRarity)
        {
            // ϡ�ж������߼�
            int expValue = GetRecipeExpValue(currentRarity);
            dataManager.AddRecipeExperience(expValue);
            dataManager.UpdateRecipeMaxRarity(item.itemID, item.rarity);

            Debug.Log($"�䷽����ϡ�жȣ�{item.itemID} �� {item.rarity} (+{expValue}����)");
        }
        else
        {
            // ����ת���߼�
            dataManager.AddRecipeExperience(item.conversionValue);
            Debug.Log($"����䷽���飺+{item.conversionValue}");
        }
    }
    // ��Ƭת����
    private int GetFragmentValue(Rarity r) => r switch
    {
        Rarity.Green => 1,
        Rarity.Blue => 2,
        Rarity.Purple => 4,
        Rarity.Gold => 8,
        Rarity.Rainbow => 15,
        _ => 0 // ��ɫƷ�ʲ�������Ƭ
    };
    // ʳ�Ĵ���
    private void HandleIngredient(ShoppingItem item, GameDataManager dataManager)
    {
        // ��ȡ��ҿ������
        var inventory = dataManager.playerInventory; 

        // ��ȡ���ʼ��ʳ������
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

        // �������ϡ�ж�
        if (item.rarity > data.highestRarity)
        {
            data.highestRarity = item.rarity;
            Debug.Log($"ʳ��{item.itemID}���ϡ�ж�������{item.rarity}");
        }

        // �ۼ���Ƭ������ϡ�ж�ת����
        int fragmentValue = GetFragmentValue(item.rarity);
        data.fragments += fragmentValue;

        Debug.Log($"���{item.itemID}��Ƭ+{fragmentValue}����ǰ��{data.fragments}");
    }
    // ��鴦��
    private void HandlePartner(ShoppingItem item, GameDataManager dataManager)
    {
        string partnerID = ParsePartnerID(item.itemID);
        int total = dataManager.totalPartnerFragments + item.conversionValue;

        // ���������
        if (total >= 10)
        {
            dataManager.SaveFullPartner(partnerID);
            dataManager.totalPartnerFragments = total % 10;
            Debug.Log($"����������飺{partnerID}");
        }
        else
        {
            dataManager.totalPartnerFragments = total;
            Debug.Log($"�����Ƭ���ȣ�{total}/10");
        }
    }
    private string ParsePartnerID(string rawID) => rawID.Split('_')[0];

    // ������ձ�
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

// �洢ÿ��ͳ������
public struct DailyStats
{
    public int day;
    public int customers;
    public int revenue;
    public int expenses;
    public int profit;
    public int totalGold; // �����Ľ�����
    public int maxDishes;    
    public int actualSales;  
}

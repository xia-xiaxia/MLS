using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerInventory inventory;
    public GachaSystem gachaSystem;
    public EconomySystem economySystem;

    void Awake()
    {
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
        // 同步初始金币
        economySystem.gold = GameDataManager.Instance.totalGold;
    }

    // 处理抽卡结果
    public void ProcessGachaResult(ShoppingItem item)
    {
        switch (item.itemType)
        {
            case ItemType.Recipe:
                HandleRecipe(item);
                break;

            case ItemType.Ingredient:
                HandleIngredient(item);
                break;

            case ItemType.Partner:
                HandlePartner(item);
                break;
        }

        GameDataManager.Instance.totalGold = economySystem.gold;
    }

    // 抽卡的配方转化
    private void HandleRecipe(ShoppingItem item)
    {
        bool isNewRecipe = !inventory.recipes.ContainsKey(item.itemID);

        if (isNewRecipe)
        {
            // 直接记录新配方
            inventory.recipes.Add(item.itemID, item.rarity);
            Debug.Log($"解锁新配方：{item.itemID}");
        }
        else
        {
            // 获取当前配方稀有度
            Rarity currentRarity = inventory.recipes[item.itemID];

            if (item.rarity > currentRarity)
            {
                // 稀有度升级：替换并补偿经验
                int expValue = GetRecipeExpValue(currentRarity);
                inventory.recipeExpPool += expValue;
                inventory.recipes[item.itemID] = item.rarity;
                Debug.Log($"配方[{item.itemID}]升级到{item.rarity}，获得补偿经验+{expValue}");
            }
            else
            {
                // 转换经验值
                inventory.recipeExpPool += item.conversionValue;
                Debug.Log($"获得配方经验+{item.conversionValue}");
            }
        }
    }

    // 抽卡获得的食材转化
    private void HandleIngredient(ShoppingItem item)
    {
        bool exists = inventory.ingredients.TryGetValue(item.itemID, out var data);

        if (!exists)
        {
            // 初次获得：初始化数据
            data = new PlayerInventory.IngredientData
            {
                highestRarity = item.rarity,
                fragments = 0,
                level = 1
            };
            inventory.ingredients.Add(item.itemID, data);
            Debug.Log($"解锁新食材：{item.itemID}");
        }

        // 处理稀有度变化
        if (item.rarity > data.highestRarity)
        {
            // 获得稀有度提升补偿
            int fragmentValue = GetFragmentValue(data.highestRarity);
            data.fragments += fragmentValue;
            data.highestRarity = item.rarity;
            Debug.Log($"食材[{item.itemID}]最高稀有度提升至{item.rarity}，获得补偿碎片+{fragmentValue}");
        }

        // 累加碎片
        data.fragments += item.conversionValue;
        Debug.Log($"当前碎片：{data.fragments}");
    }

    // 抽卡获得的伙伴碎片转化
    private void HandlePartner(ShoppingItem item)
    {
        string partnerID = item.itemID.Split('_')[0]; // 示例ID处理逻辑

        if (!inventory.partnerFragments.ContainsKey(partnerID))
            inventory.partnerFragments[partnerID] = 0;

        inventory.partnerFragments[partnerID] += item.conversionValue;
        Debug.Log($"伙伴[{partnerID}]碎片+{item.conversionValue}");
    }

    // 配方经验对照表
    private int GetRecipeExpValue(Rarity r) => r switch
    {
        Rarity.Green => 1,
        Rarity.Blue => 2,
        Rarity.Purple => 3,
        Rarity.Gold => 5,
        Rarity.Rainbow => 10,
        _ => 0
    };

    // 食材碎片对照表
    private int GetFragmentValue(Rarity r) => r switch
    {
        Rarity.Green => 1,
        Rarity.Blue => 2,
        Rarity.Purple => 4,
        Rarity.Gold => 8,
        Rarity.Rainbow => 15,
        _ => 0
    };
}

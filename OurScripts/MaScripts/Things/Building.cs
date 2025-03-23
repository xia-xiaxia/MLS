// 建筑类型
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    RestaurantCapacity,  // 餐厅容量
    KitchenCapacity,     // 后厨 厨师容量
    ServiceStaffCapacity,// 招待数量
}

// 单类建筑升级配置
[System.Serializable]
public class BuildingUpgradeConfig
{
    public BuildingType type;
    public int maxLevel = 10;            //?最大等级
    public int baseCost = 100;           // 基础升级成本
    public float costMultiplier = 1.5f;  // 每级成本增长系数
    public float baseValue = 1;          // 基础效果值
    public float valueMultiplier = 1.2f; // 每级效果增长系数
}

// 建筑全局配置
[CreateAssetMenu(menuName = "Building/BuildingConfig")]
public class BuildingGlobalConfig : ScriptableObject
{
    public List<BuildingUpgradeConfig> configs = new();
}

public class BuildingUpgradeManager : MonoBehaviour
{
    [SerializeField] private BuildingGlobalConfig config;
    [SerializeField] private EconomySystem economySystem;
    // 当前等级记录
    private Dictionary<BuildingType, int> currentLevels = new();
    private Dictionary<BuildingType, float> currentValues = new();

    void Awake()
    {
        Initialize();
        LoadProgress();
    }

    // 初始化默认值
    private void Initialize()
    {
        foreach (BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
        {
            currentLevels[type] = 1;
            currentValues[type] = GetBaseValue(type);
        }
    }

    // 获取基础值
    private float GetBaseValue(BuildingType type)
    {
        var cfg = config.configs.Find(c => c.type == type);
        return cfg?.baseValue ?? 1f;
    }

    // 判断是否可以升级
    public bool CanUpgrade(BuildingType type)
    {
        return currentLevels[type] < GetMaxLevel(type) &&economySystem.CurrentGold >= GetUpgradeCost(type);
    }

    public bool TryUpgrade(BuildingType type)
    {
        if (!CanUpgrade(type)) return false;

        int cost = GetUpgradeCost(type);
        bool success = economySystem.SpendGold(
            cost,
            FinanceType.FacilityUpgrade,
            $"{type}升级到等级{currentLevels[type] + 1}"
        );

        if (success)
        {
            currentLevels[type]++;
            currentValues[type] = CalculateCurrentValue(type);
            SaveProgress();
            return true;
        }
        return false;
    }

    // 计算当前等级的效果值
    private float CalculateCurrentValue(BuildingType type)
    {
        var cfg = config.configs.Find(c => c.type == type);
        return cfg.baseValue * Mathf.Pow(cfg.valueMultiplier, currentLevels[type] - 1);
    }

    // 获取升级成本
    public int GetUpgradeCost(BuildingType type)
    {
        var cfg = config.configs.Find(c => c.type == type);
        return Mathf.RoundToInt(cfg.baseCost * Mathf.Pow(cfg.costMultiplier, currentLevels[type] - 1));
    }

    // 获取当前效果值
    public float GetCurrentValue(BuildingType type) => currentValues[type];

    // 获取最大等级
    public int GetMaxLevel(BuildingType type)
    {
        var cfg = config.configs.Find(c => c.type == type);
        return cfg?.maxLevel ?? 1;
    }

    #region 数据持久化
    private const string SAVE_KEY = "BuildingUpgradeData";

    private void SaveProgress()
    {
        var saveData = new SaveData
        {
            levels = currentLevels,
            values = currentValues
        };
        PlayerPrefs.SetString(SAVE_KEY, JsonUtility.ToJson(saveData));
    }

    private void LoadProgress()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");
        if (!string.IsNullOrEmpty(json))
        {
            var saveData = JsonUtility.FromJson<SaveData>(json);
            currentLevels = saveData.levels;
            currentValues = saveData.values;
        }
    }

    [System.Serializable]
    private class SaveData
    {
        public Dictionary<BuildingType, int> levels;
        public Dictionary<BuildingType, float> values;
    }
    #endregion
}
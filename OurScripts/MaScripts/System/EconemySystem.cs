using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

#region 数据结构和配置
// 收支记录条目
public struct FinancialRecord
{
    public DateTime timestamp;
    public string description;
    public int amount;
    public FinanceType type;
}

// 财务类型
public enum FinanceType
{
    // 收入
    DailyRevenue,        // 日常经营收入
    SpecialEventBonus,   // 特殊事件奖励

    // 支出
    GachaSpend,          // 抽卡消费
    IngredientPurchase,  // 食材采购
    FacilityUpgrade,     // 设施升级
    StaffSalary,         // 员工工资
    RecipeResearch       // 配方研发
}

// 经济系统配置（ScriptableObject）
[CreateAssetMenu(menuName = "Economy/EconomyConfig")]
public class EconomyConfig : ScriptableObject
{
    [Header("基础参数")]
    public int initialGold = 1000;
    public int dailyBaseRent = 1000;
    public int dailyBaseSalary = 500;

    [Header("生产参数")]
    public int seats = 5;
    public float dishPrepTime = 0.1f;
    public float tableCleanTime = 60f;
    public float customerMoveTime = 5f;

    [Header("经济系数")]
    public int goldPerDish = 3;
    public int dishesPerCustomer = 2;

    [Header("升级系数")]
    public AnimationCurve upgradeCostCurve;
}

#endregion

#region 核心经济系统
public class EconomySystem : MonoBehaviour
{
    // 当前状态
    public int _currentGold { get; set; }
    private List<FinancialRecord> _financialRecords = new();

    // 配置引用
    [SerializeField] private EconomyConfig _config;
    [SerializeField] private BuildingUpgradeManager buildingManager;
    // 事件系统
    public event Action<int> OnGoldChanged;
    public event Action<FinancialRecord> OnFinancialRecordAdded;

    void Start()
    {
        Initialize(_config.initialGold);
    }

    /// <summary>
    /// 初始化经济系统
    /// </summary>
    public void Initialize(int initialGold)
    {
        _currentGold = initialGold;
        AddFinancialRecord("初始资金", initialGold, FinanceType.DailyRevenue);
    }

    #region 每日经营流程
    public void ProcessDailyOperation()
    {
        // 计算基础经营数据
        int customers = CalculateCustomerCapacity();
        int maxDishes = CalculateMaxProduction();
        int actualDishes = Mathf.Min(customers * _config.dishesPerCustomer, maxDishes);

        // 记录收入
        int revenue = actualDishes * _config.goldPerDish;
        AddGold(revenue, FinanceType.DailyRevenue, "日常经营收入");

        // 记录固定支出
        SpendGold(_config.dailyBaseRent, FinanceType.DailyRevenue, "基础租金");
        SpendGold(_config.dailyBaseSalary, FinanceType.StaffSalary, "员工工资");
    }
    #endregion

    #region 金币操作
    public void AddGold(int amount, FinanceType type, string description = "")
    {
        _currentGold += amount;
        AddFinancialRecord(description, amount, type);
        OnGoldChanged?.Invoke(_currentGold);
    }

    public bool SpendGold(int amount, FinanceType type, string description = "")
    {
        if (_currentGold < amount) return false;

        _currentGold -= amount;
        AddFinancialRecord(description, -amount, type);
        OnGoldChanged?.Invoke(_currentGold);
        return true;
    }
    #endregion

    #region 生产计算
    private int CalculateCustomerCapacity()
    {
        // 获取餐厅容量
        float restaurantCapacity = buildingManager.GetCurrentValue(BuildingType.RestaurantCapacity);
        float totalServiceTime =
            (_config.dishPrepTime * _config.dishesPerCustomer) +
            _config.tableCleanTime +
            _config.customerMoveTime;

        float dailySeconds = 50400f;
        return Mathf.FloorToInt(restaurantCapacity * (dailySeconds / totalServiceTime));
    }
    private int CalculateMaxProduction()
    {
        // 获取厨房容量
        float kitchenCapacity = buildingManager.GetCurrentValue(BuildingType.KitchenCapacity);
        float dailySeconds = 50400f;
        return Mathf.FloorToInt(kitchenCapacity * (dailySeconds / _config.dishPrepTime));
    }
    #endregion

    #region 升级系统
    public int GetUpgradeCost(int currentLevel)
    {
        return Mathf.RoundToInt(_config.upgradeCostCurve.Evaluate(currentLevel));
    }

    public bool TryUpgradeFacility(int currentLevel)
    {
        int cost = GetUpgradeCost(currentLevel);
        return SpendGold(cost, FinanceType.FacilityUpgrade, $"设施升级到Lv{currentLevel + 1}");
    }
    #endregion

    #region 数据管理
    private void AddFinancialRecord(string desc, int amount, FinanceType type)
    {
        var record = new FinancialRecord
        {
            timestamp = DateTime.Now,
            description = desc,
            amount = amount,
            type = type
        };

        _financialRecords.Add(record);
        OnFinancialRecordAdded?.Invoke(record);
    }

    public List<FinancialRecord> GetFinancialReport(FinanceType? filter = null)
    {
        return filter == null
            ? _financialRecords
            : _financialRecords.FindAll(r => r.type == filter);
    }

    public void SaveData(string saveKey)
    {
        PlayerPrefs.SetInt(saveKey + "_Gold", _currentGold);
        PlayerPrefs.SetString(saveKey + "_Records", JsonUtility.ToJson(_financialRecords));
    }

    public void LoadData(string saveKey)
    {
        _currentGold = PlayerPrefs.GetInt(saveKey + "_Gold", _config.initialGold);
        var json = PlayerPrefs.GetString(saveKey + "_Records", "");
    }
    #endregion

    #region 属性访问
    public int CurrentGold => _currentGold;
    public EconomyConfig Config => _config;
    #endregion
}
#endregion


#if UNITY_EDITOR
[CustomEditor(typeof(EconomySystem))]
public class EconomySystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var system = target as EconomySystem;
        if (system == null) return;

        GUILayout.Space(20);
        GUILayout.Label($"当前金币: {system.CurrentGold}");

        if (GUILayout.Button("模拟单日经营"))
        {
            system.ProcessDailyOperation();
        }

        if (GUILayout.Button("显示完整财务报告"))
        {
            DisplayFinancialReport(system);
        }
    }

    private void DisplayFinancialReport(EconomySystem system)
    {
        var report = system.GetFinancialReport();
        var sb = new System.Text.StringBuilder();

        foreach (var record in report)
        {
            sb.AppendLine($"[{record.timestamp}] {record.type}: {record.amount} - {record.description}");
        }

        Debug.Log(sb.ToString());
    }
}
#endif

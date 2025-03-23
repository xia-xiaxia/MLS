using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

#region ���ݽṹ������
// ��֧��¼��Ŀ
public struct FinancialRecord
{
    public DateTime timestamp;
    public string description;
    public int amount;
    public FinanceType type;
}

// ��������
public enum FinanceType
{
    // ����
    DailyRevenue,        // �ճ���Ӫ����
    SpecialEventBonus,   // �����¼�����

    // ֧��
    GachaSpend,          // �鿨����
    IngredientPurchase,  // ʳ�Ĳɹ�
    FacilityUpgrade,     // ��ʩ����
    StaffSalary,         // Ա������
    RecipeResearch       // �䷽�з�
}

// ����ϵͳ���ã�ScriptableObject��
[CreateAssetMenu(menuName = "Economy/EconomyConfig")]
public class EconomyConfig : ScriptableObject
{
    [Header("��������")]
    public int initialGold = 1000;
    public int dailyBaseRent = 1000;
    public int dailyBaseSalary = 500;

    [Header("��������")]
    public int seats = 5;
    public float dishPrepTime = 0.1f;
    public float tableCleanTime = 60f;
    public float customerMoveTime = 5f;

    [Header("����ϵ��")]
    public int goldPerDish = 3;
    public int dishesPerCustomer = 2;

    [Header("����ϵ��")]
    public AnimationCurve upgradeCostCurve;
}

#endregion

#region ���ľ���ϵͳ
public class EconomySystem : MonoBehaviour
{
    // ��ǰ״̬
    public int _currentGold { get; set; }
    private List<FinancialRecord> _financialRecords = new();

    // ��������
    [SerializeField] private EconomyConfig _config;
    [SerializeField] private BuildingUpgradeManager buildingManager;
    // �¼�ϵͳ
    public event Action<int> OnGoldChanged;
    public event Action<FinancialRecord> OnFinancialRecordAdded;

    void Start()
    {
        Initialize(_config.initialGold);
    }

    /// <summary>
    /// ��ʼ������ϵͳ
    /// </summary>
    public void Initialize(int initialGold)
    {
        _currentGold = initialGold;
        AddFinancialRecord("��ʼ�ʽ�", initialGold, FinanceType.DailyRevenue);
    }

    #region ÿ�վ�Ӫ����
    public void ProcessDailyOperation()
    {
        // ���������Ӫ����
        int customers = CalculateCustomerCapacity();
        int maxDishes = CalculateMaxProduction();
        int actualDishes = Mathf.Min(customers * _config.dishesPerCustomer, maxDishes);

        // ��¼����
        int revenue = actualDishes * _config.goldPerDish;
        AddGold(revenue, FinanceType.DailyRevenue, "�ճ���Ӫ����");

        // ��¼�̶�֧��
        SpendGold(_config.dailyBaseRent, FinanceType.DailyRevenue, "�������");
        SpendGold(_config.dailyBaseSalary, FinanceType.StaffSalary, "Ա������");
    }
    #endregion

    #region ��Ҳ���
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

    #region ��������
    private int CalculateCustomerCapacity()
    {
        // ��ȡ��������
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
        // ��ȡ��������
        float kitchenCapacity = buildingManager.GetCurrentValue(BuildingType.KitchenCapacity);
        float dailySeconds = 50400f;
        return Mathf.FloorToInt(kitchenCapacity * (dailySeconds / _config.dishPrepTime));
    }
    #endregion

    #region ����ϵͳ
    public int GetUpgradeCost(int currentLevel)
    {
        return Mathf.RoundToInt(_config.upgradeCostCurve.Evaluate(currentLevel));
    }

    public bool TryUpgradeFacility(int currentLevel)
    {
        int cost = GetUpgradeCost(currentLevel);
        return SpendGold(cost, FinanceType.FacilityUpgrade, $"��ʩ������Lv{currentLevel + 1}");
    }
    #endregion

    #region ���ݹ���
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

    #region ���Է���
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
        GUILayout.Label($"��ǰ���: {system.CurrentGold}");

        if (GUILayout.Button("ģ�ⵥ�վ�Ӫ"))
        {
            system.ProcessDailyOperation();
        }

        if (GUILayout.Button("��ʾ�������񱨸�"))
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

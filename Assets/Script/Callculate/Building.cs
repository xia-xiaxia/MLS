// ��������
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    RestaurantCapacity,  // ��������
    KitchenCapacity,     // ��� ��ʦ����
    ServiceStaffCapacity,// �д�����
}

// ���ཨ����������
[System.Serializable]
public class BuildingUpgradeConfig
{
    public BuildingType type;
    public int maxLevel = 10;            //?���ȼ�
    public int baseCost = 100;           // ���������ɱ�
    public float costMultiplier = 1.5f;  // ÿ���ɱ�����ϵ��
    public float baseValue = 1;          // ����Ч��ֵ
    public float valueMultiplier = 1.2f; // ÿ��Ч������ϵ��
}

// ����ȫ������
[CreateAssetMenu(menuName = "Building/BuildingConfig")]
public class BuildingGlobalConfig : ScriptableObject
{
    public List<BuildingUpgradeConfig> configs = new();
}

public class BuildingUpgradeManager : MonoBehaviour
{
    [SerializeField] private BuildingGlobalConfig config;
    [SerializeField] private EconomySystem economySystem;
    // ��ǰ�ȼ���¼
    private Dictionary<BuildingType, int> currentLevels = new();
    private Dictionary<BuildingType, float> currentValues = new();

    void Awake()
    {
        Initialize();
        LoadProgress();
    }

    // ��ʼ��Ĭ��ֵ
    private void Initialize()
    {
        foreach (BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
        {
            currentLevels[type] = 1;
            currentValues[type] = GetBaseValue(type);
        }
    }

    // ��ȡ����ֵ
    private float GetBaseValue(BuildingType type)
    {
        var cfg = config.configs.Find(c => c.type == type);
        return cfg?.baseValue ?? 1f;
    }

    // �ж��Ƿ��������
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
            $"{type}�������ȼ�{currentLevels[type] + 1}"
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

    // ���㵱ǰ�ȼ���Ч��ֵ
    private float CalculateCurrentValue(BuildingType type)
    {
        var cfg = config.configs.Find(c => c.type == type);
        return cfg.baseValue * Mathf.Pow(cfg.valueMultiplier, currentLevels[type] - 1);
    }

    // ��ȡ�����ɱ�
    public int GetUpgradeCost(BuildingType type)
    {
        var cfg = config.configs.Find(c => c.type == type);
        return Mathf.RoundToInt(cfg.baseCost * Mathf.Pow(cfg.costMultiplier, currentLevels[type] - 1));
    }

    // ��ȡ��ǰЧ��ֵ
    public float GetCurrentValue(BuildingType type) => currentValues[type];

    // ��ȡ���ȼ�
    public int GetMaxLevel(BuildingType type)
    {
        var cfg = config.configs.Find(c => c.type == type);
        return cfg?.maxLevel ?? 1;
    }

    #region ���ݳ־û�
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
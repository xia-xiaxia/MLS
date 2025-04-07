using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 岗位类型枚举
public enum WorkPosition
{
    None,
    Manager,
    Chef,
    Server
}

// 效果类型枚举
public enum EffectType
{
    DishIncome,     // 菜肴收益
    CustomerFlow,   // 客流量
    MoveSpeed,      // 移动速度
    CleanSpeed,     // 清理速度
    CookSpeed       // 制作速度
}

// 伙伴基础配置（ScriptableObject）
[CreateAssetMenu(fileName = "NewPartnerConfig", menuName = "Partner System/Partner Config")]
public class PartnerConfig : ScriptableObject
{
    [Header("基础设置")]
    public string partnerID;
    public string displayName;
    public Sprite icon;

    [Header("星级设置")]
    [Tooltip("各星级升级所需碎片 [0->1, 1->2,...4->5]")]
    public int[] starUpgradeCosts = new int[] { 100, 150, 200, 400, 600 };

    [Header("基础效果")]
    public EffectType baseEffectType;
    [Range(0, 1)] public float baseEffectValue = 0.05f;
    [Range(0, 0.1f)] public float starEffectIncrement = 0.01f;

    [Header("等级效果")]
    public EffectType levelEffectType;
    [Range(0, 0.1f)] public float levelEffectPerLevel = 0.01f;

    [Header("岗位效果")]
    [Range(0, 0.2f)] public float managerEffect = 0.1f;
    [Range(0, 0.2f)] public float chefEffect = 0.05f;
    [Range(0, 0.2f)] public float waiterEffect = 0.05f;

    [Header("稀有度加成")]
    public EffectType rarityEffectType;
    [Range(0, 0.1f)] public float rarityEffectPerRarity = 0.01f; 

    [Header("升级消耗曲线")]
    public AnimationCurve levelCostCurve = AnimationCurve.Linear(1, 10, 50, 500);
}

// 伙伴实例数据
[System.Serializable]
public class PartnerInstance
{
    public string partnerID;
    public int starLevel = 0;
    public int currentLevel = 1;
    public WorkPosition assignedPosition = WorkPosition.None;
    public bool isUnlocked = false;
}

// 资源服务接口
public interface IResourceService
{
    bool SpendGold(int amount);
    bool SpendFragments(string partnerID, int amount);
    int GetFragments(string partnerID);
}

// 核心管理系统
public class PartnerSystem : MonoBehaviour
{
    // 配置数据
    [SerializeField] private List<PartnerConfig> allPartnerConfigs;
    [Header("岗位容量")]
    [SerializeField] private int maxManagers = 1;
    [SerializeField] private int maxChefs = 2;
    [SerializeField] private int maxWaiters = 3;

    // 新增接口
    // 分配岗位（返回是否成功）
    public bool AssignPosition(string partnerID, WorkPosition position)
    {
        PartnerInstance partner = GetPartner(partnerID);
        if (partner == null || !CanAssignPosition(position)) return false;

        // 移除旧岗位
        if (partner.assignedPosition != WorkPosition.None)
        {
            // 通知旧岗位效果移除（需其他系统实现）
            RestaurantSystem.Instance.RemoveEffects(partnerID);
        }

        partner.assignedPosition = position;

        // 通知新岗位效果应用（需其他系统实现）
        RestaurantSystem.Instance.ApplyEffects(partnerID, GetEffects(partnerID));
        return true;
    }

    // 检查岗位容量
    public bool CanAssignPosition(WorkPosition position)
    {
        int current = partners.Count(p =>
            p.isUnlocked &&
            p.assignedPosition == position
        );

        return position switch
        {
            WorkPosition.Manager => current < maxManagers,
            WorkPosition.Chef => current < maxChefs,
            WorkPosition.Server => current < maxWaiters,
            _ => false
        };
    }


    // 运行时数据
    private List<PartnerInstance> partners = new List<PartnerInstance>();
    private IResourceService resourceService;

    //  初始化 
    public void Initialize(IResourceService resourceService)
    {
        this.resourceService = resourceService;
        LoadDefaultPartners();
    }

    private void LoadDefaultPartners()
    {
        foreach (var config in allPartnerConfigs)
        {
            partners.Add(new PartnerInstance
            {
                partnerID = config.partnerID,
                isUnlocked = false
            });
        }
    }

    // 接口 

    // 解锁伙伴
    public bool UnlockPartner(string partnerID)
    {
        PartnerInstance partner = GetPartner(partnerID);
        PartnerConfig config = GetConfig(partnerID);

        if (partner == null || config == null || partner.isUnlocked)
            return false;

        int unlockCost = 50; // 初始解锁成本
        if (resourceService.SpendFragments(partnerID, unlockCost))
        {
            partner.isUnlocked = true;
            return true;
        }
        return false;
    }

    // 提升星级
    public bool UpgradeStar(string partnerID)
    {
        PartnerInstance partner = GetPartner(partnerID);
        PartnerConfig config = GetConfig(partnerID);

        if (!ValidateUpgrade(partner, config)) return false;

        int starIndex = partner.starLevel;
        if (starIndex >= config.starUpgradeCosts.Length) return false;

        int cost = config.starUpgradeCosts[starIndex];
        if (resourceService.SpendFragments(partnerID, cost))
        {
            partner.starLevel++;
            return true;
        }
        return false;
    }

    // 提升等级
    public bool UpgradeLevel(string partnerID)
    {
        PartnerInstance partner = GetPartner(partnerID);
        PartnerConfig config = GetConfig(partnerID);

        if (!ValidateUpgrade(partner, config)) return false;

        int cost = Mathf.RoundToInt(config.levelCostCurve.Evaluate(partner.currentLevel));
        if (resourceService.SpendGold(cost))
        {
            partner.currentLevel = Mathf.Min(partner.currentLevel + 1, 50);
            return true;
        }
        return false;
    }

    // 获取总效果值
    public Dictionary<EffectType, float> GetEffects(string partnerID)
    {
        var effects = new Dictionary<EffectType, float>();
        PartnerInstance partner = GetPartner(partnerID);
        PartnerConfig config = GetConfig(partnerID);

        if (partner == null || config == null || !partner.isUnlocked)
            return effects;

        // 基础效果
        float baseEffect = config.baseEffectValue + (config.starEffectIncrement * partner.starLevel);
        effects.Add(config.baseEffectType, baseEffect);

        // 等级效果（仅在分配岗位时生效）
        if (partner.assignedPosition != WorkPosition.None)
        {
            float levelEffect = partner.currentLevel * config.levelEffectPerLevel;
            effects.Add(config.levelEffectType, levelEffect);
        }

        //稀有度效果()
        if (partner.assignedPosition!=WorkPosition.None)
        {
            float rarityEffect = partner.starLevel * config.rarityEffectPerRarity;
            effects.Add(config.rarityEffectType, rarityEffect);
        }

        // 岗位效果
        switch (partner.assignedPosition)
        {
            case WorkPosition.Manager:
                effects.Add(EffectType.DishIncome, config.managerEffect);
                break;
            case WorkPosition.Chef:
                effects.Add(EffectType.CookSpeed, config.chefEffect);
                break;
            case WorkPosition.Server:
                effects.Add(EffectType.CustomerFlow, config.waiterEffect);
                break;
        }

        return effects;
    }

  
    private PartnerInstance GetPartner(string partnerID)
    {
        return partners.Find(p => p.partnerID == partnerID);
    }

    private PartnerConfig GetConfig(string partnerID)
    {
        return allPartnerConfigs.Find(c => c.partnerID == partnerID);
    }

    private bool ValidateUpgrade(PartnerInstance partner, PartnerConfig config)
    {
        return partner != null &&
               config != null &&
               partner.isUnlocked;
    }
}
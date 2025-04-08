using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// ��λ����ö��
public enum WorkPosition
{
    None,
    Manager,
    Chef,
    Server
}

// Ч������ö��
public enum EffectType
{
    DishIncome,     // ��������
    CustomerFlow,   // ������
    MoveSpeed,      // �ƶ��ٶ�
    CleanSpeed,     // �����ٶ�
    CookSpeed       // �����ٶ�
}

// ���������ã�ScriptableObject��
[CreateAssetMenu(fileName = "NewPartnerConfig", menuName = "Partner System/Partner Config")]
public class PartnerConfig : ScriptableObject
{
    [Header("��������")]
    public string partnerID;
    public string displayName;
    public Sprite icon;

    [Header("�Ǽ�����")]
    [Tooltip("���Ǽ�����������Ƭ [0->1, 1->2,...4->5]")]
    public int[] starUpgradeCosts = new int[] { 100, 150, 200, 400, 600 };

    [Header("����Ч��")]
    public EffectType baseEffectType;
    [Range(0, 1)] public float baseEffectValue = 0.05f;
    [Range(0, 0.1f)] public float starEffectIncrement = 0.01f;

    [Header("�ȼ�Ч��")]
    public EffectType levelEffectType;
    [Range(0, 0.1f)] public float levelEffectPerLevel = 0.01f;

    [Header("��λЧ��")]
    [Range(0, 0.2f)] public float managerEffect = 0.1f;
    [Range(0, 0.2f)] public float chefEffect = 0.05f;
    [Range(0, 0.2f)] public float waiterEffect = 0.05f;

    [Header("ϡ�жȼӳ�")]
    public EffectType rarityEffectType;
    [Range(0, 0.1f)] public float rarityEffectPerRarity = 0.01f; 

    [Header("������������")]
    public AnimationCurve levelCostCurve = AnimationCurve.Linear(1, 10, 50, 500);
}

// ���ʵ������
[System.Serializable]
public class PartnerInstance
{
    public string partnerID;
    public int starLevel = 0;
    public int currentLevel = 1;
    public WorkPosition assignedPosition = WorkPosition.None;
    public bool isUnlocked = false;
}

// ��Դ����ӿ�
public interface IResourceService
{
    bool SpendGold(int amount);
    bool SpendFragments(string partnerID, int amount);
    int GetFragments(string partnerID);
}

// ���Ĺ���ϵͳ
public class PartnerSystem : MonoBehaviour
{
    // ��������
    [SerializeField] private List<PartnerConfig> allPartnerConfigs;
    [Header("��λ����")]
    [SerializeField] private int maxManagers = 1;
    [SerializeField] private int maxChefs = 2;
    [SerializeField] private int maxWaiters = 3;

    // �����ӿ�
    // �����λ�������Ƿ�ɹ���
    public bool AssignPosition(string partnerID, WorkPosition position)
    {
        PartnerInstance partner = GetPartner(partnerID);
        if (partner == null || !CanAssignPosition(position)) return false;

        // �Ƴ��ɸ�λ
        if (partner.assignedPosition != WorkPosition.None)
        {
            // ֪ͨ�ɸ�λЧ���Ƴ���������ϵͳʵ�֣�
            RestaurantSystem.Instance.RemoveEffects(partnerID);
        }

        partner.assignedPosition = position;

        // ֪ͨ�¸�λЧ��Ӧ�ã�������ϵͳʵ�֣�
        RestaurantSystem.Instance.ApplyEffects(partnerID, GetEffects(partnerID));
        return true;
    }

    // ����λ����
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


    // ����ʱ����
    private List<PartnerInstance> partners = new List<PartnerInstance>();
    private IResourceService resourceService;

    //  ��ʼ�� 
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

    // �ӿ� 

    // �������
    public bool UnlockPartner(string partnerID)
    {
        PartnerInstance partner = GetPartner(partnerID);
        PartnerConfig config = GetConfig(partnerID);

        if (partner == null || config == null || partner.isUnlocked)
            return false;

        int unlockCost = 50; // ��ʼ�����ɱ�
        if (resourceService.SpendFragments(partnerID, unlockCost))
        {
            partner.isUnlocked = true;
            return true;
        }
        return false;
    }

    // �����Ǽ�
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

    // �����ȼ�
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

    // ��ȡ��Ч��ֵ
    public Dictionary<EffectType, float> GetEffects(string partnerID)
    {
        var effects = new Dictionary<EffectType, float>();
        PartnerInstance partner = GetPartner(partnerID);
        PartnerConfig config = GetConfig(partnerID);

        if (partner == null || config == null || !partner.isUnlocked)
            return effects;

        // ����Ч��
        float baseEffect = config.baseEffectValue + (config.starEffectIncrement * partner.starLevel);
        effects.Add(config.baseEffectType, baseEffect);

        // �ȼ�Ч�������ڷ����λʱ��Ч��
        if (partner.assignedPosition != WorkPosition.None)
        {
            float levelEffect = partner.currentLevel * config.levelEffectPerLevel;
            effects.Add(config.levelEffectType, levelEffect);
        }

        //ϡ�ж�Ч��()
        if (partner.assignedPosition!=WorkPosition.None)
        {
            float rarityEffect = partner.starLevel * config.rarityEffectPerRarity;
            effects.Add(config.rarityEffectType, rarityEffect);
        }

        // ��λЧ��
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
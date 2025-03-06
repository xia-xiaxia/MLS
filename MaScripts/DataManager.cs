using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // �߻���һ����ֵ
    [Header("�߻�����")]
    [SerializeField] private TimeConfig _timeConfig;
    [SerializeField] private EconomyConfig _economyConfig;
    [SerializeField] private List<ChefData> _chefConfigs;
    [SerializeField] private List<DishData> _dishConfigs;

    // ����ʱ����
    public GameTime CurrentTime 
    { 
        get; 
        private set; 
    }
    public EconomySystem Economy 
    {
        get; 
        private set; 
    }

    protected override void Awake()
    {
        base.Awake(); // ������û���Awake
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        // ��ʼ����ϵͳ
        CurrentTime = new GameTime(_timeConfig);
        Economy = new EconomySystem(_economyConfig);

        // ����У��
        ValidateConfigs();
    }

    private void ValidateConfigs()
    {
        // �Զ����ؼ�����ȱʧ
        if (_timeConfig == null)
            Debug.LogError("û������ʱ�䣡����DataManager��Inspector����");

        if (_chefConfigs.Count == 0)
            Debug.LogWarning("��ʦ�����б�Ϊ�գ����ܵ�����Ϸ�쳣");
    }

    public bool TryGetChefData(string chefID, out ChefData data)
    {
        foreach (var chef in _chefConfigs)
        {
            if (chef.id == chefID)
            {
                data = chef;
                return true;
            }
        }
        data = null;
        Debug.LogError($"�Ҳ�����ʦ����: {chefID}");
        return false;
    }

    
}
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // 策划配一下数值
    [Header("策划配置")]
    [SerializeField] private TimeConfig _timeConfig;
    [SerializeField] private EconomyConfig _economyConfig;
    [SerializeField] private List<ChefData> _chefConfigs;
    [SerializeField] private List<DishData> _dishConfigs;

    // 运行时数据
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
        base.Awake(); // 必须调用基类Awake
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        // 初始化子系统
        CurrentTime = new GameTime(_timeConfig);
        Economy = new EconomySystem(_economyConfig);

        // 数据校验
        ValidateConfigs();
    }

    private void ValidateConfigs()
    {
        // 自动检测关键配置缺失
        if (_timeConfig == null)
            Debug.LogError("没有配置时间！请检查DataManager的Inspector设置");

        if (_chefConfigs.Count == 0)
            Debug.LogWarning("厨师配置列表为空，可能导致游戏异常");
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
        Debug.LogError($"找不到厨师配置: {chefID}");
        return false;
    }

    
}
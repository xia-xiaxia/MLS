using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using XFABManager;
using XFGameFramework.MissionSystem;

public class TestLoad : MonoBehaviour
{
    public Inventory menu;
    private MissionsConfig configs;
    private List<MissionBase> missions;
    public TextMeshProUGUI taskName;
    public TextMeshProUGUI description;
    public TextMeshProUGUI progress;

    void Start()
    {
        // 加载配表
        configs = AssetBundleManager.LoadAsset<MissionsConfig>("TestMoudle", "New Missions Config");
        // 添加任务系统
        MissionManager.AddConfig(configs);

        // 查询到所有任务对象
        missions = MissionManager.GetAllMissions("New Missions Config");
        // 修改任务状态
        foreach (MissionBase mission in missions)
        {
            // 设置为进行中的状态
            mission.SetStateWithoutNotify(MissionState.InProgress);
        }
    }

    public void Update()
    {
        if(menu != null)
        {
            MissionManager.SetInt("New Missions Config", "get_dishes_count", getRecipeCount());
        }
        OnUI();
    }

    public int getRecipeCount()
    {
        return menu.items.Count;
    }

    public void OnUI()
    {

        foreach (MissionBase mission in missions)
        {
            if(mission.State == MissionState.InProgress)
            {
                taskName.text = mission.MissionConfig.mission_name;
                description.text = mission.MissionConfig.description;
                progress.text = mission.Progress.ToString();
                break;
            }
        }
        
        
    }
}




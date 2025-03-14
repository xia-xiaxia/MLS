using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFGameFramework.MissionSystem;

public class TestLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 加载配表
        MissionsConfig configs = AssetBundleManager.LoadAsset<MissionsConfig>("TestMoudle", "New Missions Config");
        // 添加任务系统
        MissionManager.AddConfig(configs);

        // 查询到所有任务对象
        List<MissionBase> missions = MissionManager.GetAllMissions("New Missions Config");
        // 修改任务状态
        foreach (MissionBase mission in missions)
        {
            // 设置为进行中的状态
            mission.SetStateWithoutNotify(MissionState.InProgress);
        }
    }

}


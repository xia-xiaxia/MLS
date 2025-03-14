using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFGameFramework.MissionSystem;

public class TestLoad : MonoBehaviour
{
    public Inventory menu;

    void Start()
    {
        // �������
        MissionsConfig configs = AssetBundleManager.LoadAsset<MissionsConfig>("TestMoudle", "New Missions Config");
        // �������ϵͳ
        MissionManager.AddConfig(configs);

        // ��ѯ�������������
        List<MissionBase> missions = MissionManager.GetAllMissions("New Missions Config");
        // �޸�����״̬
        foreach (MissionBase mission in missions)
        {
            // ����Ϊ�����е�״̬
            mission.SetStateWithoutNotify(MissionState.InProgress);
        }
    }

    public void Update()
    {
        if(menu != null)
        {
            MissionManager.SetInt("New Missions Config", "get_dishes_count", getRecipeCount());
        }
    }

    public int getRecipeCount()
    {
        return menu.items.Count;
    }

    
    
}


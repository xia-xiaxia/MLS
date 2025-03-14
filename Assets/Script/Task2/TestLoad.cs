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

}


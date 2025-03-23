using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
//using UnityEngine.UIElements;
//using XFABManager;
//using XFGameFramework.MissionSystem;

public class TestLoad : MonoBehaviour
{
    public Inventory menu;
    //private MissionsConfig configs;
    //private List<MissionBase> missions;
    public TextMeshProUGUI taskName;
    public TextMeshProUGUI description;
    public TextMeshProUGUI progress;

    void Start()
    {
        /*/ 
        configs = AssetBundleManager.LoadAsset<MissionsConfig>("TestMoudle", "New Missions Config");
        // 
        MissionManager.AddConfig(configs);

        // 
        missions = MissionManager.GetAllMissions("New Missions Config");
        //״̬
        foreach (MissionBase mission in missions)
        {
            // 
            mission.SetStateWithoutNotify(MissionState.InProgress);
        }
    }

    public void Update()
    {
        if (menu != null)
        {
            MissionManager.SetInt("New Missions Config", "get_dishes_count", getRecipeCount());
        }
        OnUI();
    }

    public int getRecipeCount()
    {
        return menu.Recipes.Count;
    }

    public void OnUI()
    {

        foreach (MissionBase mission in missions)
        {
            if (mission.State == MissionState.InProgress)
            {
                taskName.text = mission.MissionConfig.mission_name;
                description.text = mission.MissionConfig.description;
                progress.text = mission.Progress.ToString();
                break;
            }
        }
        */

    }
}



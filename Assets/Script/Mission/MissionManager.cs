using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; set; }

    //public event Action<string> OnUpdateBroadcast;
    //private Dictionary<string, object> monitoredProperties;
    private Dictionary<string, Mission> missions = new Dictionary<string, Mission>
    {
        {"guestCount", new Mission("ÕÐ´ý 10 Î»¹Ë¿Í.", 10, null) }
    };
    private static List<string> names = new List<string> { "guestCount" };
    private Queue<string> missionQueue = new Queue<string>(names);
    private Mission curMission = null;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(transform.parent);
    }
    private void Start()
    {
        UpdateMission();
    }
    public void UpdateMission()
    {
        if (curMission != null)
        {
            curMission.OnAccomplishMission();
        }
        if (missionQueue.Count > 0)
        {
            curMission = missions[missionQueue.Dequeue()];
            curMission.OnStartMission();
        }
        else
        {
            curMission = null;
        }
    }
    public void RegisterMission(string monitoredValueName, object value)
    {
        if (missions.ContainsKey(monitoredValueName))
        {
            missions[monitoredValueName].value = value;
        }
        else
        {
            Debug.LogWarning(monitoredValueName + " Is Not Existent in the Missions");
        }
    }
    public void UpdateValue(string monitoredValueName, object value)
    {
        if (missions.ContainsKey(monitoredValueName))
        {
            missions[monitoredValueName].OnUpdate(value);
        }
        else
        {
            Debug.LogWarning(monitoredValueName + " Is Not Existent in the Missions");
        }
    }
}
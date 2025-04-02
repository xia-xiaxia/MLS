using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Mission
{
    public string description;
    public object value;
    public int progress;
    public int maxProgress;

    public Mission(string description, int maxProgress, object value)
    {
        this.description = description;
        this.progress = 0;
        this.maxProgress = maxProgress;
        this.value = value;
    }
    public void OnStartMission()
    {
        MissionUIManager.Instance.UpdateMissionView(this);
    }
    public void OnAccomplishMission()
    {
    }
    public void OnUpdate(object value)
    {
        this.value = value;
        progress = (int)value;
        MissionUIManager.Instance.UpdateProgressBar(progress, maxProgress);
    }
}

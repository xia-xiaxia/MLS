using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework.MissionSystem;

public class TestMission : MissionBase
{ 
    protected override void OnProgressChange()
    {
        base.OnProgressChange();
        Debug.LogFormat("任务:{0} 进度:{1}", MissionConfig.id, Progress);
    }

    protected override void OnStateChange(MissionState state)
    {
        base.OnStateChange(state);
        Debug.LogFormat("任务:{0} 状态:{1}", MissionConfig.id, State);
    }


}

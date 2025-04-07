using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskData_SO", menuName = "Data/ TaskData")]
public class TaskData_SO : ScriptableObject
{
    public List<TaskDetails> TaskDetailsList;
}

[System.Serializable]
public class TaskDetails
{
    [Header("任务数据 ID从1开始")]
    [Header("ID and Icon")]
    public int taskID;//任务ID
    public string TaskName;//任务名称
    public TaskType taskType;//任务类型
    [TextArea]
    public string taskDescription;//任务描述

    [Header("Task Targets")]
    [Header("菜肴数及种类 ")]
    public List<Dishes> dishes;//菜肴数及种类

    [System.Serializable]
    public class Dishes//菜肴 包括ID 名称 价格
    {
        public int dishID;
        public string dishName;
        public int dishCost;
    }

    [System.Serializable]
    public enum TaskType//任务类型
    {
        Amount,
        time,
    }

    [Header("Task Rewards")]
    public int rewardAmount;//奖励金额
    [Header("state of the task")]
    public bool taskOnGoing;//任务进行中
    [Header("is compulsory or not")]
    public bool isCompulsoryTask;//是否强制任务

}
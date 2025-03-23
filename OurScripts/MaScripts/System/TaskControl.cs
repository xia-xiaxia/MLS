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
    [Header("�������� ID��1��ʼ")]
    [Header("ID and Icon")]
    public int taskID;//����ID
    public string TaskName;//��������
    public TaskType taskType;//��������
    [TextArea]
    public string taskDescription;//��������

    [Header("Task Targets")]
    [Header("������������ ")]
    public List<Dishes> dishes;//������������

    [System.Serializable]
    public class Dishes//���� ����ID ���� �۸�
    {
        public int dishID;
        public string dishName;
        public int dishCost;
    }

    [System.Serializable]
    public enum TaskType//��������
    {
        Amount,
        time,
    }

    [Header("Task Rewards")]
    public int rewardAmount;//�������
    [Header("state of the task")]
    public bool taskOnGoing;//���������
    [Header("is compulsory or not")]
    public bool isCompulsoryTask;//�Ƿ�ǿ������

}
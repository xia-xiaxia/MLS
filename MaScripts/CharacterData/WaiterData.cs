using UnityEngine;

[CreateAssetMenu(menuName = "Data/Waiter")]
public class WaiterData : ScriptableObject
{
    public string id;                    // ΨһID
    [Tooltip("��/����")]
    public float walkingSpeed = 5f;      // ��·�ٶ�
    public int maxCleanSpeed = 3;  // ���ͬʱ���������
}

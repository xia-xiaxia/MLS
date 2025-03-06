using UnityEngine;

[CreateAssetMenu(menuName = "Data/Chef")]
public class ChefData : ScriptableObject
{
    public string id;                    // ΨһID
    [Tooltip("��/����")]
    public float cookingSpeed = 5f;      // �����ٶ�
    public int maxConcurrentOrders = 3;  // ���ͬʱ��������
}
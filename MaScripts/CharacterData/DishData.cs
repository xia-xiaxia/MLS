using UnityEngine;

[CreateAssetMenu(menuName = "Data/Dish")]
public class DishData : ScriptableObject
{
    public string dishID;         // ��ƷΨһID
    public string dishName;       // ��Ʒ����
    public float baseCost;        // �����ɹ��ɱ�
    public float sellPrice;       // ���ۼ۸�
    [Range(0, 100)]
    public int quality;           // Ʒ�ʵȼ�
}
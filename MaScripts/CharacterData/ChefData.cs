using UnityEngine;

[CreateAssetMenu(menuName = "Data/Chef")]
public class ChefData : ScriptableObject
{
    public string id;                    // 唯一ID
    [Tooltip("秒/道菜")]
    public float cookingSpeed = 5f;      // 做菜速度
    public int maxConcurrentOrders = 3;  // 最大同时处理订单数
}
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Waiter")]
public class WaiterData : ScriptableObject
{
    public string id;                    // 唯一ID
    [Tooltip("秒/道菜")]
    public float walkingSpeed = 5f;      // 走路速度
    public int maxCleanSpeed = 3;  // 最大同时清理餐桌数
}

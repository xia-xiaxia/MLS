using UnityEngine;

[CreateAssetMenu(menuName = "Data/Dish")]
public class DishData : ScriptableObject
{
    public string dishID;         // 菜品唯一ID
    public string dishName;       // 菜品名字
    public float baseCost;        // 基础采购成本
    public float sellPrice;       // 销售价格
    [Range(0, 100)]
    public int quality;           // 品质等级
}
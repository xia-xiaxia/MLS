// 餐厅系统
using System.Collections.Generic;
using UnityEngine;

public class RestaurantSystem : MonoBehaviour
{
    public static RestaurantSystem Instance;

    // 当前生效的效果集合
    private Dictionary<EffectType, float> activeEffects = new Dictionary<EffectType, float>();

    void Awake() => Instance = this;

    // 应用效果
    public void ApplyEffects(string partnerID, Dictionary<EffectType, float> effects)
    {
        foreach (var effect in effects)
        {
            // 叠加效果（示例为加法叠加）
            if (activeEffects.ContainsKey(effect.Key))
                activeEffects[effect.Key] += effect.Value;
            else
                activeEffects.Add(effect.Key, effect.Value);

            UpdateGameSystems(effect.Key);
        }
    }

    // 移除效果
    public void RemoveEffects(string partnerID)
    {
        // 需要记录每个伙伴的效果以便精准移除
        // 此处简化实现，实际需要更复杂的数据结构
    }

    // 更新具体游戏系统
    private void UpdateGameSystems(EffectType type)
    {
        switch (type)
        {
            case EffectType.DishIncome:
                EconomySystem.Instance.dishIncomeMultiplier = activeEffects[type];
                break;
            case EffectType.CookSpeed:
                KitchenSystem.Instance.cookSpeedMultiplier = activeEffects[type];
                break;
                
        }
    }
}

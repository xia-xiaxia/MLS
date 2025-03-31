// ����ϵͳ
using System.Collections.Generic;
using UnityEngine;

public class RestaurantSystem : MonoBehaviour
{
    public static RestaurantSystem Instance;

    // ��ǰ��Ч��Ч������
    private Dictionary<EffectType, float> activeEffects = new Dictionary<EffectType, float>();

    void Awake() => Instance = this;

    // Ӧ��Ч��
    public void ApplyEffects(string partnerID, Dictionary<EffectType, float> effects)
    {
        foreach (var effect in effects)
        {
            // ����Ч����ʾ��Ϊ�ӷ����ӣ�
            if (activeEffects.ContainsKey(effect.Key))
                activeEffects[effect.Key] += effect.Value;
            else
                activeEffects.Add(effect.Key, effect.Value);

            UpdateGameSystems(effect.Key);
        }
    }

    // �Ƴ�Ч��
    public void RemoveEffects(string partnerID)
    {
        // ��Ҫ��¼ÿ������Ч���Ա㾫׼�Ƴ�
        // �˴���ʵ�֣�ʵ����Ҫ�����ӵ����ݽṹ
    }

    // ���¾�����Ϸϵͳ
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

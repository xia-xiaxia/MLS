/*using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    //用动画表示营业前和歇业后吧，这是从8点到22点的14个营业小时
    public float realTime_GameTime = 105f; // 现实1秒 = 游戏105秒
    public float gameTime = 0;
    public bool isOpen = true;
    public event Action OnDayEnd;

    void Update()//开张
    {
        if (isOpen)
        {
            gameTime += Time.deltaTime * realTime_GameTime;
            if (gameTime >= 50400)
            { 
                // 22:00 结算
                isOpen = false;
                OnDayEnd?.Invoke();
                gameTime = 0;//开始新的一天
            }
        }
    }
}
*/
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
// 物品稀有度
public enum Rarity
{
    white,   //白，实质上是没有获得
    Green,  // 绿
    Blue,   // 蓝
    Purple, // 紫
    Gold,   // 金
    Rainbow // 彩
}

// 物品类别
public enum ItemType
{
    Recipe, // 配方
    Ingredient, // 食材
    Partner, // 伙伴
    //PartnerFragments//伙伴碎片
}

// 物品数据
public class ShoppingItem
{
    public readonly string itemID;    // 物品唯一标识
    public readonly ItemType itemType;
    public readonly Rarity rarity;
    public readonly int conversionValue;

    public ShoppingItem(ItemType type, Rarity rarity, int value, string id)
    {
        itemType = type;
        this.rarity = rarity;
        conversionValue = value;
        itemID = id;
    }
}



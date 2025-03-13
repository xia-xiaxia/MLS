using System;
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




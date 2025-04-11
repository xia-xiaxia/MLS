using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseData : MonoBehaviour
{

    public TextMeshProUGUI timeText;  // 用于显示时间的 TextMeshProUGUI 组件
    public TextMeshProUGUI earningsText;

    private float realTimeElapsed = 0f;  // 现实中流逝的时间（秒）
    private float startTimeInScene = 8f * 60f;  // 场景中开始的时间，8:00 AM，单位为分钟
    private float endTimeInScene = 22f * 60f;  // 场景中结束的时间，10:00 PM，单位为分钟
    private float totalDayDuration = 20f * 60f;  // 场景中的 1 天总时间，单位为分钟 (20分钟现实时间 = 1天)

    public float earnings;
    public float nowTime;
    public bool isPaused = false;

    void Start()
    {
        earnings = 0;
    }

    void Update()
    {
        //AmountTransfer();
        TimePass();

    }

    public void AmountTransfer()
    {
        earnings += 0.01f;
        earningsText.text = "Earnings: " + earnings.ToString("F2");
    }

    public void TimePass()
    {

        if (isPaused)
            return;  // 如果暂停，就跳过时间更新

        // 每一秒现实世界的时间
        realTimeElapsed += Time.deltaTime;

        // 计算场景中的时间，使用现实世界的时间与时间比例关系来计算
        float sceneTimeInMinutes = startTimeInScene + (realTimeElapsed / 1f);  // 每分钟现实时间 = 场景中的 1 小时

        // 确保场景时间在 8:00 到 22:00 之间
        if (sceneTimeInMinutes >= endTimeInScene)
        {
            // 如果到达 22:00，重置回 08:00，实现循环
            realTimeElapsed = 0f;  // 重置现实时间
            earnings -= 100;
            sceneTimeInMinutes = startTimeInScene;
        }

        // 计算小时和分钟
        int hours = Mathf.FloorToInt(sceneTimeInMinutes / 60f);
        int minutes = Mathf.FloorToInt(sceneTimeInMinutes % 60f);

        // 格式化时间字符串，确保时间格式为 "00:00"
        timeText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);
    }

    // 暂停时间
    public void PauseTime()
    {
        isPaused = true;
    }

    // 恢复时间
    public void ResumeTime()
    {
        isPaused = false;
    }
}

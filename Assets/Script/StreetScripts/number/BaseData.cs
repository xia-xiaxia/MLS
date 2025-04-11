using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseData : MonoBehaviour
{

    public TextMeshProUGUI timeText;  // ������ʾʱ��� TextMeshProUGUI ���
    public TextMeshProUGUI earningsText;

    private float realTimeElapsed = 0f;  // ��ʵ�����ŵ�ʱ�䣨�룩
    private float startTimeInScene = 8f * 60f;  // �����п�ʼ��ʱ�䣬8:00 AM����λΪ����
    private float endTimeInScene = 22f * 60f;  // �����н�����ʱ�䣬10:00 PM����λΪ����
    private float totalDayDuration = 20f * 60f;  // �����е� 1 ����ʱ�䣬��λΪ���� (20������ʵʱ�� = 1��)

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
            return;  // �����ͣ��������ʱ�����

        // ÿһ����ʵ�����ʱ��
        realTimeElapsed += Time.deltaTime;

        // ���㳡���е�ʱ�䣬ʹ����ʵ�����ʱ����ʱ�������ϵ������
        float sceneTimeInMinutes = startTimeInScene + (realTimeElapsed / 1f);  // ÿ������ʵʱ�� = �����е� 1 Сʱ

        // ȷ������ʱ���� 8:00 �� 22:00 ֮��
        if (sceneTimeInMinutes >= endTimeInScene)
        {
            // ������� 22:00�����û� 08:00��ʵ��ѭ��
            realTimeElapsed = 0f;  // ������ʵʱ��
            earnings -= 100;
            sceneTimeInMinutes = startTimeInScene;
        }

        // ����Сʱ�ͷ���
        int hours = Mathf.FloorToInt(sceneTimeInMinutes / 60f);
        int minutes = Mathf.FloorToInt(sceneTimeInMinutes % 60f);

        // ��ʽ��ʱ���ַ�����ȷ��ʱ���ʽΪ "00:00"
        timeText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);
    }

    // ��ͣʱ��
    public void PauseTime()
    {
        isPaused = true;
    }

    // �ָ�ʱ��
    public void ResumeTime()
    {
        isPaused = false;
    }
}

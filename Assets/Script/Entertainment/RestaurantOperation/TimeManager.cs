using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private enum TimeState
    {
        Run,
        Stop,
        Close
    }
    private TimeState curState = TimeState.Close;
    private float timer = 0f;
    private float timeRatio = 2f;
    private List<int> operationSpan = new List<int> { 18, 21, 22 };
    private int hour;
    private int minute;



    private void Update()
    {
        if (curState == TimeState.Close)
            return;
        timer += Time.deltaTime;
        if (timer >= timeRatio)
        {
            timer = 0f;
            minute += 1;
        }
        if (minute >= 60)
        {
            minute = 0;
            hour += 1;
        }
        TimeUIManager.Instance.minute.text = minute.ToString("D2");
        TimeUIManager.Instance.hour.text = hour.ToString("D2");
        if (curState == TimeState.Run)
        {
            if (hour >= operationSpan[1])
            {
                StopNewGuest();
            }
        }
        else if (curState == TimeState.Stop)
        {
            if (GuestManager.Instance.CheckGuestsCount() <= 0 || hour >= operationSpan[2])
            {
                ForceGuestsAway();
            }
        }
    }
    public void StartOperation()
    {
        curState = TimeState.Run;
        timer = 0;
        hour = operationSpan[0];
        minute = 0;
        TimeUIManager.Instance.OnShowTime();
    }
    public void StopNewGuest()
    {
        curState = TimeState.Stop;
        RestaurantOperationManager.Instance.StopReceivingGuests();
    }
    public void ForceGuestsAway()
    {
        curState = TimeState.Close;
        TimeUIManager.Instance.OnHideTime();
        RestaurantEconomyManager.Instance.SettleAccounts();
        RestaurantOperationManager.Instance.ForceGuestsAway();
        RestaurantOperationManager.Instance.EndOperation();
    }
}

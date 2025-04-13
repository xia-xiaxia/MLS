using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantOperationManager : Singleton<RestaurantOperationManager>
{
    public GameObject startButton;



    public void BeginOperation()
    {
        if (GuestManager.Instance.menu.recipes.Count <= 0)
        {
            WarningUIManager.Instance.ShowWarning();
        }
        else
        {
            startButton.SetActive(false);
            TimeManager.Instance.StartOperation();
            ServerManager.Instance.OnBeginOperation();
            BeginReceivingGuests();
        }
    }
    public void EndOperation()
    {
        ServerManager.Instance.OnEndOperation();
        startButton.SetActive(true);
    }
    public void BeginReceivingGuests()
    {
            GuestManager.Instance.OnBeginReceivingGuests();
    }
    public void StopReceivingGuests()
    {
        GuestManager.Instance.OnStopReceivingGuests();
    }
    public void ForceGuestsAway()
    {
        GuestManager.Instance.ForceGuestsAway();
    }
}

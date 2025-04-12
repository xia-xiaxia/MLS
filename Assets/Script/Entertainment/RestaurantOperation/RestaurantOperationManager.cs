using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantOperationManager : Singleton<RestaurantOperationManager>
{
    //private void Start()
    //{
    //    BeginOperation();
    //    BeginReceivingGuests();
    //}
    public void BeginOperation()
    {
        if (GuestManager.Instance.menu.recipes.Count <= 0)
        {
            WarningUIManager.Instance.ShowWarning();
        }
        else
            ServerManager.Instance.OnBeginOperation();
    }
    public void EndOperation()
    {
        ServerManager.Instance.OnEndOperation();
    }
    public void BeginReceivingGuests()
    {
        if (GuestManager.Instance.menu.recipes.Count <= 0)
        {
            WarningUIManager.Instance.ShowWarning();
        }
        else
            GuestManager.Instance.OnBeginReceivingGuests();
    }
    public void EndReceivingGuests()
    {
        GuestManager.Instance.OnEndReceivingGuests();
    }
}

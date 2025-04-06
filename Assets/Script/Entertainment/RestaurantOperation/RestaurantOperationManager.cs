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
        ServerManager.Instance.OnBeginOperation();
    }
    public void EndOperation()
    {
        ServerManager.Instance.OnEndOperation();
    }
    public void BeginReceivingGuests()
    {
        GuestManager.Instance.OnBeginReceivingGuests();
    }
    public void EndReceivingGuests()
    {
        GuestManager.Instance.OnEndReceivingGuests();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeUIManager : Singleton<TimeUIManager>
{
    public GameObject timeUI;
    public TextMeshProUGUI hour;
    public TextMeshProUGUI minute;



    public void OnShowTime()
    {
        timeUI.SetActive(true);
    }
    public void OnHideTime()
    {
        timeUI.SetActive(false);
    }
}

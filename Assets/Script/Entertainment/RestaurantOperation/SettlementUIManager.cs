using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettlementUIManager : Singleton<SettlementUIManager>
{
    public GameObject ui;
    public TextMeshProUGUI earnings;

    private bool isShowing = false;



    private void Update()
    {
        if (isShowing)
        {
            if (Input.anyKeyDown)
            {
                ui.SetActive(false);
                isShowing = false;
            }
        }
    }
    public void OnShowEarnings(int revenue)
    {
        ui.SetActive(true);
        earnings.text = "гд" + revenue;
    }
}

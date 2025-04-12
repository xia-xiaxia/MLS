using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class WarningUIManager : Singleton<WarningUIManager>
{
    public CanvasGroup canvasGroup;

    private bool isWarning = false;
    private float timer = 0;
    private float warningDuration_1 = 0.5f;
    private float warningDuration_2 = 2.5f;



    private void Update()
    {
        if (isWarning)
        {
            timer += Time.deltaTime;
            if (timer > warningDuration_1)
            {
                if (timer > warningDuration_2)
                {
                    HideWarning();
                }
                else
                {
                    canvasGroup.alpha = Mathf.Lerp(1, 0, (timer - warningDuration_1) / (warningDuration_2 - warningDuration_1));
                }
            }
        }
    }
    public void ShowWarning()
    {
        isWarning = true;
        timer = 0;
        canvasGroup.alpha = 1;
    }
    private void HideWarning()
    {
        isWarning = false;
        timer = 0;
        canvasGroup.alpha = 0;
    }
}

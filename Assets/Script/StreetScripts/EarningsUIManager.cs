using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EarningsUIManager : Singleton<EarningsUIManager>
{
    public TextMeshProUGUI earnings;



    public void UpdateEarnings(int earnings)
    {
        this.earnings.text = earnings.ToString();
    }
}

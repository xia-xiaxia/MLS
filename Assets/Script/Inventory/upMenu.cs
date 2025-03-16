using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upMenu : MonoBehaviour
{
    public GridControl ggg;

    public void OnClicked()
    {
        if (ggg.index > 0)
        {
            ggg.Grids[ggg.index].SetActive(false);
            ggg.index--;
        }

    }
}

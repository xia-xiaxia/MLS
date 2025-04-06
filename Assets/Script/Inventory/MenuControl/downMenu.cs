using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class downMenu : MonoBehaviour
{
    public GridControl ggg;

    public void OnClicked()
    {
        if (ggg.index < ggg.Grids.Count - 1)
        {
            ggg.Grids[ggg.index].SetActive(false);
            ggg.index++;
        }

    }
}

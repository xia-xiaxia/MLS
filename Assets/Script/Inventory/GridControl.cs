using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridControl : MonoBehaviour
{
    public Transform slotParent;
    public GameObject currentGrid;
    public GameObject GridPrefab;
    public List<GameObject> Grids;
    public InventoryManager inventoryManager;
    public int index;
    private bool started = false;

    public void clickedStart()
    {
        currentGrid = GameObject.FindWithTag("Grid");
        slotParent = currentGrid.transform.parent;
        Grids.Add(currentGrid);
        inventoryManager.slotGrid = currentGrid;
        index = 0;
        foreach(GameObject grid in Grids)
        {
            grid.SetActive(false);
        }
        started = true;
    }

    void Update()
    {
        if (started)
        {
            inventoryManager.slotGrid = currentGrid;
            if (Grids.Count >= 1 && currentGrid.transform.childCount >= 6)
            {
                dependCount();
            }
            Grids[index].SetActive(true);
        }
    }

    public void dependCount()
    {
        int count = currentGrid.transform.childCount;
        if(count >= 6)
        {
            currentGrid = Instantiate(GridPrefab, slotParent);
            currentGrid.transform.SetParent(slotParent);
            currentGrid.transform.position = Grids[0].transform.position;
            Grids.Add(currentGrid);
            currentGrid.SetActive(false);
            count = 0;
            return;
        }
    }
}

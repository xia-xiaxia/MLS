using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridControl : MonoBehaviour
{
    public Transform slotParent;
    public GameObject currentGrid;
    public GameObject GridPrefab;
    public List<GameObject> Grids;

    public InventoryManager inventoryManager; // inventoryManager
    public int index;
    private bool started = false;

    public void clickedStart()
    {
        currentGrid = GameObject.FindWithTag("Grid");
        slotParent = currentGrid.transform.parent;
        Grids.Add(currentGrid);
        InventoryManager.instance.slotGrid = currentGrid;
        index = 0;
        foreach (GameObject grid in Grids)
        {
            grid.SetActive(false);
        }
        started = true;
        RefreshItem();
    }

    void Update()
    {
        if (started)
        {
            InventoryManager.instance.slotGrid = currentGrid;
            if (Grids.Count >= 1 && currentGrid.transform.childCount > 6)
            {
                dependCount();
            }
            Grids[index].SetActive(true);
        }
    }

    public void dependCount()
    {
        while (currentGrid.transform.childCount > 6)
        {
            // 创建新的 Grid
            if (Grids.Count <= index + 1)
            {
                GameObject newGrid = Instantiate(GridPrefab, slotParent);
                newGrid.transform.SetParent(slotParent);
                newGrid.transform.position = Grids[0].transform.position;
                Grids.Add(newGrid);
                newGrid.SetActive(false);
            }

            // 获取下一个 Grid
            GameObject nextGrid = Grids[index + 1];

            // 移动子物体到下一个 Grid
            for (int i = 6; i < currentGrid.transform.childCount; i++)
            {
                Transform child = currentGrid.transform.GetChild(i);
                child.SetParent(nextGrid.transform);
            }

            // 更新 currentGrid 和 index
            currentGrid = nextGrid;
            index++;
        }
    }

    public void RefreshItem()
    {
        if (inventoryManager == null || inventoryManager.inventory == null)
        {
            Debug.LogError("inventory is not assigned.");
            return;
        }

        // 清空所有 Grid 的子物体
        foreach (GameObject grid in Grids)
        {
            foreach (Transform child in grid.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // 从 bag 中获取子物体并分配到 Grid
        int itemCount = 0;
        foreach (Ingredient ingredient in inventoryManager.inventory.ingredients)
        {
            if (itemCount % 6 == 0)
            {
                if (Grids.Count <= itemCount / 6)
                {
                    GameObject newGrid = Instantiate(GridPrefab, slotParent);
                    newGrid.transform.SetParent(slotParent);
                    newGrid.transform.position = Grids[0].transform.position;
                    Grids.Add(newGrid);
                    newGrid.SetActive(false);
                }
                currentGrid = Grids[itemCount / 6];
            }

            GameObject newSlot = Instantiate(inventoryManager.slotPrefab.gameObject, currentGrid.transform);
            ingredientSlot slot = newSlot.GetComponent<ingredientSlot>();
            slot.slotIngredient = ingredient;
            slot.slotName.text = ingredient.IngredientName + "  LV:" + ingredient.IngredientLevel.ToString();
            slot.slotIcon.sprite = ingredient.IngredientImage;

            itemCount++;
        }
    }
}


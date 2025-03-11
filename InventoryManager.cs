using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    static InventoryManager instance;
    public GameObject slotGrid;
    public slot slotPrefab;
    public TextMeshProUGUI scipeDescription;
    public Inventory menu;
    public Inventory inventory;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // 确保 inventory 和 menu 已初始化
        if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned in the inspector.");
        }

        if (menu == null)
        {
            Debug.LogError("Menu is not assigned in the inspector.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            checkScipeInMenu();
        }
    }

    public static void CreatNewSlot(Scipe scipe)
    {
        slot newSlot = Instantiate(instance.slotPrefab, instance.slotGrid.transform);
        newSlot.gameObject.transform.SetParent(instance.slotGrid.transform);
        newSlot.slotScipe = scipe;
        newSlot.slotDescription.text = scipe.ScipeDescription + "  价格为： " + scipe.ScipePrice + "  成本为： " + scipe.ScripeCost;
        newSlot.slotName.text = scipe.ScipeName;
        newSlot.slotIcon.sprite = scipe.ScipeIcon;
    }

    public void checkScipeInMenu()
    {
        if (inventory == null || menu == null)
        {
            Debug.LogError("Inventory or Menu is not assigned.");
            return;
        }

        foreach (Scipe scipe in inventory.items)
        {
            if (!menu.items.Contains(scipe))
            {
                CreatNewSlot(scipe);
                menu.items.Add(scipe); // 确保不会重复添加
            }
        }
    }
}

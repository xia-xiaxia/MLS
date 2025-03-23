using Spine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class BagControl : MonoBehaviour
{
    static BagControl instance;

    public GameObject slotGrid;
    public slot slotPrefab;
    public TextMeshProUGUI scipeDescription;
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


    public void OnEnable()
    {
        if (scipeDescription != null)
            scipeDescription.text = "00";
        RefreshItem();
    }

    public static void UpdateItemInfo(string information)
    {
        if (instance != null && instance.scipeDescription != null)
        {
            instance.scipeDescription.text = information;
        }
    }

    public static void CreatNewSlot(Ingredient ingredient)
    {
        if (instance != null && instance.slotGrid != null && instance.slotPrefab != null)
        {
            ingredientSlot newSlot = Instantiate(instance.slotPrefab, instance.slotGrid.transform) as ingredientSlot;
            newSlot.gameObject.transform.SetParent(instance.slotGrid.transform);
            newSlot.slotIngredient = ingredient;
            newSlot.slotName.text = ingredient.IngredientName + "  LV:" + ingredient.CurrentLevel.ToString();
            newSlot.slotIcon.sprite = ingredient.IngredientImage;
        }
    }

    public static void RefreshItem()
    {
        if (instance == null || instance.slotGrid == null || instance.inventory == null)
        {
            Debug.LogError("InventoryManager instance, slotGrid, or inventory is not assigned.");
            return;
        }

        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            if (instance.slotGrid.transform.GetChild(i).gameObject != null)
                Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < instance.inventory.ingredients.Count; i++)
        {
            CreatNewSlot(instance.inventory.ingredients[i]);
        }
    }
}
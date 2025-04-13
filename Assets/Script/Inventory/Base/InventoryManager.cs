using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

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

    public static void CreatNewSlot(Recipe recipe)
    {
        if (instance != null && instance.slotGrid != null && instance.slotPrefab != null)
        {
            RecipeSlot newSlot = Instantiate(instance.slotPrefab, instance.slotGrid.transform) as RecipeSlot;
            newSlot.gameObject.transform.SetParent(instance.slotGrid.transform);
            newSlot.slotRecipe = recipe;
            newSlot.slotName.text = recipe.RecipeName + "  Rarity:" + recipe.Rarity.ToString();
            newSlot.slotIcon = newSlot.GetComponent<Image>();
            newSlot.slotIcon.sprite = recipe.RecipeImage;
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
        for (int i = 0; i < instance.inventory.Recipes.Count; i++)
        {
            CreatNewSlot(instance.inventory.Recipes[i]);
        }
    }
}

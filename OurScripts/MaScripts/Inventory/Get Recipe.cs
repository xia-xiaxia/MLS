using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRecipe : MonoBehaviour
{
    public Recipe recipe;
    public Inventory theInventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // ���ո������
        {
            AddNewRecipe();
            Destroy(gameObject);
        }
    }


    public void AddNewRecipe()
    {
        if (!theInventory.items.Contains(recipe))
        {
            theInventory.items.Add(recipe);
            InventoryManager.CreatNewSlot(recipe);
            Debug.Log("Recipe added to inventory: " + recipe.RecipeName);
        }
        else
        {
            Debug.Log("Recipe already in inventory");
        }

        InventoryManager.RefreshItem();
    }
}
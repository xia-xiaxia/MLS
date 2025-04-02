using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItems : MonoBehaviour
{
    public Recipe recipe;
    public Inventory theInventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // ¼ì²â¿Õ¸ñ¼ü°´ÏÂ
        {
            AddNewRecipe();
            Destroy(gameObject);
        }
    }
    public void AddNewRecipe()
    {
        if (!theInventory.Recipes.Contains(recipe))
        {
            theInventory.Recipes.Add(recipe);
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

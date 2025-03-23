using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class composeManager : MonoBehaviour
{
    public Recipe recipe;
    public Inventory ingredientBag;
    public List<TextMeshProUGUI> ingredientsTexts;
    public GameObject ingredientGrid;
    private int ingreCountInBag = 0;
    private bool visiable;

    public void FixedUpdate()
    {
        if (visiable)
            Visualisation();
    }

    public void Visualisation()
    {
        if (recipe != null)
        {
            Debug.Log(recipe.RecipeName);
            int length = recipe.ingredients.Count;
            for (int i = 0; i < length; i++)
            {
                ingredientsTexts[i].text = recipe.ingredients[i].IngredientName;
                ingredientsTexts[i].color = Color.white;
            }

            for (int i = 0; i < recipe.ingredients.Count; i++)
            {
                if (ingredientBag.ingredients.Contains(recipe.ingredients[i]))
                {
                    ingredientsTexts[i].color = Color.green;
                    ingreCountInBag++;
                }
            }

        }
        else
        {
            Debug.Log("No recipe is selected.");
        }
    }

    public bool CheckEnable()
    {
        ingreCountInBag = 0;
        Debug.Log("CheckEnable");
        if (recipe != null)
        {
            Debug.Log(recipe.RecipeName);
            for (int i = 0; i < recipe.ingredients.Count; i++)
            {
                if (ingredientBag.ingredients.Contains(recipe.ingredients[i]))
                {
                    ingreCountInBag++;
                }
            }
            if (ingreCountInBag == recipe.ingredients.Count)
            {
                Debug.Log("All ingredients are in bag. This dish has been made!");
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.Log("No recipe is selected.");
            return false;
        }
    }

    public void RecipeOnClicked()
    {
        ingredientGrid.SetActive(true);
        ingreCountInBag = 0;
        visiable = true;
    }

    
}

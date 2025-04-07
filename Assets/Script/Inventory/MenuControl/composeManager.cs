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
    public bool isEnable = false;


    public void RecipeOnClicked()
    {
        ingredientGrid.SetActive(true);
        int ingreCountInBag = 0;
        

        if (recipe != null)
        {
            int length = recipe.ingredients.Count;
            for (int i = 0; i < length; i++)
            {
                ingredientsTexts[i].text = recipe.ingredients[i].IngredientName;
                ingredientsTexts[i].color = Color.white;
            }

            for (int i = 0; i < recipe.ingredients.Count; i++)
            {
                if (ingredientBag.ingredients.Contains(recipe.ingredients[i].ingredients))//µã³öingredients
                {
                    ingredientsTexts[i].color = Color.green;
                    ingreCountInBag++;
                }
            }

            if (ingreCountInBag == recipe.ingredients.Count)
            {
                isEnable = true;
            }
        }
        else
        {
            Debug.Log("No recipe is selected.");
        }


    }
}

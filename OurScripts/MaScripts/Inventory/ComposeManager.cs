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
            int length = recipe.requirements.Count;
            for (int i = 0; i < length; i++)
            {
                ingredientsTexts[i].text = recipe.requirements[i].ingredient.displayName;
                ingredientsTexts[i].color = Color.white;
            }
        }

        for (int i = 0; i < recipe.requirements.Count; i++)
        {
            if (ingredientBag.ingredients.Contains(recipe.requirements[i].ingredients))
            {
                ingredientsTexts[i].color = Color.green;
                ingreCountInBag++;
            }
        }

        if (ingreCountInBag == recipe.requirements.Count)
        {
            isEnable = true;
        }
    }
}

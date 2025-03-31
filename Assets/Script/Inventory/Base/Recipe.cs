using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe")]
public class Recipe : ScriptableObject
{
    [Header("Recipe Info")]
    public string RecipeName;
    public Sprite RecipeImage;
    [TextArea]
    public string RecipeDescription;
    public int RecipeID;
    public int RecipeLevel;
    public float RecipePrice;
    public float RecipeCost;
    public string color;

    [Header("Recipe Ingredients")]
    public List<Ingredient> ingredients;

}

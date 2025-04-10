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
    public int RecipeLevelPriceAddition; // ϡ�жȼ۸�����
    public int RecipeBasePrice; // �����۸�
    public int RecipePrice;
    public int RecipeCost;
    public Rarity Rarity;

    [Header("Recipe Ingredients")]
    public List<Ingredient> ingredients;

    public bool isEnable;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Inventory/Ingredient")]
public class Ingredient : ScriptableObject
{
    [Header("Ingredient Info")]
    public string IngredientName;
    public Sprite IngredientImage;
    [TextArea]
    public string IngredientDescription;
    public int IngredientID;
    public int IngredientLevel;
    public float IngredientPrice;
    public Rarity Rarity;

    public bool isEnable;

}

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
    public int IngredientLevelPriceAddition; // 稀有度采购费增长
    public int IngredientLevelProfitAddition; // 稀有度菜品价格增长
    public int IngredientBasePrice; // 基础价格
    public int IngredientPrice;
    public Rarity Rarity;

    public bool isEnable;

}

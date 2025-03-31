using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public List<Recipe> Recipes = new List<Recipe>();
    public List<Ingredient> ingredients = new List<Ingredient>();
}
  

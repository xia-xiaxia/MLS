using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public List<Ingredient> ingredients = new List<Ingredient>(); 
    public List<Recipe> items = new List<Recipe>();
}

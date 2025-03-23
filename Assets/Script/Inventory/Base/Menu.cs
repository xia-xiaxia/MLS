using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Menu")]
public class Menu : ScriptableObject
{
    public List<Recipe> recipes = new List<Recipe>();
}

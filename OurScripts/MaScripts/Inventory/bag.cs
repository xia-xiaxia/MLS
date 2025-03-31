using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bag", menuName = "Bag/Bag")]
public class bag : ScriptableObject
{
    public List<ingredientSlot> ingredients = new List<ingredientSlot>();

}


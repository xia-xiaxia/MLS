using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Bag/Item")]
public class Item : ScriptableObject
{
    [Header("ŒÔ∆∑")]
    public string itemName;
    public Sprite itemIcon;
    [TextArea]
    public string itemDescription;

    public int itemID;
    public int itemLevel;
    public float itemPrice;
    public float itemCost;

}

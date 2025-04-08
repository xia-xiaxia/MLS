using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderDishManager : Singleton<OrderDishManager>
{
    public CardImageDatabase cardImageDatabase;

    private Dictionary<string, Sprite> dishes = new Dictionary<string, Sprite>();



    private void Start()
    {
        foreach(var dish in cardImageDatabase.imageEntries)
        {
            if (!dishes.ContainsKey(dish.cardName))
                dishes.Add(dish.cardName, dish.image);
            else
                Debug.LogError(Equals($"Duplicate dish entry: {dish.cardName}"));
        }
    }
}

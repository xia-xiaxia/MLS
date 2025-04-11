using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderDishManager : Singleton<OrderDishManager> //作用待定，也许可以直接用CardImageDatabase的字典
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
    public Sprite GetDishSprite(string dishName)
    {
        Debug.Log(dishName);
        if (dishes.ContainsKey(dishName))
            return dishes[dishName];
        else
            return null;
    }
}

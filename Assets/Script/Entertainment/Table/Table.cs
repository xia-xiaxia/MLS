using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class Table : MonoBehaviour
{
    public enum TableSize
    {
        S,
        M,
        L
    }
    public TableSize tableSize;
    public int tableIndex;
    [HideInInspector]
    public List<Seat> seats = new List<Seat>();
    public Dictionary<int, List<SpriteRenderer>> dishSites = new Dictionary<int, List<SpriteRenderer>>();



    private void Start()
    {
        tableIndex = TableManager.Instance.AddTable(this);
        seats = transform.Find("SeatSites").GetComponentsInChildren<Seat>().ToList();

        for (int i = 0; i < transform.Find("DishSites").childCount; i++)
        {
            List<SpriteRenderer> list = new List<SpriteRenderer>();
            Transform child = transform.Find("DishSites").GetChild(i);
            int count = int.Parse(child.name.Substring(5, 2));
            for (int j = 0; j < child.childCount; j++)
                list.Add(child.GetChild(j).GetComponent<SpriteRenderer>());
            dishSites.Add(count, list);
        }
    }
    public void ServeDish(int dishCount, int dishIndex, string dishName)
    {
        Debug.LogWarning($"Dictionary[dishCount] = {dishSites[dishCount].Count}");
        Debug.Log("ServeDish: count " + dishCount + " index " + dishIndex + " name " + dishName);
        dishSites[dishCount][dishIndex].sprite = OrderDishManager.Instance.GetDishSprite(dishName);
    }
    public void ClearTable(int dishCount)
    {
        foreach (var dish in dishSites[dishCount])
        {
            dish.sprite = null;
        }
    }
}

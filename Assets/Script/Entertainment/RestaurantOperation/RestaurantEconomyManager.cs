using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantEconomyManager : Singleton<RestaurantEconomyManager>
{
    private int totalEarnings; // ��Ǯ�� ��20000
    private int TotalEarnings
    {
        get { return totalEarnings; }
        set 
        {
            totalEarnings = value;
            EarningsUIManager.Instance.UpdateEarnings(totalEarnings);
        }
    }

    // ����
    public Dictionary<string, Recipe> allRecipes;
    public Dictionary<string, Ingredient> allIngredients;

    private int revenuePerDay = 0; // ÿ�������



    private void Start()
    {
        TotalEarnings = 20000;
    }
    public void TransferDictionary(Dictionary<string, Recipe> recipes, Dictionary<string, Ingredient> ingredients)
    {
        allRecipes = recipes;
        allIngredients = ingredients;
    }
    public void AddRevenue(string dishName)
    {
        revenuePerDay += allRecipes[dishName].RecipePrice - allRecipes[dishName].RecipeCost;
    }
    public void UseOrGainMoney(int money)
    {
        TotalEarnings += money;
    }
    public void SettleAccounts() // һ��Ľ���
    {
        TotalEarnings += revenuePerDay;
        revenuePerDay = 0;
    }
}
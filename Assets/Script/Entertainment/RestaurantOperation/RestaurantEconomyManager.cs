using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantEconomyManager : Singleton<RestaurantEconomyManager>
{
    private int totalEarnings; // 总钱数 起步20000
    private int TotalEarnings
    {
        get { return totalEarnings; }
        set 
        {
            totalEarnings = value;
            EarningsUIManager.Instance.UpdateEarnings(totalEarnings);
        }
    }

    // 数据
    public Dictionary<string, Recipe> allRecipes;
    public Dictionary<string, Ingredient> allIngredients;

    private int revenuePerDay = 0; // 每天的收入



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
    public void SettleAccounts() // 一天的结算
    {
        TotalEarnings += revenuePerDay;
        revenuePerDay = 0;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    Green, Blue, Purple, Gold, Rainbow
}

public class GetItems : MonoBehaviour
{
    public static GetItems instance; 
    public Dictionary<string, Rarity> itemsRarity;
    public Inventory bag;
    public Inventory inventory;
    public List<Recipe> recipes = new List<Recipe>();
    public List<Ingredient> ingredients = new List<Ingredient>();


    public Dictionary<string, Recipe> recipeCard = new Dictionary<string, Recipe>();
    public Dictionary<string, Ingredient> ingredientCard = new Dictionary<string, Ingredient>();

    void Strat()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        instance = this;

        InitializeDictionaries();
    }
    void InitializeDictionaries()
    {
        // 初始化 recipeCard 字典
        foreach (var recipe in recipes)
        {
            if (!recipeCard.ContainsKey(recipe.RecipeName))
            {
                recipeCard.Add(recipe.RecipeName, recipe);
            }
        }

        // 初始化 ingredientCard 字典
        foreach (var ingredient in ingredients)
        {
            if (!ingredientCard.ContainsKey(ingredient.IngredientName))
            {
                ingredientCard.Add(ingredient.IngredientName, ingredient);
            }
        }

    }
    public void RefreshItems()
    {
        foreach (var kvp in itemsRarity)
        {
            string name = kvp.Key;
            Rarity rarity = kvp.Value;

            if(recipeCard.ContainsKey(name))
            {
                Recipe recipe = recipeCard[name];
                recipe.color = rarity.ToString();
                inventory.Recipes.Add(recipe);
            }

            if(ingredientCard.ContainsKey(name))
            {
                Ingredient ingredient = ingredientCard[name];
                ingredient.color = rarity.ToString();
                bag.ingredients.Add(ingredient);
            }
        }
    }
}



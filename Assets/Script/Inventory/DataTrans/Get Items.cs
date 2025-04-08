using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItems : MonoBehaviour
{
    public static GetItems instance;
    public List<Card> gettenItems;
    public Inventory bag;
    public Inventory inventory;
    public List<Recipe> recipes = new List<Recipe>();
    public List<Ingredient> ingredients = new List<Ingredient>();

    public Dictionary<string, Recipe> recipeCard = new Dictionary<string, Recipe>();
    public Dictionary<string, Ingredient> ingredientCard = new Dictionary<string, Ingredient>();

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitializeDictionaries();
    }

    void InitializeDictionaries()
    {
        if (recipes == null)
        {
            Debug.LogError("recipes list is null");
            return;
        }

        if (ingredients == null)
        {
            Debug.LogError("ingredients list is null");
            return;
        }

        // 初始化 recipeCard 字典
        foreach (var recipe in recipes)
        {
            if (recipe != null && !recipeCard.ContainsKey(recipe.RecipeName))
            {
                recipeCard.Add(recipe.RecipeName, recipe);
            }
        }

        // 初始化 ingredientCard 字典
        foreach (var ingredient in ingredients)
        {
            if (ingredient != null && !ingredientCard.ContainsKey(ingredient.IngredientName))
            {
                ingredientCard.Add(ingredient.IngredientName, ingredient);
            }
        }
    }

    public void RefreshItems()
    {
        foreach (var card in gettenItems)
        {
            string name = card.name;
            if (card.type == CardType.Recipe)
            {
                if (recipeCard.TryGetValue(card.name, out Recipe recipe))
                {
                    Debug.Log(recipe.RecipeName);
                    if (inventory.Recipes.Contains(recipe))
                    {
                        int index = inventory.Recipes.IndexOf(recipe);
                        if (card.rarity > inventory.Recipes[index].Rarity)
                        {
                            inventory.Recipes[index].Rarity = card.rarity;
                        }
                    }
                    else
                    {
                        recipe.Rarity = card.rarity;
                        inventory.Recipes.Add(recipe);
                    }
                }
                else
                {
                    Debug.LogWarning($"Recipe {card.name} not found in recipeCard dictionary");
                }
            }
            if (card.type == CardType.Ingredient)
            {
                if (ingredientCard.TryGetValue(card.name, out Ingredient ingredient))
                {
                    Debug.Log(ingredient.IngredientName);
                    if (bag.ingredients.Contains(ingredient))
                    {
                        int index = bag.ingredients.IndexOf(ingredient);
                        if (card.rarity > bag.ingredients[index].Rarity)
                        {
                            bag.ingredients[index].Rarity = card.rarity;
                        }

                    }
                    else
                    {
                        ingredient.Rarity = card.rarity;
                        bag.ingredients.Add(ingredient);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Ingredient {card.name} not found in ingredientCard dictionary");
            }
        }
    }
}

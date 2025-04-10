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

        // ��ʼ�� recipeCard �ֵ�
        foreach (var recipe in recipes)
        {
            if (recipe != null && !recipeCard.ContainsKey(recipe.RecipeName))
            {
                recipeCard.Add(recipe.RecipeName, recipe);
            }
        }

        // ��ʼ�� ingredientCard �ֵ�
        foreach (var ingredient in ingredients)
        {
            if (ingredient != null && !ingredientCard.ContainsKey(ingredient.IngredientName))
            {
                ingredientCard.Add(ingredient.IngredientName, ingredient);
            }
        }
        if (RestaurantEconomyManager.Instance != null)
        {
            RestaurantEconomyManager.Instance.TransferDictionary(recipeCard, ingredientCard);
        }
        StartCoroutine(RefreshPriceAndCost());
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
                            Debug.Log("recipes: " + index + "changed");
                        }
                    }
                    else
                    {
                        recipe.Rarity = card.rarity;
                        inventory.Recipes.Add(recipe);
                        Debug.Log("Recipe added: " + recipe.RecipeName);
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
                        Debug.Log("111111111111111111111111");
                        int index = bag.ingredients.IndexOf(ingredient);
                        if (card.rarity > bag.ingredients[index].Rarity)
                        {
                            bag.ingredients[index].Rarity = card.rarity;
                            Debug.Log("ingredients: " + index + "changed");
                        }
                        Debug.Log("111111111111111111111112222");

                    }
                    else
                    {
                        ingredient.Rarity = card.rarity;
                        bag.ingredients.Add(ingredient);
                        Debug.Log("Ingredient added: " + ingredient.IngredientName);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Ingredient {card.name} not found in ingredientCard dictionary");
            }
        }
        StartCoroutine(RefreshPriceAndCost());
    }
    public IEnumerator RefreshPriceAndCost() //Э�̷�ֹ����
    {
        // �ȸ���ʳ�ĵĲɹ���
        foreach (var i in ingredients)
        {
            i.IngredientPrice = i.IngredientBasePrice + i.IngredientLevel * (i.IngredientLevelPriceAddition - 1);
        }
        yield return null;
        // �ٸ��²�Ʒ�ĳɱ��ͼ۸�
        foreach (var j in recipes)
        {
            int cost = 0, price = 0;
            foreach (var k in j.ingredients)
            {
                cost += k.IngredientPrice;
                price += k.IngredientLevel * (k.IngredientLevelProfitAddition - 1);
            }
            j.RecipeCost = cost;
            j.RecipePrice = j.RecipeBasePrice + j.RecipeLevel * (j.RecipeLevelPriceAddition - 1) + price; // ��Ʒ�ļ۸���ڻ����۸� + ��Ʒ��ϡ�ж����� + ʳ�ĵ�ϡ�ж�����
            yield return null;
        }
    }
}

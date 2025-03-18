using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    public HashSet<string> fullPartners = new HashSet<string>(); // 存储已经抽到的伙伴list
    public int totalPartnerFragments = 0;  // 存储总伙伴碎片
    public Dictionary<string, Rarity> recipeMaxRarity = new Dictionary<string, Rarity>();  // 存储每个配方当前的最高稀有度
    public int totalRecipeExperience = 0;
    public int totalIngredientExperience = 0; // 存储配方碎片总经验和食材碎片总经验

    void Awake()
    {
       
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    public void UpdateRecipeMaxRarity(string recipeName, Rarity rarity)
    {
        if (!recipeMaxRarity.ContainsKey(recipeName))
        {
            recipeMaxRarity[recipeName] = rarity;
        }
        else if (rarity > recipeMaxRarity[recipeName])
        {
            recipeMaxRarity[recipeName] = rarity;
        }
    }
    public void AddIngredientExperience(int experience)
    {
        totalIngredientExperience += experience;
    }
    public void AddRecipeExperience(int experience)
    {
        totalRecipeExperience += experience;
    }
    public void AddPartnerFragments(int fragments)
    {
        totalPartnerFragments += fragments;
    }

    public void SaveFullPartner(string partnerName)
    {
        fullPartners.Add(partnerName);
    }
}

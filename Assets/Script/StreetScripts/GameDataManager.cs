using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    public HashSet<string> collectedCardIDs = new HashSet<string>();
    public HashSet<string> fullPartners = new HashSet<string>(); // 存储已经抽到的伙伴list
    public int totalPartnerFragments = 0;  // 存储总伙伴碎片
    public Dictionary<string, Rarity> recipeMaxRarity = new Dictionary<string, Rarity>();  // 存储每个配方当前的最高稀有度
    public int totalRecipeExperience = 0;
    public int totalIngredientExperience = 0; // 存储配方碎片总经验和食材碎片总经验
    public List<Card> allDrawnCards = new List<Card>();//抽到的卡。
    private string saveFilePath;

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

        saveFilePath = Path.Combine(Application.persistentDataPath, "game_data.json");
        LoadGameData();
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

    public void AddCollectedCard(string cardID)
    {
        collectedCardIDs.Add(cardID);
    }

    public bool IsCardCollected(string cardID)
    {
        return collectedCardIDs.Contains(cardID);
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

    
    public void SaveGameData()
    {
        GameData data = new GameData
        {
            collectedCardIDs = new List<string>(collectedCardIDs),
            fullPartners = new List<string>(fullPartners),
            totalPartnerFragments = totalPartnerFragments,
            recipeMaxRarity = new Dictionary<string, int>(), 
            totalRecipeExperience = totalRecipeExperience,
            totalIngredientExperience = totalIngredientExperience,
            allDrawnCards = new List<Card>(allDrawnCards)
        };

        foreach (var recipe in recipeMaxRarity)
        {
            data.recipeMaxRarity[recipe.Key] = (int)recipe.Value; 
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

//JSON
    public void LoadGameData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            collectedCardIDs = new HashSet<string>(data.collectedCardIDs);
            fullPartners = new HashSet<string>(data.fullPartners);
            totalPartnerFragments = data.totalPartnerFragments;
            totalRecipeExperience = data.totalRecipeExperience;
            totalIngredientExperience = data.totalIngredientExperience;
         
            recipeMaxRarity.Clear();
            foreach (var recipe in data.recipeMaxRarity)
            {
                recipeMaxRarity[recipe.Key] = (Rarity)recipe.Value;
            }
            if (data.allDrawnCards != null)
            {
                allDrawnCards = new List<Card>(data.allDrawnCards);//加载卡片列表
            }
        }
    }

    [Serializable]
    public class GameData
    {
        public List<string> collectedCardIDs = new List<string>();
        public List<string> fullPartners = new List<string>();
        public int totalPartnerFragments = 0;
        public Dictionary<string, int> recipeMaxRarity = new Dictionary<string, int>();
        public int totalRecipeExperience = 0;
        public int totalIngredientExperience = 0;
        public List<Card> allDrawnCards = new List<Card>();

    }
    public void ClearAllData()
    {
        collectedCardIDs.Clear();                
        fullPartners.Clear();                   
        totalPartnerFragments = 0;             
        recipeMaxRarity.Clear();               
        totalRecipeExperience = 0;            
        totalIngredientExperience = 0;   
        
        Debug.Log("所有存档数据已清除！");
    }
}

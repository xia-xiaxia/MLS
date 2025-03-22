using System.Collections.Generic;
using UnityEngine;

// ���ݴ洢ϵͳ
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    public HashSet<string> fullPartners = new HashSet<string>();
    public int totalPartnerFragments = 0;
    public Dictionary<string, Rarity> recipeMaxRarity = new Dictionary<string, Rarity>();
    public int totalRecipeExperience = 0;
    public int totalIngredientExperience = 0;
    public int totalGold; 
    public PlayerInventory playerInventory = new PlayerInventory();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateRecipeMaxRarity(string recipeName, Rarity rarity)
    {
        if (!recipeMaxRarity.ContainsKey(recipeName) || rarity > recipeMaxRarity[recipeName])
        {
            recipeMaxRarity[recipeName] = rarity;
        }
    }

    public void AddIngredientExperience(int experience) => totalIngredientExperience += experience;
    public void AddRecipeExperience(int experience) => totalRecipeExperience += experience;
    public void AddPartnerFragments(int fragments) => totalPartnerFragments += fragments;
    public void SaveFullPartner(string partnerName) => fullPartners.Add(partnerName);
    public int gameDays; // ��Ϸ����׷��

    public void SaveGame()
    {
        // ʵ�ִ浵�߼�
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(this));
    }
}



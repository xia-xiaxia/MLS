using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    // �洢�Ѿ��鵽�Ļ��list
    public HashSet<string> fullPartners = new HashSet<string>();

    // �洢�ܻ����Ƭ
    public int totalPartnerFragments = 0;

    // �洢ÿ���䷽��ǰ�����ϡ�ж�
    public Dictionary<string, Rarity> recipeMaxRarity = new Dictionary<string, Rarity>();

    // �洢�䷽��Ƭ�ܾ����ʳ����Ƭ�ܾ���
    public int totalRecipeExperience = 0;
    public int totalIngredientExperience = 0;

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

    // �����䷽��ǰ�����ϡ�ж�
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

  //ʳ�ľ���
    public void AddIngredientExperience(int experience)
    {
        totalIngredientExperience += experience;
    }

    //�䷽����
    public void AddRecipeExperience(int experience)
    {
        totalRecipeExperience += experience;
    }

    // �����Ƭ
    public void AddPartnerFragments(int fragments)
    {
        totalPartnerFragments += fragments;
    }

    // ���������Ļ�飨��
    public void SaveFullPartner(string partnerName)
    {
        fullPartners.Add(partnerName);
    }
}

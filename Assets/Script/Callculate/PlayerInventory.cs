using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
using static PlayerInventory;
[System.Serializable]
//���������Ʒ�嵥
public class PlayerInventory
{
    [System.Serializable]
    public class RecipeProgress
    {
        public Rarity highestRarity;
        public int currentLevel;
    }

    // �䷽���ݣ���¼�䷽���ϡ�жȣ�
    public Dictionary<int, RecipeProgress> recipes = new Dictionary<int, RecipeProgress>();

    // ʳ�����ݣ���¼���ϡ�жȺ���Ƭ��
    public Dictionary<string, IngredientData> ingredients = new Dictionary<string, IngredientData>();
    // �����Ƭ
    public Dictionary<string, int> partnerFragments = new Dictionary<string, int>();
    // �䷽�����
    public int recipeExpPool;
    //ʳ��
    [System.Serializable]
    public class IngredientProgress
    {
        //��ʼ��
        public int currentLevel = 1;
        public Rarity highestRarity = Rarity.white;
        public int ownedFragments = 0;
    }
    public class IngredientData
    {
        public Rarity highestRarity; // ��ǰ���ϡ�ж�
        public int fragments;        // ��ǰ������Ƭ
        public int currentLevel;     // ��ǰʹ�õȼ�
    }
    public int gold = 1000; // ��ʼ���

    public Dictionary<string, IngredientData> ingredient = new();
}

// ����ͬ������
public class InventoryBridge : MonoBehaviour
{
    [SerializeField] private Inventory _uiInventory; // ���ѵ�Inventory
    [SerializeField] private PlayerInventory _playerInventory;

    // ��������䷽ʱͬ��
    public void SyncNewRecipe(Recipe recipe)
    {
       /* 
        // ����UI���
        if (!_uiInventory.items.Contains(recipe))
        {
            _uiInventory.items.Add(recipe);
            InventoryManager.RefreshItem();
        }
       */

        // ������ֵ���
        if (!_playerInventory.recipes.ContainsKey(recipe.RecipeID))
        {
            _playerInventory.recipes.Add(recipe.RecipeID, new RecipeProgress
            {
                highestRarity = Rarity.white,
                currentLevel = 1
            });
        }
    }
}


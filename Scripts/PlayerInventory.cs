using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
using static PlayerInventory;
[System.Serializable]
//玩家已有物品清单
public class PlayerInventory
{
    [System.Serializable]
    public class RecipeProgress
    {
        public Rarity highestRarity;
        public int currentLevel;
    }

    // 配方数据（记录配方最高稀有度）
    public Dictionary<int, RecipeProgress> recipes = new Dictionary<int, RecipeProgress>();

    // 食材数据（记录最高稀有度和碎片）
    public Dictionary<string, IngredientData> ingredients = new Dictionary<string, IngredientData>();
    // 伙伴碎片
    public Dictionary<string, int> partnerFragments = new Dictionary<string, int>();
    // 配方经验池
    public int recipeExpPool;
    //食材
    [System.Serializable]
    public class IngredientProgress
    {
        //初始化
        public int currentLevel = 1;
        public Rarity highestRarity = Rarity.white;
        public int ownedFragments = 0;
    }
    public class IngredientData
    {
        public Rarity highestRarity; // 当前最高稀有度
        public int fragments;        // 当前持有碎片
        public int currentLevel;     // 当前使用等级
    }
    public int gold = 1000; // 初始金币

    public Dictionary<string, IngredientData> ingredient = new();
}

// 数据同步服务
public class InventoryBridge : MonoBehaviour
{
    [SerializeField] private Inventory _uiInventory; // 队友的Inventory
    [SerializeField] private PlayerInventory _playerInventory;

    // 当获得新配方时同步
    public void SyncNewRecipe(Recipe recipe)
    {
        // 更新UI库存
        if (!_uiInventory.items.Contains(recipe))
        {
            _uiInventory.items.Add(recipe);
            InventoryManager.RefreshItem();
        }

        // 更新数值库存
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


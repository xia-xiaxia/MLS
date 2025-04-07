using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.UI;

public class RecipeSlot : slot
{
    public GameObject infoWindow;
    public Recipe slotRecipe;
    public GameObject ingredientSlotPrefab; // 预制件，用于生成ingredientSlot物体
    private composeManager cM;


    public override void Start()
    {
        cM = GameObject.Find("BagManager").GetComponent<composeManager>();
        slotIcon = GetComponent<Image>();
        if (slotRecipe != null && !slotRecipe.isEnable)
        {
            Sprite recipeImage = slotRecipe.RecipeImage;
            slotIcon.sprite = recipeImage;
            Color color = slotIcon.color;
            color.a = 0.5f;
            slotIcon.color = color;
        }
    }

    public override void OnClicked()
    {
        Debug.Log("RecipeSlot clicked");
        if (slotRecipe != null)
            InventoryManager.UpdateItemInfo(slotRecipe.RecipeDescription);
        
        cM.recipe = this.slotRecipe;
        CreatMenu.instance.recipe = this.slotRecipe;

        if (cM.isEnable)
        {
            slotRecipe.isEnable = true;
            if (slotIcon != null)
            {
                Color color = slotIcon.color;
                color.a = 1f;
                slotIcon.color = color;
            }
        }



    }

    public override void infoOfRecipe()
    {
        infoWindow = GameObject.Find("infoWindow");
        if (slotRecipe != null && infoWindow != null && ingredientSlotPrefab != null)
        {
            foreach (Transform child in infoWindow.transform)
            {
                Destroy(child.gameObject);
            }

            // 生成并添加ingredientSlot物体
            foreach (var ingredient in slotRecipe.ingredients)
            {
                GameObject ingredientSlotObject = Instantiate(ingredientSlotPrefab, infoWindow.transform);
                // 假设ingredientSlotPrefab有一个TextMeshPro组件用于显示ingredient信息
                TextMeshProUGUI ingredientText = ingredientSlotObject.GetComponentInChildren<TextMeshProUGUI>();
                if (ingredientText != null)
                {
                    ingredientText.text = ingredient.IngredientName; 
                }
            }
        }
    }
}

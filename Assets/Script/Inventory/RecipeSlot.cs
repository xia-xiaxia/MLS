using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class RecipeSlot : slot
{
    public GameObject infoWindow;
    public Recipe slotRecipe;
    public GameObject ingredientSlotPrefab; // Ԥ�Ƽ�����������ingredientSlot����

    public override void Start()
    {
        if (slotRecipe != null)
        {
            slotIcon.sprite = slotRecipe.RecipeImage;
            Color color = slotIcon.color;
            color.a = 0.5f;
            slotIcon.color = color;
        }
    }

    public override void OnClicked()
    {
        if (slotRecipe != null)
            InventoryManager.UpdateItemInfo(slotRecipe.RecipeDescription);
        if (slotIcon != null)
        {
            Color color = slotIcon.color;
            color.a = 1f;
            slotIcon.color = color;
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

            // ���ɲ����ingredientSlot����
            foreach (var ingredient in slotRecipe.ingredients)
            {
                GameObject ingredientSlotObject = Instantiate(ingredientSlotPrefab, infoWindow.transform);
                // ����ingredientSlotPrefab��һ��TextMeshPro���������ʾingredient��Ϣ
                TextMeshProUGUI ingredientText = ingredientSlotObject.GetComponentInChildren<TextMeshProUGUI>();
                if (ingredientText != null)
                {
                    ingredientText.text = ingredient.IngredientName; 
                }
            }
        }
    }
}

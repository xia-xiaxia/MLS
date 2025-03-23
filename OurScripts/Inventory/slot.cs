using Spine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class slot : MonoBehaviour
{
    public Image slotIcon;
    public TextMeshProUGUI slotName;

    public virtual void Start()
    {

    }

    public virtual void OnClicked()
    {

    }

    public virtual void infoOfRecipe()
    {

    }

}


public class RecipeSlot : slot
{
    public GameObject infoWindow;
    public Recipe slotRecipe;
    public GameObject ingredientSlotPrefab; // 预制件，用于生成ingredientSlot物体
    private composeManager cM;

    public override void Start()
    {
        cM = GameObject.Find("BagManager").GetComponent<composeManager>();
        if (slotRecipe != null)
        {
            //slotIcon.sprite = slotRecipe.RecipeImage;
            Color color = slotIcon.color;
            color.a = 0.5f;
            slotIcon.color = color;
        }
    }

    public override void OnClicked()
    {
        if (slotRecipe != null)
            InventoryManager.UpdateItemInfo(slotRecipe.RecipeDescription);

        cM.recipe = this.slotRecipe;
        if (cM.isEnable)
        {
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
            foreach (var ingredient in slotRecipe.requirements)
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
public class ingredientSlot : slot
{
    public Ingredient slotIngredient;

    public override void Start()
    {
        if (slotIngredient != null)
        {
            slotIcon.sprite = slotIngredient.IngredientImage;
            Color color = slotIcon.color;
            color.a = 0.5f;
            slotIcon.color = color;
        }
    }

    public override void OnClicked()
    {
        if (slotIngredient != null)
            InventoryManager.UpdateItemInfo(slotIngredient.IngredientDescription);
        if (slotIcon != null)
        {
            Color color = slotIcon.color;
            color.a = 1f;
            slotIcon.color = color;
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ingredientSlot : slot
{
    public Ingredient slotIngredient;



    public override void Start()
    {
        slotIcon = GetComponent<Image>();
        if (slotIngredient != null && !slotIngredient.isEnable)
        {
            slotIcon.sprite = slotIngredient.IngredientImage;
            Color color = slotIcon.color;
            color.a = 0.5f;
            slotIcon.color = color;
        }
    }

    public override void OnClicked()
    {
        slotIngredient.isEnable = true;
        if (slotIngredient != null)
            BagControl.UpdateItemInfo(slotIngredient.IngredientDescription);
        if (slotIcon != null)
        {
            Color color = slotIcon.color;
            color.a = 1f;
            slotIcon.color = color;
        }
    }

}
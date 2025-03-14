using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class slot : MonoBehaviour
{
    public Recipe slotRecipe;
    public Image slotIcon;
    public TextMeshProUGUI slotName;

    void Start()
    {
        if (slotRecipe != null)
        {
            slotIcon.sprite = slotRecipe.RecipeImage;
            Color color = slotIcon.color;
            color.a = 0.5f;
            slotIcon.color = color;
        }
    }

    public void OnClicked()
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


}

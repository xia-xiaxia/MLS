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

    public void OnClicked()
    {
        if (slotRecipe != null)
            InventoryManager.UpdateItemInfo(slotRecipe.RecipeDescription);
    }
}

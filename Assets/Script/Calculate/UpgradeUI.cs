using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpgradeButtonController:MonoBehaviour	
{
     public Button RecipeUpgradeButton;
     public Button IngredientUpgradeButton;
     public Button PartnerUpgradeButton;

    private void Start()
    {
        RecipeUpgradeButton.onClick.AddListener(OnUpgradeClicked);
        UpdateButtonState();
    }
    private void OnUpgradeClicked()
    {

    }
    private void UpdateButtonState()
    {

    }
    
}

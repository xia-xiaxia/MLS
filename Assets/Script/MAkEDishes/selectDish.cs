using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeSelector : MonoBehaviour
{
    public Recipe recipe;
    public Button selectButton;
    public TextMeshProUGUI recipeNameText;

    private void Start()
    {
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectRecipe);
        }

        if (recipeNameText != null && recipe != null)
        {
            recipeNameText.text = recipe.RecipeName;
        }
    }

    private void OnSelectRecipe()
    {
        dishManager.Instance.SelectRecipe(recipe);
    }
}

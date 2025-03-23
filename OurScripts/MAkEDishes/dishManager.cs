using UnityEngine;
using UnityEngine.UI;

public class dishManager : MonoBehaviour
{
    public static dishManager Instance;

    public Button makeDishButton;
    public Transform spawnPoint;
    public GameObject recipePrefab;

    private Recipe selectedRecipe;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (makeDishButton != null)
        {
            makeDishButton.onClick.AddListener(OnMakeDish);
        }
    }

    public void SelectRecipe(Recipe recipe)
    {
        selectedRecipe = recipe;
        Debug.Log("Selected Recipe: " + recipe.RecipeName);
    }

    private void OnMakeDish()
    {
        if (selectedRecipe != null && recipePrefab != null && spawnPoint != null)
        {
            GameObject newRecipe = Instantiate(recipePrefab, spawnPoint.position, spawnPoint.rotation);
            // ��������������������ɵ�Ԥ��������ԣ��������ơ�ͼ���
            Debug.Log("Created Recipe: " + selectedRecipe.RecipeName);
        }
        else
        {
            Debug.LogWarning("Recipe or prefab or spawn point is not set.");
        }
    }
}

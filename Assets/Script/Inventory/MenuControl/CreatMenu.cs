using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreatMenu : MonoBehaviour
{
    public static CreatMenu instance;

    public GameObject slotGrid;
    public Menu menu;
    public Recipe recipe;
    public GameObject TextPrefab;
    private int index;
    List<TextMeshProUGUI> Names;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    public void OnEnable()
    {
        Names = new List<TextMeshProUGUI>();
        //menu.recipes.Clear();
        index = 0;
        if (Names.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject text = Instantiate(TextPrefab, slotGrid.transform);
                TextMeshProUGUI nameText = text.GetComponent<TextMeshProUGUI>();
                Names.Add(nameText);
            }
        }
        for (int i = 0; i < 10; i++)
        {
            Names[i].text = "";
        }
        RefreshItem();
    }

    public void addRecipe()
    {
        if (instance != null && menu.recipes.Count > 0)
        {
            for (int i = 0; i < menu.recipes.Count; i++)
            {

                if (menu.recipes[i] != null)
                    Names[i].text = menu.recipes[i].RecipeName;
            }
        }
    }


    public static void RefreshItem()
    {
        if (instance == null || instance.slotGrid == null || instance.menu == null)
        {
            Debug.LogError("CreatMenu instance, slotGrid, or menu is not assigned.");
            return;
        }

        if (instance.slotGrid!=null)
        {
            for (int i = 0; i < 10; i++)
            {
                instance.Names[i].text = "";
            }
        }
        instance.addRecipe();

    }

    public void OnClicked()
    {
        if (instance != null)
        {
            if (menu.recipes.Count == 10)
            {
                Debug.Log("The menu is full.");
                return;
            }
        }

        if (recipe != null)
        {
            if (menu.recipes.Contains(recipe))
            {
                return;
            }
            else
            {
                index++;
                Names[index].text = recipe.name;
                menu.recipes.Add(recipe);
            }
        }

        RefreshItem();

    }

    public void onClear()
    {
        menu.recipes.Clear();
    }
}


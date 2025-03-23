using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; set; }
    private enum MenuState
    {
        Restaurant,
        Kitchen,
        Street,
        Shop
    }
    public List<Image> images = new List<Image>();

    private Dictionary<MenuState, Image> buttons = new Dictionary<MenuState, Image>();
    [SerializeField]
    private MenuState curState = MenuState.Restaurant;
    private Color selectedColor = Color.yellow;
    private Color unselectedColor = Color.white;
    private bool isLoadingScene = false;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(transform.parent);
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            buttons.Add((MenuState)i, images[i]);
        }
        buttons[curState].color = selectedColor;
    }

    public void OnClicked(string name)
    {
        Debug.Log(name);
        switch (name)
        {
            case "restaurant":
                Switch(MenuState.Restaurant);
                break;
            case "kitchen":
                Switch(MenuState.Kitchen);
                break;
            case "street":
                Switch(MenuState.Street);
                break;
            case "shop":
                Switch(MenuState.Shop);
                break;
            default:
                Debug.LogError("Unknown button name: " + name);
                break;
        }
    }

    private void Switch(MenuState state)
    {
        if (state == curState || isLoadingScene)
        {
            return;
        }

        Debug.Log(curState + " Switch To " + state);
        buttons[curState].color = unselectedColor;
        buttons[state].color = selectedColor;
        curState = state;

        StartCoroutine(LoadSceneAsync(state.ToString()));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoadingScene = true;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        isLoadingScene = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSwitchManager : Singleton<MenuSwitchManager>
{
    public GameObject staticCanvas;

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



    protected override void Awake()
    {
        base.Awake();
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

        StartCoroutine(LoadSceneAsync(state));
    }

    private IEnumerator LoadSceneAsync(MenuState state)
    {
        isLoadingScene = true;
        string sceneName = state.ToString();
        AsyncOperation asyncOperation;
        if (state == MenuState.Street)
        {
            RestaurantAudioManager.Instance.OnEndMusic();
            staticCanvas.SetActive(false);
        }
        else
        {
            RestaurantAudioManager.Instance.OnStartMusic();
            staticCanvas.SetActive(true);
        }
        if (curState == MenuState.Restaurant)//由餐厅切换到其他场景
        {
            asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        else if (state == MenuState.Restaurant)//由其他场景切换到餐厅
        {
            asyncOperation = SceneManager.UnloadSceneAsync(curState.ToString());
        }
        else//由其他场景切换到其他场景
        {
            SceneManager.UnloadSceneAsync(curState.ToString());
            asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        curState = state;
        isLoadingScene = false;
    }
    //private IEnumerator LoadSceneAsync(string sceneName)
    //{
    //    isLoadingScene = true;
    //    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
    //    while (!asyncOperation.isDone)
    //    {
    //        yield return null;
    //    }
    //    isLoadingScene = false;
    //}
}

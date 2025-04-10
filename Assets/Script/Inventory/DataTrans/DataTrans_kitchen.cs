using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataTrans_kitchen : MonoBehaviour
{
    public static DataTrans_kitchen instance;

    public List<Card> gettenCards = new List<Card>();

    void Start()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void Update()
    {
        if (GameDataManager.Instance != null)
        {
            if (GameDataManager.Instance.allDrawnCards != null)
            {
                gettenCards = GameDataManager.Instance.allDrawnCards;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Kitchen") // 替换 "Kitchen" 为你要检测的场景名称
        {
            LoadScence();
            Debug.Log("LoadScence::::::::::::::: kitchen");
        }
    }

    private void LoadScence()
    {
        if (GetItems.instance)
        {
            GetItems.instance.gettenItems = gettenCards;
            Debug.Log("传输成功");
            GetItems.instance.RefreshItems();
        }
        else
        {
            Debug.Log("GetItems is null");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

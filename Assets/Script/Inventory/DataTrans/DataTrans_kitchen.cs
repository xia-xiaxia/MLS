using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataTrans_kitchen : MonoBehaviour
{
    public static DataTrans_kitchen instance;

    public Dictionary<string, Rarity> BagAndInventory = new Dictionary<string, Rarity>();

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
        }
    }

    void Update()
    {
        if (GameDataManager.Instance != null)
        {
            //BagAndInventory = GameDataManager.Instance.recipeMaxRarity;
            if (GameDataManager.Instance.recipeMaxRarity != null)
            {
                Debug.Log("recipeMaxRarity: " + GameDataManager.Instance.recipeMaxRarity.Count);
                foreach (var kvp in GameDataManager.Instance.recipeMaxRarity)
                {
                    string name = kvp.Key;
                    Rarity rarity = kvp.Value;
                    BagAndInventory[name] = rarity;
                    Debug.Log("name: " + name + " rarity: " + rarity);
                }
            }
            else Debug.Log("recipeMaxRarity:" + null);
            Debug.Log("BagAndInventory: " + BagAndInventory.Count);
        }
        else Debug.Log("GameDataManager.Instance is null");
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Kitchen") // 替换 "Scene1" 为你要检测的场景名称
        {
            LoadScence();
        }
    }

    private void LoadScence()
    {
        if (GetItems.instance)
        {
            GetItems.instance.itemsRarity = BagAndInventory;
            Debug.Log("传输成功");
        }
        else return;

    }
}

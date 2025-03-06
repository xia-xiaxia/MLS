using UnityEngine;
public class GameManager : MonoBehaviour
{
    [Header("策划配置")]
    public TimeConfig timeConfig;
    public EconomyConfig economyConfig;
    public ChefData[] chefConfigs;
    public DishData[] dishConfigs;

    /*
    void Start()
    {
        // 初始化数据管理中心
        DataManager.Instance.Initialize
        (
            timeConfig,
            economyConfig,
            chefConfigs,
            dishConfigs
        );

        // 启动游戏循环
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            float delta = Time.deltaTime;
            DataManager.Instance.CurrentTime.Update(delta);
            yield return null;
        }
    }
  */
}
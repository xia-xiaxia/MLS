using UnityEngine;
public class GameManager : MonoBehaviour
{
    [Header("�߻�����")]
    public TimeConfig timeConfig;
    public EconomyConfig economyConfig;
    public ChefData[] chefConfigs;
    public DishData[] dishConfigs;

    /*
    void Start()
    {
        // ��ʼ�����ݹ�������
        DataManager.Instance.Initialize
        (
            timeConfig,
            economyConfig,
            chefConfigs,
            dishConfigs
        );

        // ������Ϸѭ��
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
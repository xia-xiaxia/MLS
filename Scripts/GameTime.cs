[System.Serializable]
public class GameTime
{
    public int gameDay = 1;
    public int gameHour;
    private float realTimeAccumulator;
    private readonly TimeConfig config;

    public GameTime(TimeConfig config)
    {
        this.config = config;
        gameHour = config.dailyStartHour;
    }

    public void Update(float deltaTime)
    {
        realTimeAccumulator += deltaTime * config.timeScale;

        while (realTimeAccumulator >= 1f)
        { 
            // 1现实秒=1游戏小时
            realTimeAccumulator -= 1f;
            gameHour++;

            if (gameHour >= 24)//开始新的一天
            {
                gameHour = 0;
                gameDay++;
                DataManager.Instance.Economy.DailySettlement();
            }
        }
    }
}
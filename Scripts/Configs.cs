[System.Serializable]
public class TimeConfig
{
    public float timeScale = 1.0f;  // 时间流速
    public int dailyStartHour = 8;   // 每日营业开始时间
    public int stopEnterHour = 22;   // 停止进客时间
    public int forceCloseHour = 23;  // 强制关店时间,有这个吗？
}

public class EconomyConfig
{
    public float initialG = 1000f;   // 初始资金，是这个吗
    public float dailyFixedCost = 300f;  // 每日固定支出
}
using UnityEngine;

// 经济系统
public class EconomySystem
{
    // 属性访问
    public int currentGold { get; private set; }

    // 配置参数（可抽离为ScriptableObject）
    public struct Config
    {
        public int baseRent;
        public int baseSalary;
        public int seatCount;
        public float chefSpeed;
        public float cleanSpeed;
        public float walkSpeed;
    }

    private Config _config;

    public void Initialize(int initialGold)
    {
        currentGold = initialGold;

        // 初始化默认配置
        _config = new Config
        {
            baseRent = 1000,
            baseSalary = 500,
            seatCount = 5,
            chefSpeed = 0.1f,
            cleanSpeed = 60f,
            walkSpeed = 5f
        };
    }

    // 收益计算
    public DailyStats CalculateDailyProfit()
    {
        var stats = new DailyStats
        {
            customers = CalculateDailyCustomers(),
            maxDishes = CalculateMaxProduction(),
            day = GameDataManager.Instance.gameDays
        };

        stats.actualSales = Mathf.Min(stats.customers * 2, stats.maxDishes);
        stats.revenue = stats.actualSales * 3;
        stats.expenses = _config.baseRent + _config.baseSalary;
        stats.profit = stats.revenue - stats.expenses;
        stats.totalGold = currentGold + stats.profit;

        return stats;
    }

    // 执行每日结算
    public void ApplyDailyResult(DailyStats stats)
    {
        currentGold = stats.totalGold;
        GameDataManager.Instance.totalGold = currentGold;
    }

    // 私有计算方法
    private int CalculateDailyCustomers() =>
        Mathf.FloorToInt(_config.seatCount * (50400f / GetTotalServiceTime()));

    private float GetTotalServiceTime() =>
        300f + _config.cleanSpeed + _config.walkSpeed;

    private int CalculateMaxProduction() =>
        Mathf.FloorToInt(1 * _config.chefSpeed * 50400f); // 暂时固定1个厨师
}

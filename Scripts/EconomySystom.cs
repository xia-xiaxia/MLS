[System.Serializable]
public class EconomySystem
{
    public int gold = 10000;
    public int rentCost = 1000;
    public int salaryCost = 500;
    public int seatCount = 5;
    public int chefCount = 1;
    public float chefSpeed = 0.1f; // 盘/秒
    public float cleanSpeed = 60f; // 秒/桌
    public float walkSpeed = 5f; // 秒/顾客

    public int CalculateDailyCustomers()
    {
        float eatingTime = 300f;//顾客用餐时间
        float totalTime = eatingTime + cleanSpeed + walkSpeed;//服务一个顾客所需的时间
        return Mathf.FloorToInt(seatCount * (50400f / totalTime));
    }

    public int CalculateMaxProduction()
    {
        return Mathf.FloorToInt(chefCount * chefSpeed * 50400f);//厨师最大生产量
    }

    //计算每日收益
    public int CalculateProfit()
    {
        int customers = CalculateDailyCustomers();
        int maxDishes = CalculateMaxProduction();
        int actualSales = Mathf.Min(customers * 2, maxDishes);//销售额

        int revenue = actualSales * 3; // 3金币利润/盘
        int expenses = rentCost + salaryCost;//消耗
        int profit = revenue - expenses;

        gold += profit;
        return profit;
    }

    //抽卡消耗资源
    public void BuyGacha(bool isPremium)
    {
        int cost = isPremium ? 3000 : 1000;
        if (gold < cost) return;

        gold -= cost;
        var item = GameManager.Instance.gachaSystem.DrawItem(isPremium);
        GameManager.Instance.ProcessGachaResult(item);
    }
}

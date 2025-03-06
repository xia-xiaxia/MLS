[System.Serializable]
public class EconomySystem
{
    //初始资金
    public float CurrentG; 
    private readonly EconomyConfig config;

    public EconomySystem(EconomyConfig config)
    {
        this.config = config;
        CurrentG = config.initialG;
    }
    
    //每日结算
    public void DailySettlement()
    {
        float income = CalculateIncome();
        float cost = CalculateCost();
        float netProfit = income - cost - config.dailyFixedCost;//总利润

        CurrentG += netProfit;//累计资金
    }

    private float CalculateIncome() 
    {
        return 0.5f;
        //从订单系统获取数据 
        //看看挣了多少钱
        //任务奖励
    }
    private float CalculateCost() 
    {
        return 0.5f;
        // 从库存系统获取数据
        // 他因该会有做菜要花的钱
        // 他应该会有员工工资，答应我按天支付好吗
        // 可能会有装修，抽卡，招募费用
    }
    
}
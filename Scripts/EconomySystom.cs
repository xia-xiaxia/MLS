[System.Serializable]
public class EconomySystem
{
    //��ʼ�ʽ�
    public float CurrentG; 
    private readonly EconomyConfig config;

    public EconomySystem(EconomyConfig config)
    {
        this.config = config;
        CurrentG = config.initialG;
    }
    
    //ÿ�ս���
    public void DailySettlement()
    {
        float income = CalculateIncome();
        float cost = CalculateCost();
        float netProfit = income - cost - config.dailyFixedCost;//������

        CurrentG += netProfit;//�ۼ��ʽ�
    }

    private float CalculateIncome() 
    {
        return 0.5f;
        //�Ӷ���ϵͳ��ȡ���� 
        //�������˶���Ǯ
        //������
    }
    private float CalculateCost() 
    {
        return 0.5f;
        // �ӿ��ϵͳ��ȡ����
        // ����û�������Ҫ����Ǯ
        // ��Ӧ�û���Ա�����ʣ���Ӧ�Ұ���֧������
        // ���ܻ���װ�ޣ��鿨����ļ����
    }
    
}
[System.Serializable]
public class TimeConfig
{
    public float timeScale = 1.0f;  // ʱ������
    public int dailyStartHour = 8;   // ÿ��Ӫҵ��ʼʱ��
    public int stopEnterHour = 22;   // ֹͣ����ʱ��
    public int forceCloseHour = 23;  // ǿ�ƹص�ʱ��,�������
}

public class EconomyConfig
{
    public float initialG = 1000f;   // ��ʼ�ʽ��������
    public float dailyFixedCost = 300f;  // ÿ�չ̶�֧��
}
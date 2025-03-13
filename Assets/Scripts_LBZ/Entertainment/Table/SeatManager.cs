using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public static SeatManager Instance { get; set; }

    private Dictionary<int, Pair<bool, Seat>> tables = new Dictionary<int, Pair<bool, Seat>>();//��ʱ�����������ò���
    private List<int> freeTables = new List<int>();



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
    
    public void AddSeat(int index, Seat seat)//��ʼʱTable��TableManagerע��
    {
        tables.Add(index, new Pair<bool, Seat>(false, seat));
        freeTables.Add(index);
    }
    public void OccupySeat(int index)//����ѡ��
    {
        tables[index].first = true;
        if (freeTables.Contains(index))
            freeTables.Remove(index);
    }
    public void FreeSeat(int index)//��������
    {
        tables[index].first = false;
        if (!freeTables.Contains(index))
            freeTables.Add(index);
    }
    public bool CheckSeat(int index)//�鿴��λ
    {
        return tables[index].first;
    }
    public List<int> CheckFreeSeatCounts()//�鿴����λ����
    {
        return freeTables;
    }
    public Seat GetSeat(int index) => tables[index].second;
}
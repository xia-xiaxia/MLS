using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatManager : Singleton<SeatManager>
{
    private Dictionary<int, Pair<bool, Seat>> seats = new Dictionary<int, Pair<bool, Seat>>();//��ʱ�����������ò���
    //private List<int> freeSeats = new List<int>();



    public int AddSeat(Seat seat)//��ʼʱTable��TableManagerע��
    {
        int index = seats.Count;
        seats.Add(index, new Pair<bool, Seat>(false, seat));
        //if (!freeSeats.Contains(index))
        //    freeSeats.Add(index);
        return index;
    }
    public Seat GetSeat(int index) => seats[index].second;
    public Seat.SeatDir CheckSeatDir(int seatIndex)
    {
        return seats[seatIndex].second.seatDir;
    }
    //public void OccupySeat(int index)//����ѡ��
    //{
    //    seats[index].first = true;
    //    if (freeSeats.Contains(index))
    //        freeSeats.Remove(index);
    //}
    //public void EmptySeat(int index)//��������
    //{
    //    seats[index].first = false;
    //    if (!freeSeats.Contains(index))
    //        freeSeats.Add(index);
    //}
    //public List<int> CheckEmptySeatCounts()//�鿴����λ
    //{
    //    return freeSeats;
    //}
    //public bool CheckEmptyTable()
    //{
    //    return true;
    //}
}
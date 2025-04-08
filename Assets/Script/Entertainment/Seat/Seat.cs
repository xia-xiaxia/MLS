using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField]
    private int seatIndex;
    public enum SeatDir
    {
        None,
        Forward,
        Back,
        Left,
        Right
    }
    public SeatDir seatDir;



    private void Start()
    {
        seatIndex = SeatManager.Instance.AddSeat(this);
    }
    //private void Start()
    //{
    //    if (name.StartsWith("Seat") && name.Length == 6)
    //    {
    //        string numberPart = name.Substring(4, 2);
    //        if (int.TryParse(numberPart, out seatIndex))
    //        {
    //            SeatManager.Instance.AddSeat(seatIndex, this);
    //        }
    //        else
    //        {
    //            Debug.LogError("�޷������������");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("�������Ƹ�ʽ����ȷ");
    //    }
    //}
}

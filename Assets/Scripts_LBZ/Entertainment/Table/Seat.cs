using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    private int index;



    private void Start()
    {
        if (name.StartsWith("Seat") && name.Length == 6)
        {
            string numberPart = name.Substring(4, 2);
            if (int.TryParse(numberPart, out index))
            {
                SeatManager.Instance.AddSeat(index, this);
            }
            else
            {
                Debug.LogError("�޷������������");
            }
        }
        else
        {
            Debug.LogError("�������Ƹ�ʽ����ȷ");
        }
    }
}

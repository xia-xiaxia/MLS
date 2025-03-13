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
                Debug.LogError("无法解析桌子序号");
            }
        }
        else
        {
            Debug.LogError("桌子名称格式不正确");
        }
    }
}

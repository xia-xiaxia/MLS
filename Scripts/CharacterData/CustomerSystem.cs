using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;

//顾客服务系统
public class CustomerSystem
{
    public int seats = 5;
    public int diningTime = 300;
    public int cleaningTime = 60;

    public int CalculateDailyCustomers()
    {
        return seats * 50400 / (diningTime + cleaningTime);//计算顾客数
    }
}

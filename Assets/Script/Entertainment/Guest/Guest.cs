using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuestState
{
    GetIn,//初始状态
    WaitForOrder,//到达座位后转换到此状态
    Order,//server到达后转换到此状态
    WaitForFood,//点完餐后转换到此状态
    Eat,//上完菜后转换到此状态
    WaitForBill,//吃完饭后转换到此状态
    Bill,//server到达后转换到此状态
    Leave
}
public class Guest
{
    public int index;
    public GuestState state = GuestState.GetIn;
    public TaskBase task;
}

public abstract class TaskBase
{
    public Guest guest;
}

public class OrderTask : TaskBase
{
    public OrderTask(Guest guest)
    {
        this.guest = guest;
        RestaurantManager.Instance.EnqueueTaskBase(this);
    }
}
public class ServeTaskBase : TaskBase
{
    public int dishCount;
    public int servedDishCount = 0;
    public List<ServeTask> serveTasks;

    public ServeTaskBase(Guest guest, List<ServeTask> orders)
    {
        this.guest = guest;
        this.serveTasks = orders;
        if (orders != null)
            dishCount = orders.Count;
    }
}
public class ServeTask : ServeTaskBase
{
    public Recipe recipe;
    public bool isFetched = false;
    public bool isServed = false;

    public ServeTask(Recipe recipe, Guest guest) : base(guest, null)
    {
        this.recipe = recipe;
        this.guest = guest;
    }
    public void IsServed()
    {
        //Debug.Log(recipe.RecipeName + " is served");
        if (!isServed)
        {
            isServed = true;
            ((ServeTaskBase)guest.task).servedDishCount++;
        }
    }
}
public class BillTask : TaskBase
{
    public BillTask(Guest guest)
    {
        this.guest = guest;
        RestaurantManager.Instance.EnqueueTaskBase(this);//可以写到基类里
    }
}
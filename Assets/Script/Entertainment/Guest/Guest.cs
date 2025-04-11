using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuestState
{
    None,//伴随客人
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
    public int seatIndex;
    public int tableIndex;
    public int dishCount;
    public bool isOrderer;//一桌只有一个点菜者
    public List<Guest> accompanyings;
    public Seat.SeatDir seatDir;
    public GuestState state;
    public Bubble bubble;
    public TaskBase task;

    public void UpdateState(GuestState newState)
    {
        switch (newState)
        {
            case GuestState.None:
                state = GuestState.None;
                bubble.UpdateState("");
                break;
            case GuestState.GetIn:
                state = GuestState.GetIn;
                bubble.UpdateState("新！");
                break;
            case GuestState.WaitForOrder:
                state = GuestState.WaitForOrder;
                bubble.UpdateState("等待点餐...");
                break;
            case GuestState.Order:
                state = GuestState.Order;
                bubble.UpdateState("点餐中~");
                break;
            case GuestState.WaitForFood:
                state = GuestState.WaitForFood;
                bubble.UpdateState("等待上菜...");
                break;
            case GuestState.Eat:
                state = GuestState.Eat;
                bubble.UpdateState("用餐中~");
                break;
            case GuestState.WaitForBill:
                state = GuestState.WaitForBill;
                bubble.UpdateState("等待结账...");
                break;
            case GuestState.Bill:
                state = GuestState.Bill;
                bubble.UpdateState("结账中~");
                break;
            case GuestState.Leave:
                state = GuestState.Leave;
                bubble.UpdateState("离开");
                break;
        }
    }
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
        Debug.Log(recipe.RecipeName);
    }
    public void IsServed()
    {
        //Debug.Log(recipe.RecipeName + " is served");
        if (!isServed)
        {
            isServed = true;
            TableManager.Instance.ServeDish(guest.tableIndex, guest.dishCount, ((ServeTaskBase)guest.task).servedDishCount, recipe.RecipeName);
            RestaurantEconomyManager.Instance.AddRevenue(recipe.RecipeName);
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
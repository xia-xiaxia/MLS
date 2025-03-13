using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuestAI : BTAI
{
    public Guest guest = new Guest();



    protected override void Start()
    {
        base.Start();

        root = new Selector("root");



        //进门
        Sequence isSeated = new Sequence("isSeated");
        isSeated.AddChild(new Leaf("isSeated", new ConditionStrategy(() => guest.state == GuestState.GetIn)));

        Selector GetIn = new Selector("GetIn");
        GetIn.AddChild(new Leaf("goToRandomDoor", new ExecuteOnceStrategy(new GotoRandomDoorStrategy(agent, transform))));
        GetIn.AddChild(new Leaf("chooseSeat", new GotoRandomSeatStrategy(agent, guest)));//此处给guest设置了index
        GetIn.AddChild(new Leaf("seated", new ActionStrategy(() => guest.state = GuestState.WaitForOrder)));

        isSeated.AddChild(GetIn);



        //点餐
        Sequence isOrdered = new Sequence("isOrdered");
        isOrdered.AddChild(new Leaf("isOrdered", new ConditionStrategy(() => guest.state == GuestState.WaitForOrder || guest.state == GuestState.Order)));

        Selector waitOrOrder = new Selector("waitOrOrder");

        Sequence waitForOrder = new Sequence("waitForOrder");
        waitForOrder.AddChild(new Leaf("isWaiting", new ConditionStrategy(() => guest.state == GuestState.WaitForOrder)));
        waitForOrder.AddChild(new Leaf("callServer", new ActionStrategy(() => { guest.task ??= new OrderTask(guest); })));//此处给guest初始化了orderTask
        waitForOrder.AddChild(new Leaf("waitOrOrder", new WaitingStrategy()));

        Sequence order = new Sequence("order");
        order.AddChild(new Leaf("order", new ExecuteOnceStrategy(new GuestOrderStrategy(guest))));
        order.AddChild(new Leaf("waitForFood", new ActionStrategy(() => guest.state = GuestState.WaitForFood)));

        waitOrOrder.AddChild(waitForOrder);
        waitOrOrder.AddChild(order);
        isOrdered.AddChild(waitOrOrder);



        //用餐
        Sequence isEating = new Sequence("isEating");
        isEating.AddChild(new Leaf("isEating", new ConditionStrategy(() => guest.state == GuestState.WaitForFood || guest.state == GuestState.Eat)));

        Selector waitOrEat = new Selector("waitOrEat");

        Sequence waitForFood = new Sequence("waitForFood");
        waitForFood.AddChild(new Leaf("isWaiting", new ConditionStrategy(() => guest.state == GuestState.WaitForFood)));
        waitForFood.AddChild(new Leaf("isAFoodServed", new ConditionStrategy(() =>
        {
            Debug.Log(((ServeTaskBase)guest.task).servedDishCount + " dishes are served");
            if (((ServeTaskBase)guest.task).servedDishCount == 0)
                return true;
            else
            {
                guest.state = GuestState.Eat;
                return false;
            }
        })));
        waitForFood.AddChild(new Leaf("waiting", new WaitingStrategy()));

        Sequence waitAllDishes = new Sequence("waitAllDishes");
        waitAllDishes.AddChild(new Leaf("isAllDishesServed", new ConditionStrategy(() =>
        {
            bool temp = ((ServeTaskBase)guest.task).servedDishCount < ((ServeTaskBase)guest.task).dishCount;
            if (temp)
                Debug.Log(((ServeTaskBase)guest.task).servedDishCount + " dishes are served");
            return temp;
        })));
        waitAllDishes.AddChild(new Leaf("waiting", new WaitingStrategy()));

        Sequence eat = new Sequence("eat");
        eat.AddChild(new Leaf("eating", new EatStrategy(executeInterval, guest)));
        eat.AddChild(new Leaf("finishEating", new ActionStrategy(() =>
        {
            if (guest.state == GuestState.Eat)
            {
                guest.task = new BillTask(guest);//此处给guest更新了任务
                guest.state = GuestState.WaitForBill;
            }
        })));

        waitOrEat.AddChild(waitForFood);
        waitOrEat.AddChild(waitAllDishes);
        waitOrEat.AddChild(eat);
        isEating.AddChild(waitOrEat);



        //结账
        Sequence isbilling = new Sequence("isbilling");
        isbilling.AddChild(new Leaf("isbilling", new ConditionStrategy(() => guest.state == GuestState.WaitForBill || guest.state == GuestState.Bill)));

        Selector waitOrBill = new Selector("waitOrBill");

        Sequence waitForBill = new Sequence("waitForBill");
        waitForBill.AddChild(new Leaf("isWaiting", new ConditionStrategy(() => guest.state == GuestState.WaitForBill)));
        waitForBill.AddChild(new Leaf("waiting", new WaitingStrategy()));

        Sequence bill = new Sequence("bill");
        bill.AddChild(new Leaf("billing", new BillStrategy(executeInterval, guest)));

        waitOrBill.AddChild(waitForBill);
        waitOrBill.AddChild(bill);
        isbilling.AddChild(waitOrBill);



        //离开
        Sequence isLeaving = new Sequence("isLeaving");
        isLeaving.AddChild(new Leaf("isLeaving", new ConditionStrategy(() => guest.state == GuestState.Leave)));
        isLeaving.AddChild(new Leaf("leave", new LeaveStrategy(agent)));
        isLeaving.AddChild(new Leaf("destroy", new ActionStrategy(() => Destroy(gameObject))));




        //Inverter test = new Inverter("test");
        //test.AddChild(new Leaf("test", new ActionStrategy(() => Debug.Log(guest.state))));
        //root.AddChild(test);
        root.AddChild(isSeated);
        root.AddChild(isOrdered);
        root.AddChild(isEating);
        root.AddChild(isbilling);
        root.AddChild(isLeaving);
    }
    protected override void Update()
    {
        base.Update();
    }
}
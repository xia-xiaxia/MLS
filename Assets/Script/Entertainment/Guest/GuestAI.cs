using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuestAI : BTAI
{
    public Guest guest = new Guest();
    public Animator animator;



    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();



        root = new Selector("root");



        //进门
        Sequence isSeated = new Sequence("isSeated");
        isSeated.AddChild(new Leaf("isSeated", new ConditionStrategy(() => guest.state == GuestState.GetIn)));

        Selector GetIn = new Selector("GetIn");
        GetIn.AddChild(new Leaf("goToRandomDoor", new ExecuteOnceStrategy(new GotoRandomDoorStrategy(agent, transform))));
        GetIn.AddChild(new Leaf("chooseSeat", new GotoSeatStrategy(agent, guest)));//此处给guest设置了index
        GetIn.AddChild(new Leaf("seated", new ActionStrategy(() => 
        {
            if (!guest.isOrderer)
                guest.UpdateState(GuestState.None);
            else
                guest.UpdateState(GuestState.WaitForOrder);
        })));

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
        order.AddChild(new Leaf("waitForFood", new ActionStrategy(() => guest.UpdateState(GuestState.WaitForFood))));

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
            //Debug.Log(((ServeTaskBase)guest.task).servedDishCount + " dishes are served");
            if (((ServeTaskBase)guest.task).servedDishCount == 0)
                return true;
            else
            {
                guest.UpdateState(GuestState.Eat);
                return false;
            }
        })));
        waitForFood.AddChild(new Leaf("waiting", new WaitingStrategy()));

        Sequence waitAllDishes = new Sequence("waitAllDishes");
        waitAllDishes.AddChild(new Leaf("isAllDishesServed", new ConditionStrategy(() =>
        {
            bool temp = ((ServeTaskBase)guest.task).servedDishCount < ((ServeTaskBase)guest.task).dishCount;
            //if (temp)
            //    Debug.Log(((ServeTaskBase)guest.task).servedDishCount + " dishes are served");
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
                guest.UpdateState(GuestState.WaitForBill);
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
        isLeaving.AddChild(new Leaf("destroy", new ActionStrategy(() => GuestManager.Instance.DestroyGuest(gameObject))));




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

        //控制动画的切换
        if (guest.state == GuestState.GetIn || guest.state == GuestState.Leave)
        {
            if (agent.velocity.magnitude > 0)
            {
                float angle = Vector2.SignedAngle(new Vector2(1, 1), new Vector2(agent.velocity.x, agent.velocity.y));
                if (angle >= 0 && angle < 90)
                    animator.Play("Back");
                else if (angle >= 90 && angle <= 180)
                    animator.Play("Left");
                else if (angle >= -180 && angle < -90)
                    animator.Play("Forward");
                else if (angle >= -90 && angle < 0)
                    animator.Play("Right");
                //float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
                //if (angle > -30f && angle < 30f)
                //    animator.Play("Right");
                //else if (angle >= 30 && angle <= 150)
                //    animator.Play("Back");
                //else if (angle > 150 || angle < -150)
                //    animator.Play("Left");
                //else if (angle >= -150 && angle <= -30)
                //    animator.Play("Forward");
            }
            else
                animator.Play("IdleForward");
        }
        else if (guest.state == GuestState.WaitForOrder || guest.state == GuestState.None)
            animator.Play("Idle" + guest.seatDir);
    }
}
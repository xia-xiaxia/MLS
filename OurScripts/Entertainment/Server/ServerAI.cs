using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class ServerAI : BTAI
{
    public Transform waitingSite;
    public Transform hangOrderSite;
    public Transform dishSite;
    public Bubble bubble;

    private Queue<TaskBase> leftTasks = new Queue<TaskBase>();
    private TaskBase curTask;

    public int willingness = 60;
    public float cleanTime = 3f;



    protected override void Start()
    {
        /*
        sequence：1.意愿接取？
                  2.有任务？有left任务？没有当前任务？加入到当前任务
                          ：有当前任务？1.order
                                        2.serve
                                        3.bill
                                        ：回到前台
        */

        base.Start();

        root = new ExecuteAll("root");

        /*
        root.AddChild(new Leaf("Debug", new ActionStrategy(() =>
        {
            if (curTask != null)
            {
                Debug.LogWarning("curTask : " + curTask);
                if (curTask.GetType() == typeof(ServeTask))
                    Debug.LogWarning("curTask : " + ((ServeTask)curTask).dishName);
            }
            foreach (var leftTask in leftTasks)
            {
                Debug.LogWarning("leftTask : " + leftTask); 
                if (leftTask.GetType() == typeof(ServeTask))
                    Debug.LogWarning("leftTask : " + ((ServeTask)leftTask).dishName);
            }
        })));
        */

        // { 意愿接取任务
        Sequence isAcceptTask = new Sequence("isAcceptTask");
        isAcceptTask.AddChild(new Leaf("isNewTaskToDo", new ConditionStrategy(() => RestaurantManager.Instance.curTask != null)));
        isAcceptTask.AddChild(new Leaf("acceptWork", new ActionStrategy(() =>
        {
            if (willingness > 20 && !RestaurantManager.Instance.CheckWillinger(this))
            {
                //Debug.Log("WillingToAcccept");
                RestaurantManager.Instance.AddWillinger(this);
                willingness -= 20;
            }
        })));
        // }

        // { 是否有任务
        Selector isTask = new Selector("isTask");
        // [ 有剩余任务
        Sequence isLeftTask = new Sequence("isLeftTask");
        isLeftTask.AddChild(new Leaf("isLeftTask", new ConditionStrategy(() => leftTasks.Count > 0)));
        isLeftTask.AddChild(new Leaf("isCurTask", new ConditionStrategy(() => curTask == null)));
        isLeftTask.AddChild(new Leaf("startNextTask", new ActionStrategy(() => { curTask = leftTasks.Dequeue(); /*Debug.Log("StartNextTask");*/ })));
        // ]
        // [ 有当前任务
        Sequence isCurTask = new Sequence("isCurTask");
        isCurTask.AddChild(new Leaf("isCurTask", new ConditionStrategy(() => curTask != null)));
        Allocator allocateTask = new Allocator("allocateTask", () =>
        {
            switch (curTask)
            {
                case OrderTask _:
                    return 1;
                case ServeTask _:
                    return 2;
                case BillTask _:
                    return 3;
                default:
                    return 0;
            }
        });
        // order
        Selector order = new Selector("order");
        Sequence isWaitForOrder = new Sequence("isWaitForServe");
        isWaitForOrder.AddChild(new Leaf("isWaitForServe", new ConditionStrategy(() => curTask.guest.state == GuestState.WaitForOrder)));
        isWaitForOrder.AddChild(new Leaf("headToOrder", new HeadToOrderStrategy(agent, this)));
        order.AddChild(isWaitForOrder);
        order.AddChild(new Leaf("waitOrder", new ConditionStrategy(() => curTask.guest.state == GuestState.Order)));
        order.AddChild(new Leaf("takeBackOrder", new TakeBackOrderStrategy(agent, hangOrderSite, this)));
        order.AddChild(new Leaf("finishTask", new ActionStrategy(() =>
        {
            //Debug.LogWarning("Finish a task");
            curTask = null;
            willingness += 20;
            timer = executeInterval;
        })));
        // serve
        Selector serve = new Selector("serve");
        Sequence fetchDish = new Sequence("fetchDish");
        fetchDish.AddChild(new Leaf("isFetched", new ConditionStrategy(() => !((ServeTask)curTask).isFetched)));
        fetchDish.AddChild(new Leaf("fetchDish", new FetchDishStrategy(agent, dishSite, this)));
        serve.AddChild(fetchDish);
        serve.AddChild(new Leaf("serve", new ServeStrategy(agent, this)));
        serve.AddChild(new Leaf("finishTask", new ActionStrategy(() =>
        {
            //Debug.LogWarning("Finish a task");
            curTask = null;
            willingness += 20;
            timer = executeInterval;
        })));
        // bill
        Selector bill = new Selector("bill");
        Sequence isWaitForBill = new Sequence("isWaitForBill");
        isWaitForBill.AddChild(new Leaf("isWaitForServe", new ConditionStrategy(() => curTask.guest.state == GuestState.WaitForBill)));
        isWaitForBill.AddChild(new Leaf("headToBill", new HeadToBillStrategy(agent, this)));
        bill.AddChild(isWaitForBill);
        bill.AddChild(new Leaf("waitBill", new ConditionStrategy(() => curTask.guest.state == GuestState.Bill)));
        bill.AddChild(new Leaf("clean", new CleanStrategy(executeInterval, cleanTime, this)));
        bill.AddChild(new Leaf("finishTask", new ActionStrategy(() =>
        {
            //Debug.LogWarning("Finish a task");
            curTask = null;
            willingness += 20;
            timer = executeInterval;
        })));
        //
        allocateTask.AddChild(order);
        allocateTask.AddChild(serve);
        allocateTask.AddChild(bill);
        //
        isCurTask.AddChild(allocateTask);
        // ]
        // [ 空闲、回到前台
        Sequence available = new Sequence("available");
        available.AddChild(new Leaf("goToForntDesk", new GotoFrontDesk(agent, waitingSite)));
        // ]
        isTask.AddChild(isLeftTask);
        isTask.AddChild(isCurTask);
        isTask.AddChild(available);
        // }

        root.AddChild(isAcceptTask);
        root.AddChild(isTask);
    }
    protected override void Update()
    {
        base.Update();
    }
    public void AcceptTask(TaskBase task)
    {
        leftTasks.Enqueue(task);
        //if (task.GetType() == typeof(ServeTask))
        //    Debug.LogWarning(((ServeTask)task).dishName);
    }
    public TaskBase GetCurTask()
    {
        return curTask;
    }
}
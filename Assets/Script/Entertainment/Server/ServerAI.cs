using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class ServerAI : BTAI
{
    public Server server = new Server();
    //���������ServerAI����д��ֱ�ӻ�ȡ���ⲿ���ͨ��������ServerManager�ڴ�����ʱ��ֵ
    public Animator animator;



    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();



        /*
        sequence��1.��Ը��ȡ��
                  2.��������left����û�е�ǰ���񣿼��뵽��ǰ����
                          ���е�ǰ����1.order
                                        2.serve
                                        3.bill
                                        ���ص�ǰ̨
        */
        root = new ExecuteAll("root");

        /* //���Խڵ�
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

        // { ��Ը��ȡ����
        Sequence isAcceptTask = new Sequence("isAcceptTask");
        isAcceptTask.AddChild(new Leaf("isNewTaskToDo", new ConditionStrategy(() => RestaurantManager.Instance.curTask != null)));
        isAcceptTask.AddChild(new Leaf("acceptWork", new ActionStrategy(() =>
        {
            //Debug.Log(name + " " + server.willingness);
            if (server.willingness >= 20 && !RestaurantManager.Instance.CheckWillinger(this))
            {
                //Debug.Log("WillingToAcccept");
                RestaurantManager.Instance.AddWillinger(this);
                server.willingness -= 20;
            }
        })));
        // }

        // { �Ƿ�������
        Selector isTask = new Selector("isTask");
        // [ ��ʣ������
        Sequence isLeftTask = new Sequence("isLeftTask");
        isLeftTask.AddChild(new Leaf("isLeftTask", new ConditionStrategy(() => server.leftTasks.Count > 0)));
        isLeftTask.AddChild(new Leaf("isCurTask", new ConditionStrategy(() => server.curTask == null)));
        isLeftTask.AddChild(new Leaf("startNextTask", new ActionStrategy(() => { server.curTask = server.leftTasks.Dequeue(); /*Debug.Log("StartNextTask");*/ })));
        // ]
        // [ �е�ǰ����
        Sequence isCurTask = new Sequence("isCurTask");
        isCurTask.AddChild(new Leaf("isCurTask", new ConditionStrategy(() => server.curTask != null)));
        Allocator allocateTask = new Allocator("allocateTask", () =>
        {
            switch (server.curTask)
            {
                case OrderTask _:
                    return 1;
                case ServeTask _:
                    return 2;
                case BillTask _:
                    return 3;
                default:
                    Debug.LogError("Unknown Task");
                    return 0;
            }
        });
        // order
        Selector order = new Selector("order");
        Sequence isWaitForOrder = new Sequence("isWaitForServe");
        isWaitForOrder.AddChild(new Leaf("isWaitForServe", new ConditionStrategy(() => server.curTask.guest.state == GuestState.WaitForOrder)));
        isWaitForOrder.AddChild(new Leaf("headToOrder", new HeadToOrderStrategy(agent, server)));
        order.AddChild(isWaitForOrder);
        order.AddChild(new Leaf("waitOrder", new ConditionStrategy(() => server.curTask.guest.state == GuestState.Order)));
        order.AddChild(new Leaf("takeBackOrder", new TakeBackOrderStrategy(agent, server.hangOrderSite, server)));
        order.AddChild(new Leaf("finishTask", new ActionStrategy(() =>
        {
            //Debug.LogWarning("Finish a task");
            server.curTask = null;
            server.willingness += 20;
            timer = executeInterval;
        })));
        // serve
        Selector serve = new Selector("serve");
        Sequence fetchDish = new Sequence("fetchDish");
        fetchDish.AddChild(new Leaf("isFetched", new ConditionStrategy(() => !((ServeTask)server.curTask).isFetched)));
        fetchDish.AddChild(new Leaf("fetchDish", new FetchDishStrategy(agent, server.dishSite, server)));
        serve.AddChild(fetchDish);
        serve.AddChild(new Leaf("serve", new ServeStrategy(agent, server)));
        serve.AddChild(new Leaf("finishTask", new ActionStrategy(() =>
        {
            //Debug.LogWarning("Finish a task");
            server.curTask = null;
            server.willingness += 20;
            timer = executeInterval;
        })));
        // bill
        Selector bill = new Selector("bill");
        Sequence isWaitForBill = new Sequence("isWaitForBill");
        isWaitForBill.AddChild(new Leaf("isWaitForServe", new ConditionStrategy(() => server.curTask.guest.state == GuestState.WaitForBill)));
        isWaitForBill.AddChild(new Leaf("headToBill", new HeadToBillStrategy(agent, server)));
        bill.AddChild(isWaitForBill);
        bill.AddChild(new Leaf("waitBill", new ConditionStrategy(() => server.curTask.guest.state == GuestState.Bill)));
        bill.AddChild(new Leaf("clean", new CleanStrategy(executeInterval, server.cleanTime, server)));
        bill.AddChild(new Leaf("finishTask", new ActionStrategy(() =>
        {
            //Debug.LogWarning("Finish a task");
            server.curTask = null;
            server.willingness += 20;
            timer = executeInterval;
        })));
        //
        allocateTask.AddChild(order);
        allocateTask.AddChild(serve);
        allocateTask.AddChild(bill);
        //
        isCurTask.AddChild(allocateTask);
        // ]
        // [ ���С��ص�ǰ̨
        Sequence available = new Sequence("available");
        available.AddChild(new Leaf("hideBubble", new ActionStrategy(() => server.bubble.Hide())));
        available.AddChild(new Leaf("goToForntDesk", new GotoFrontDesk(agent, server.waitingSite)));
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

        //���ƶ������л�
        if (agent.velocity.magnitude > 0)
        {
            float angle = Vector2.SignedAngle(new Vector2(1,1), new Vector2(agent.velocity.x, agent.velocity.y));
            if (angle >= 0 && angle < 90)
                animator.Play("Back");
            else if (angle >= 90 && angle <= 180)
                animator.Play("Left");
            else if (angle >= -180 && angle < -90)
                animator.Play("Forward");
            else if (angle >= -90 && angle < 0)
                animator.Play("Right");
        }
        else
            animator.Play("Idle");
    }
    public void AcceptTask(TaskBase task)
    {
        server.leftTasks.Enqueue(task);
        //if (task.GetType() == typeof(ServeTask))
        //    Debug.LogWarning(((ServeTask)task).dishName);
    }
}
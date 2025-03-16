using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GotoFrontDesk : IStrategy
{
    private NavMeshAgent agent;
    private Transform frontDesk;

    public GotoFrontDesk(NavMeshAgent agent, Transform frontDesk)
    {
        this.agent = agent;
        this.frontDesk = frontDesk;
    }
    public Node.State Execute()
    {
        agent.SetDestination(frontDesk.position);
        return Node.State.Running;
    }
}

public class HeadToOrderStrategy : IStrategy
{
    private NavMeshAgent agent;
    private ServerAI server;

    public HeadToOrderStrategy(NavMeshAgent agent, ServerAI server)
    {
        this.agent = agent;
        this.server = server;
    }
    public Node.State Execute()
    {
        agent.SetDestination(SeatManager.Instance.GetSeat(server.GetCurTask().guest.index).transform.position);
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
                return Node.State.Running;
            else
            {
                //Debug.Log("Start Order");
                if (server.GetCurTask().guest.state == GuestState.WaitForOrder)
                    server.GetCurTask().guest.state = GuestState.Order;
                agent.ResetPath();
                return Node.State.Failure;
            }
        }
        return Node.State.Running;
    }
}

public class TakeBackOrderStrategy : IStrategy
{
    private NavMeshAgent agent;
    private Transform hangOrderSite;
    private ServerAI server;

    public TakeBackOrderStrategy(NavMeshAgent agent, Transform hangOrderSite, ServerAI server)
    {
        this.agent = agent;
        this.hangOrderSite = hangOrderSite;
        this.server = server;
    }
    public Node.State Execute()
    {
        agent.SetDestination(hangOrderSite.position);
        server.bubble.UpdateState("order");
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
            {
                //Debug.Log("Backing");
                return Node.State.Running;
            }
            else
            {
                //Debug.Log("New Order hung on");
                agent.ResetPath();
                server.bubble.Hide();
                RestaurantManager.Instance.AddOrder(((ServeTaskBase)server.GetCurTask().guest.task).serveTasks);
                return Node.State.Failure;
            }
        }
        //Debug.Log("Backing");
        return Node.State.Running;
    }
}

public class FetchDishStrategy : IStrategy
{
    private NavMeshAgent agent;
    private Transform dishSite;
    private ServerAI server;

    public FetchDishStrategy(NavMeshAgent agent, Transform dishSite, ServerAI server)
    {
        this.agent = agent;
        this.dishSite = dishSite;
        this.server = server;
    }
    public Node.State Execute()
    {
        agent.SetDestination(dishSite.position);
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
            {
                //Debug.Log("Fetch dish");
                return Node.State.Running;
            }
            else
            {
                //Debug.Log("Get the dish");
                agent.ResetPath();
                ((ServeTask)server.GetCurTask()).isFetched = true;
                return Node.State.Failure;
            }
        }
        //Debug.Log("Fetch dish");
        return Node.State.Running;
    }
}

public class ServeStrategy : IStrategy
{
    private NavMeshAgent agent;
    private ServerAI server;
    private TaskBase task;

    public ServeStrategy(NavMeshAgent agent, ServerAI server)
    {
        this.agent = agent;
        this.server = server;
    }
    public Node.State Execute()
    {

        if (task == null)
            task = server.GetCurTask();
        agent.SetDestination(SeatManager.Instance.GetSeat(task.guest.index).transform.position);
        server.bubble.UpdateState("serve");
        //Debug.Log("Serving");
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
            {
                return Node.State.Running;
            }
            else
            {
                //Debug.Log("Served");
                agent.ResetPath();
                server.bubble.Hide();
                ((ServeTask)task).IsServed();
                task = null;
                return Node.State.Failure;
            }
        }
        return Node.State.Running;
    }
}

public class HeadToBillStrategy : IStrategy
{
    private NavMeshAgent agent;
    private ServerAI server;

    public HeadToBillStrategy(NavMeshAgent agent, ServerAI server)
    {
        this.agent = agent;
        this.server = server;
    }
    public Node.State Execute()
    {
        agent.SetDestination(SeatManager.Instance.GetSeat(server.GetCurTask().guest.index).transform.position);
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
                return Node.State.Running;
            else
            {
                //Debug.Log("Start Bill");
                if (server.GetCurTask().guest.state == GuestState.WaitForBill)
                    server.GetCurTask().guest.state = GuestState.Bill;
                agent.ResetPath();
                return Node.State.Failure;
            }
        }
        return Node.State.Running;
    }
}

public class CleanStrategy : IStrategy
{
    private float timer = 0;
    private float executeInterval;
    private float cleanTime;
    private ServerAI server;

    public CleanStrategy(float executeInterval, float cleanTime, ServerAI server)
    {
        this.executeInterval = executeInterval;
        this.cleanTime = cleanTime;
        this.server = server;
    }
    public Node.State Execute()
    {
        timer += executeInterval;
        if (timer >= cleanTime)
        {
            //Debug.Log("Finish cleaning");
            timer = 0;
            server.bubble.Hide();
            return Node.State.Failure;
        }
        else
        {
            server.bubble.UpdateState("clean");
            //Debug.Log("Cleaning");
            return Node.State.Running;
        }
    }
}

//public class DetermineAcceptionStrategy : IStrategy
//{
//    private ServerAI server;

//    public DetermineAcceptionStrategy(ServerAI server)
//    {
//        this.server = server;
//    }
//    public Node.State Execute()
//    {
//        if (server.willingness > 50)
//            return Node.State.Success;
//        else 
//            return Node.State.Failure;
//    }
//}

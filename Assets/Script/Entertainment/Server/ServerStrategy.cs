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
    private Server server;

    public HeadToOrderStrategy(NavMeshAgent agent, Server server)
    {
        this.agent = agent;
        this.server = server;
    }
    public Node.State Execute()
    {
        agent.SetDestination(SeatManager.Instance.GetSeat(server.curTask.guest.index).transform.position);
        server.bubble.UpdateState("前往点单——");
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
                return Node.State.Running;
            else
            {
                //Debug.Log("Start Order");
                if (server.curTask.guest.state == GuestState.WaitForOrder)
                {
                    server.curTask.guest.UpdateState(GuestState.Order);
                    server.bubble.Hide();
                }
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
    private Server server;

    public TakeBackOrderStrategy(NavMeshAgent agent, Transform hangOrderSite, Server server)
    {
        this.agent = agent;
        this.hangOrderSite = hangOrderSite;
        this.server = server;
    }
    public Node.State Execute()
    {
        agent.SetDestination(hangOrderSite.position);
        server.bubble.UpdateState("取回菜单...");
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
                RestaurantManager.Instance.AddOrder(((ServeTaskBase)server.curTask.guest.task).serveTasks);
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
    private Server server;

    public FetchDishStrategy(NavMeshAgent agent, Transform dishSite, Server server)
    {
        this.agent = agent;
        this.dishSite = dishSite;
        this.server = server;
    }
    public Node.State Execute()
    {
        agent.SetDestination(dishSite.position);
        server.bubble.UpdateState("取菜中...");
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
                ((ServeTask)server.curTask).isFetched = true;
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
    private Server server;
    private TaskBase task;

    public ServeStrategy(NavMeshAgent agent, Server server)
    {
        this.agent = agent;
        this.server = server;
    }
    public Node.State Execute()
    {

        if (task == null)
            task = server.curTask;
        agent.SetDestination(SeatManager.Instance.GetSeat(task.guest.index).transform.position);
        server.bubble.UpdateState("上菜——");
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
    private Server server;

    public HeadToBillStrategy(NavMeshAgent agent, Server server)
    {
        this.agent = agent;
        this.server = server;
    }
    public Node.State Execute()
    {
        agent.SetDestination(SeatManager.Instance.GetSeat(server.curTask.guest.index).transform.position);
        server.bubble.UpdateState("前往结账——");
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
                return Node.State.Running;
            else
            {
                //Debug.Log("Start Bill");
                if (server.curTask.guest.state == GuestState.WaitForBill)
                {
                    server.curTask.guest.UpdateState(GuestState.Bill);
                    server.bubble.Hide();
                }
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
    private Server server;

    public CleanStrategy(float executeInterval, float cleanTime, Server server)
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
            server.bubble.UpdateState("收拾桌子...");
            //Debug.Log("Cleaning");
            return Node.State.Running;
        }
    }
}

//public class DetermineAcceptionStrategy : IStrategy
//{
//    private ServerAI owner;

//    public DetermineAcceptionStrategy(ServerAI owner)
//    {
//        this.owner = owner;
//    }
//    public Node.State Execute()
//    {
//        if (owner.willingness > 50)
//            return Node.State.Success;
//        else 
//            return Node.State.Failure;
//    }
//}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GotoRandomDoorStrategy : IStrategy
{
    NavMeshAgent agent;
    Transform guest;
    List<Transform> doors;

    public GotoRandomDoorStrategy(NavMeshAgent agent, Transform guest)
    {
        this.agent = agent;
        this.guest = guest;
        this.doors = RestaurantManager.Instance.doors;
    }
    public Node.State Execute()
    {
        if (!agent.pathPending && agent.hasPath)
        {
            Debug.LogWarning("WrongPath");
            if (agent.remainingDistance >= agent.stoppingDistance)
                return Node.State.Running;
            else
                return Node.State.Failure;
        }

        int i = UnityEngine.Random.Range(6, doors.Count);
        guest.position = doors[i].position;
        agent.SetDestination(doors[i - 6].position);

        return Node.State.Success;
    }
}

public class GotoRandomSeatStrategy : IStrategy
{
    NavMeshAgent agent;
    private Guest guest;
    bool isSeatChosen;

    public GotoRandomSeatStrategy(NavMeshAgent agent, Guest guest)
    {
        this.agent = agent;
        this.isSeatChosen = false;
        this.guest = guest;
    }
    public Node.State Execute()
    {
        if (!isSeatChosen)
        {
            isSeatChosen = true;
            List<int> freeSeats = SeatManager.Instance.CheckFreeSeatCounts();
            guest.index = freeSeats[UnityEngine.Random.Range(0, freeSeats.Count)];
            SeatManager.Instance.OccupySeat(guest.index);
            return Node.State.Success;
        }
        agent.SetDestination(SeatManager.Instance.GetSeat(guest.index).transform.position);
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
            {
                return Node.State.Running;
            }
            else
            {
                Debug.Log("Guest Seated  " + guest.index);
                agent.ResetPath();
                return Node.State.Failure;
            }
        }
        return Node.State.Running;
    }
}

public class WaitingStrategy : IStrategy
{
    public WaitingStrategy() { }
    public Node.State Execute()
    {
        return Node.State.Running;
    }
}

public class GuestOrderStrategy : IStrategy
{
    private Guest guest;

    public GuestOrderStrategy(Guest guest)
    {
        this.guest = guest;
    }
    public Node.State Execute()
    {
        guest.task = new ServeTaskBase(guest, new List<ServeTask> {
        new ServeTask("Fish", guest),
        new ServeTask("Chicken", guest)
        });
        Debug.Log("Seat " + guest.index + " Ordered");
        return Node.State.Success;
    }
}

public class EatStrategy : IStrategy
{
    private float timer = 0;
    private float executeInterval;
    private float eatTime = 5f;
    private Guest guest;

    public EatStrategy(float executeInterval, Guest guest)
    {
        this.executeInterval = executeInterval;
        this.guest = guest;
    }
    public Node.State Execute()
    {
        timer += executeInterval;
        if (timer >= eatTime)
        {
            Debug.Log("Finish eating");
            return Node.State.Success;
        }
        else
        {
            Debug.Log("Eating");
            return Node.State.Running;
        }
    }
}

public class BillStrategy : IStrategy
{
    private float timer = 0;
    private float executeInterval;
    private float billTime = 3f;
    private Guest guest;

    public BillStrategy(float executeInterval, Guest guest)
    {
        this.executeInterval = executeInterval;
        this.guest = guest;
    }
    public Node.State Execute()
    {
        timer += executeInterval;
        if (timer >= billTime)
        {
            Debug.Log("Finish billing");
            guest.state = GuestState.Leave;
            SeatManager.Instance.FreeSeat(guest.index);
            return Node.State.Success;
        }
        else
        {
            Debug.Log("Billing");
            return Node.State.Running;
        }
    }
}

public class LeaveStrategy : IStrategy
{
    NavMeshAgent agent;
    List<Transform> doors;
    private int i;

    public LeaveStrategy(NavMeshAgent agent)
    {
        this.agent = agent;
        this.doors = RestaurantManager.Instance.doors;
        i = UnityEngine.Random.Range(6, doors.Count);
    }
    public Node.State Execute()
    {
        agent.SetDestination(doors[i - 6].position);
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
                return Node.State.Running;
            else
            {
                Debug.Log("Leave");
                agent.ResetPath();
                return Node.State.Success;
            }
        }
        return Node.State.Running;
    }
}
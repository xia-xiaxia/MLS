using NUnit.Framework;
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
    List<Transform> entrances;
    List<Transform> doors;

    public GotoRandomDoorStrategy(NavMeshAgent agent, Transform guest)
    {
        this.agent = agent;
        this.guest = guest;
        this.entrances = RestaurantManager.Instance.entrances;
        this.doors = RestaurantManager.Instance.doors;
    }
    public Node.State Execute()
    {
        if (!agent.pathPending && agent.hasPath)
        {
            //Debug.LogWarning("WrongPath");
            if (agent.remainingDistance >= agent.stoppingDistance)
                return Node.State.Running;
            else
                return Node.State.Failure;
        }

        int i = UnityEngine.Random.Range(0, entrances.Count);
        guest.position = entrances[i].position;
        int j = UnityEngine.Random.Range(0, doors.Count);
        agent.SetDestination(doors[j].position);

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
            List<int> freeSeats = SeatManager.Instance.CheckEmptySeatCounts();
            guest.seatIndex = freeSeats[UnityEngine.Random.Range(0, freeSeats.Count)];
            guest.seatDir = SeatManager.Instance.CheckSeatDir(guest.seatIndex);
            SeatManager.Instance.OccupySeat(guest.seatIndex);
            return Node.State.Success;
        }
        agent.SetDestination(SeatManager.Instance.GetSeat(guest.seatIndex).transform.position);
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
            {
                return Node.State.Running;
            }
            else
            {
                //Debug.Log("Guest Seated  " + guest.seatIndex);
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
        List<Recipe> recipes = OrderRandomly();
        List<ServeTask> serveTasks = new List<ServeTask>();
        foreach (var recipe in recipes)
            serveTasks.Add(new ServeTask(recipe, guest));
        guest.task = new ServeTaskBase(guest, serveTasks);
        //Debug.Log("Seat " + guest.seatIndex + " Ordered");
        return Node.State.Success;
    }

    private List<Recipe> OrderRandomly()
    {
        Menu menu = GuestManager.Instance.menu;
        if (menu.recipes.Count == 0)
        {
            Debug.LogWarning("No Recipes");
            return new List<Recipe> { ScriptableObject.CreateInstance<Recipe>() };
        }
        else
        {
            int n = UnityEngine.Random.Range(1, Math.Min(menu.recipes.Count, 7)); // ×î¶àÁùµÀ²Ë
            List<Recipe> recipes = GuestManager.Instance.menu.recipes.GetRandomElements(n);
            return recipes;
        }
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
            //Debug.Log("Finish eating");
            return Node.State.Success;
        }
        else
        {
            //Debug.Log("Eating");
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
            //Debug.Log("Finish billing");
            guest.UpdateState(GuestState.Leave);
            SeatManager.Instance.EmptySeat(guest.seatIndex);
            return Node.State.Success;
        }
        else
        {
            //Debug.Log("Billing");
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
        i = UnityEngine.Random.Range(0, doors.Count);
    }
    public Node.State Execute()
    {
        agent.SetDestination(doors[i].position);
        if (!agent.pathPending)
        {
            if (agent.remainingDistance >= agent.stoppingDistance)
                return Node.State.Running;
            else
            {
                //Debug.Log("Leave");
                agent.ResetPath();
                return Node.State.Success;
            }
        }
        return Node.State.Running;
    }
}
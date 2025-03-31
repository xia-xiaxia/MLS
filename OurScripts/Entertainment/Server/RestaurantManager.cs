using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantManager : MonoBehaviour
{
    public static RestaurantManager Instance { get; set; }

    private float executeInterval = 0.5f;
    private float executeTimer = 0;
    private float cookInterval = 3f;
    private float cookTimer = 0;
    public List<Transform> entrances;
    public List<Transform> doors;

    private Queue<TaskBase> waitForOrdering = new Queue<TaskBase>();
    private Queue<ServeTask> dishes = new Queue<ServeTask>();
    private Queue<TaskBase> waitForServing = new Queue<TaskBase>();
    private Queue<TaskBase> waitForBilling = new Queue<TaskBase>();
    private List<ServerAI> willingers = new List<ServerAI>();
    public TaskBase curTask;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
    private void Update()
    {
        executeTimer += Time.deltaTime;
        if (executeTimer >= executeInterval)
        {
            executeTimer = 0;
            if (curTask != null)
            {
                if (willingers.Count > 0)
                {
                    //Debug.Log("AssignTask " + curTask.GetType().Name);
                    AssignTask();
                }
                else
                {
                    //Debug.Log("Task nobody does : " + curTask.GetType().Name);
                }
            }
            else if (waitForOrdering.Count > 0)
            {
                //Debug.Log("Guests are waiting");
                curTask = waitForOrdering.Peek();
            }
            else if (waitForServing.Count > 0)
            {
                //Debug.Log("Guests wait for dishes");
                curTask = waitForServing.Peek();
            }
            else if (waitForBilling.Count > 0)
            {
                //Debug.Log("Guests have finished the meal");
                curTask = waitForBilling.Peek();
            }
        }

        if (dishes.Count > 0)
        {
            cookTimer += Time.deltaTime;
            if (cookTimer >= cookInterval)
            {
                cookTimer = 0;
                ServeTask dish = dishes.Dequeue();
                EnqueueTaskBase(dish);
                //Debug.Log("Dish " + dish.dishName + " is ready");
            }
        }
    }

    public void EnqueueTaskBase(TaskBase task)
    {
        switch (task)
        {
            case OrderTask _:
                if (!waitForOrdering.Contains(task))
                    waitForOrdering.Enqueue(task);
                //else
                //    Debug.LogWarning("This orderTask is already in the queue");
                break;
            case ServeTask _:
                if (!waitForServing.Contains(task))
                    waitForServing.Enqueue(task);
                //else
                //    Debug.LogWarning("This serveTask is already in the queue");
                break;
            case BillTask _:
                if (!waitForBilling.Contains(task))
                    waitForBilling.Enqueue(task);
                //else
                //    Debug.LogWarning("This billTask is already in the queue");
                break;
        }
    }
    public bool CheckWillinger(ServerAI server)
    {
        return willingers.Contains(server);
    }
    public void AddWillinger(ServerAI server)
    {
        if (!willingers.Contains(server))
            willingers.Add(server);
    }
    public void AssignTask()
    {
        if (willingers.Count > 0)
        {
            int index = Random.Range(0, willingers.Count);
            willingers[index].AcceptTask(curTask);
            switch (curTask)
            {
                case OrderTask _:
                    if (waitForOrdering.Contains(curTask))
                        waitForOrdering.Dequeue();
                    break;
                case ServeTask _:
                    if (waitForServing.Contains(curTask))
                        waitForServing.Dequeue();
                    break;
                case BillTask _:
                    if (waitForBilling.Contains(curTask))
                        waitForBilling.Dequeue();
                    break;
            }
            curTask = null;
            willingers.Clear();
        }
    }
    public void AddOrder(List<ServeTask> orders)
    {
        foreach (ServeTask dish in orders)
        {
            dishes.Enqueue(dish);
        }
    }
    public int CheckWaitingTasksCounts()
    {
        return waitForOrdering.Count + waitForServing.Count + waitForBilling.Count;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server
{
    public Transform waitingSite;
    public Transform hangOrderSite;
    public Transform dishSite;
    public Bubble bubble;

    public Queue<TaskBase> leftTasks = new Queue<TaskBase>();
    public TaskBase curTask;

    public int willingness = 40;
    public float cleanTime = 3f;
}

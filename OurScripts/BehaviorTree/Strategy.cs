using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IStrategy
{
    Node.State Execute();

    void Reset()
    {
        // Noop
    }
}

public class ActionStrategy : IStrategy
{
    readonly Action doSomething;

    public ActionStrategy(Action doSomething)
    {
        this.doSomething = doSomething;
    }

    public Node.State Execute()
    {
        doSomething();
        return Node.State.Success;
    }
}

public class ConditionStrategy : IStrategy
{
    readonly Func<bool> predicate;

    public ConditionStrategy(Func<bool> predicate)
    {
        this.predicate = predicate;
    }

    public Node.State Execute() => predicate() ? Node.State.Success : Node.State.Failure;
}

public class ExecuteOnceStrategy : IStrategy
{
    readonly IStrategy action;
    private bool isExecuted = false;

    public ExecuteOnceStrategy(IStrategy action)
    {
        this.action = action;
    }
    public Node.State Execute()
    {
        if (!isExecuted && action.Execute() == Node.State.Success)
        {
            isExecuted = true;
            return Node.State.Success;
        }
        else
        {
            return Node.State.Failure;
        }
    }
}



public class ElderPatrolStrategy : IStrategy
{
    readonly Transform entity;
    readonly NavMeshAgent agent;
    readonly List<Transform> patrolPoints;
    readonly float patrolSpeed;
    int currentIndex;
    bool isPathCalculated;

    public ElderPatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> patrolPoints, float patrolSpeed = 2f)
    {
        this.entity = entity;
        this.agent = agent;
        this.patrolPoints = patrolPoints;
        this.patrolSpeed = patrolSpeed;
    }

    public Node.State Execute()
    {
        if (currentIndex == patrolPoints.Count) return Node.State.Success;

        var target = patrolPoints[currentIndex];
        agent.SetDestination(target.position);
        entity.LookAt(target.position.With(y: entity.position.y));

        if (isPathCalculated && agent.remainingDistance < 0.1f)
        {
            currentIndex++;
            isPathCalculated = false;
        }

        if (agent.pathPending)
        {
            isPathCalculated = true;
        }

        return Node.State.Running;
    }

    public void Reset() => currentIndex = 0;
}

//public class ElderMoveToTarget : IStrategy
//{
//    readonly Transform entity;
//    readonly NavMeshAgent agent;
//    readonly Transform target;
//    bool isPathCalculated;

//    public ElderMoveToTarget(Transform entity, NavMeshAgent agent, Transform target)
//    {
//        this.entity = entity;
//        this.agent = agent;
//        this.target = target;
//    }

//    public Node.State Execute()
//    {
//        if (Vector3.Distance(entity.position, target.position) < 1f)
//        {
//            return Node.State.Success;
//        }

//        agent.SetDestination(target.position);
//        entity.LookAt(target.position.With(y: entity.position.y));

//        if (agent.pathPending)
//        {
//            isPathCalculated = true;
//        }
//        return Node.State.Running;
//    }

//    public void Reset() => isPathCalculated = false;
//}

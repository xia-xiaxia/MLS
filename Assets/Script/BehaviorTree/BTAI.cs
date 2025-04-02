using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTAI : MonoBehaviour
{
    protected NavMeshAgent agent;

    protected float executeInterval = 1f;
    protected float timer = 0;
    
    protected Node root;
    
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    protected virtual void Update()
    {
        timer += Time.deltaTime;
        if (timer >= executeInterval)
        {
            timer = 0;
            root.Execute();
        }
    }
}

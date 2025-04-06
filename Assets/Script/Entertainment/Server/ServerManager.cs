using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : Singleton<ServerManager>
{
    public Transform Servers;
    public GameObject serverPrefab;
    public Transform Bubbles;
    public GameObject bubblePrefab;

    public Transform assignedWaitingSite;
    public Transform hangOrderSite;
    public Transform dishSite;
    public List<Transform> waitingsSitesTransform = new List<Transform>();
    public Dictionary<Transform, bool> waitingSites = new Dictionary<Transform, bool>();

    private bool isOperating;
    private float timer = 0;
    private float CheckInterval = 10f;
    private List<GameObject> servers = new List<GameObject>();



    private void Update()
    {
        if (!isOperating)
            return;
        timer += Time.deltaTime;
        if (timer >= CheckInterval)
        {
            timer = 0;
            if (RestaurantManager.Instance.CheckWaitingTasksCounts() > 3)
                AddServer();
        }
    }
    private void Start()
    {
        foreach (Transform w in waitingsSitesTransform)
            waitingSites.Add(w, false);
    }
    private void AddServer(Transform assignedWaitingSite = null)
    {
        GameObject server = Instantiate(serverPrefab, Servers);
        server.name += servers.Count;
        ServerAI serverAI = server.GetComponent<ServerAI>();
        serverAI.server.hangOrderSite = hangOrderSite;
        serverAI.server.dishSite = dishSite;
        Bubble bubble = Instantiate(bubblePrefab, Bubbles).GetComponent<Bubble>();
        bubble.owner = server.transform;
        serverAI.server.bubble = bubble;

        if (assignedWaitingSite != null)
        {
            serverAI.server.waitingSite = assignedWaitingSite;
            server.transform.position = assignedWaitingSite.position;
            if (waitingSites.ContainsKey(assignedWaitingSite))
                waitingSites[assignedWaitingSite] = true;
        }
        else
        {
            foreach (KeyValuePair<Transform, bool> w in waitingSites)
            {
                if (!w.Value)
                {
                    waitingSites[w.Key] = true;
                    serverAI.server.waitingSite = w.Key;
                    break;
                }
            }
        }

        if (serverAI.server.waitingSite == null)
        {
            Destroy(server);
            Debug.LogWarning("No More WaitingSites !!!");
        }
        else
        {
            //Debug.LogWarning("New Server");
            servers.Add(server);
        }
    }
    public void OnBeginOperation()
    {
        isOperating = true;
        AddServer(assignedWaitingSite);
    }
    public void OnEndOperation()
    {
        isOperating = false;
        foreach(var server in servers)
        {
            Destroy(server.GetComponent<ServerAI>().server.bubble.gameObject);
            Destroy(server);
        }
        servers.Clear();
    }
}

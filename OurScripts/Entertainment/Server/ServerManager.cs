using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; set; }

    public Transform Servers;
    public GameObject serverPrefab;
    public Transform Bubbles;
    public GameObject bubblePrefab;

    public Transform assignedWaitingSite;
    public Transform hangOrderSite;
    public Transform dishSite;
    public List<Transform> waitingsSitesTransform = new List<Transform>();
    public Dictionary<Transform, bool> waitingSites = new Dictionary<Transform, bool>();

    private float timer = 0;
    private float CheckInterval = 10f;
    private List<GameObject> servers = new List<GameObject>();



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
        AddServer(assignedWaitingSite);
    }
    public void AddServer(Transform assignedWaitingSite = null)
    {
        GameObject server = Instantiate(serverPrefab, Servers);
        ServerAI serverAI = server.GetComponent<ServerAI>();
        serverAI.hangOrderSite = hangOrderSite;
        serverAI.dishSite = dishSite;
        Bubble bubble = Instantiate(bubblePrefab, Bubbles).GetComponent<Bubble>();
        //bubble..server = server.transform;
        serverAI.bubble = bubble;

        if (assignedWaitingSite != null)
        {
            serverAI.waitingSite = assignedWaitingSite;
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
                    serverAI.waitingSite = w.Key;
                    break;
                }
            }
        }

        if (serverAI.waitingSite == null)
        {
            Destroy(server);
            //Debug.LogWarning("No More WaitingSites !!!");
        }
        else
            servers.Add(server);
    }
    private void AssignBubble(ServerAI server)
    {

    }
}

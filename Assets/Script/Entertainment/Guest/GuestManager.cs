using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GuestManager : MonoBehaviour
{
    public static GuestManager Instance { get; set; }

    public Transform Guests;
    public GameObject guestPrefab;
    public Transform Bubbles;
    public GameObject bubblePrefab;
    public Menu menu;

    private float timer = 10f;
    private float newGuestInterval = 10f;
    private List<GameObject> guests = new List<GameObject>();
    private int guestCount;



    private void Awake()
    { 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        MissionManager.Instance.RegisterMission("guestCount", guestCount);
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= newGuestInterval)
        {
            timer = 0;
            if (guests.Count < SeatManager.Instance.CheckEmptySeatCounts().Count)
            {
                AddGuest();
            }
            else
            {
                //Debug.LogWarning("No Empty Seat !!!");
            }
        }
    }
    public void AddGuest()
    {
        MissionManager.Instance.UpdateValue("guestCount", ++guestCount);
        GameObject guest = Instantiate(guestPrefab, Guests);
        GuestAI guestAI = guest.GetComponent<GuestAI>();
        Bubble bubble = Instantiate(bubblePrefab, Bubbles).GetComponent<Bubble>();
        bubble.owner = guest.transform;
        guestAI.guest.bubble = bubble;
        guestAI.guest.UpdateState(GuestState.GetIn);
        guests.Add(guest);
    }
    public void DestroyGuest(GameObject guest)
    {
        Destroy(guest.GetComponent<GuestAI>().guest.bubble.gameObject);
        Destroy(guest);
        if (guests.Contains(guest))
        {
            guests.Remove(guest);
        }
    }
}

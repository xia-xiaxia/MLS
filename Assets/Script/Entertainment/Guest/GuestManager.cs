using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestManager : MonoBehaviour
{
    public static GuestManager Instance { get; set; }

    public Transform Guests;
    public GameObject guestPrefab;

    private float timer = 10f;
    private float newGuestInterval = 10f;
    private List<GameObject> guests = new List<GameObject>();



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
        if (timer >= newGuestInterval)
        {
            timer = 0;
            AddGuest();
        }
    }
    public void AddGuest()
    {
        if (guests.Count < SeatManager.Instance.CheckEmptySeatCounts().Count)
        {
            guests.Add(Instantiate(guestPrefab, Guests));
        }
        else
        {
            //Debug.LogWarning("No Empty Seat !!!");
        }
    }
    public void DestroyGuest(GameObject guest)
    {
        Destroy(guest);
        if (guests.Contains(guest))
        {
            guests.Remove(guest);
        }
    }
}

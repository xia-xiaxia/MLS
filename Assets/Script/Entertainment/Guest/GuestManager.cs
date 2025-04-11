using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Table;

public class GuestManager : Singleton<GuestManager>
{
    public Transform Guests;
    public GameObject guestPrefab;
    public Transform Bubbles;
    public GameObject bubblePrefab;
    public Menu menu;

    private bool isOperating;
    private List<GameObject> guests = new List<GameObject>();
    private int guestCount; // 任务需求
    private float timer;
    private float newGuestsInterval = 25f;
    private float batchInterval = 0.5f;



    private void Start()
    {
        MissionManager.Instance.RegisterMission("guestCount", guestCount);
        timer = newGuestsInterval;
    }
    private void Update()
    {
        if (!isOperating)
            return;
        timer += Time.deltaTime;
        if (timer >= newGuestsInterval)
        {
            timer = 0;
            //if (guests.Count < SeatManager.Instance.CheckEmptySeatCounts().Count)
            //{
            //    AddGuest();
            //}
            Table table = TableManager.Instance.OccupyTable();
            if (table != null)
            {
                StartCoroutine(BatchOfGuests(table));
            }
        }
    }
    public Guest AddGuest(bool isOrderer, Table table, int seat)
    {
        MissionManager.Instance.UpdateValue("guestCount", ++guestCount);//任务系统所需

        GameObject guestObj = Instantiate(guestPrefab, Guests);
        Guest guest = guestObj.GetComponent<GuestAI>().guest;

        Bubble bubble = Instantiate(bubblePrefab, Bubbles).GetComponent<Bubble>();
        bubble.owner = guestObj.transform;
        guest.bubble = bubble;
        guest.isOrderer = isOrderer;
        guest.tableIndex = table.tableIndex;
        if (seat < table.seats.Count)
            guest.seatIndex = table.seats[seat].seatIndex;
        guest.seatDir = SeatManager.Instance.CheckSeatDir(guest.seatIndex);

        guest.UpdateState(GuestState.GetIn);
        guests.Add(guestObj);
        return guest;
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
    public void OnBeginReceivingGuests()
    {
        isOperating = true;
    }
    public void OnEndReceivingGuests()
    {
        isOperating = false;
    }
    private IEnumerator BatchOfGuests(Table table)
    {
        int guestCount = 0;
        int dishCount = 0;
        switch (table.tableSize)
        {
            case TableSize.S:
                guestCount = UnityEngine.Random.Range(1, 2); // 小桌1-2人
                dishCount = UnityEngine.Random.Range(1, Mathf.Min(2, GuestManager.Instance.menu.recipes.Count) + 1); // 1-2道菜
                break;
            case TableSize.M:
                guestCount = UnityEngine.Random.Range(3, 5); // 中桌3-4人
                dishCount = UnityEngine.Random.Range(2, Mathf.Min(5, GuestManager.Instance.menu.recipes.Count) + 1); // 2-5道菜
                break;
            case TableSize.L:
                guestCount = UnityEngine.Random.Range(5, 7); // 大桌5-6人
                dishCount = UnityEngine.Random.Range(4, Mathf.Min(6, GuestManager.Instance.menu.recipes.Count) + 1); // 4-6道菜
                break;
        }
        Debug.Log(guestCount);
        table.seats.Shuffle();
        List<Guest> accompanyings = new List<Guest>();
        Guest orderer = AddGuest(true, table, guestCount - 1);
        orderer.dishCount = dishCount;
        while (--guestCount > 0)
        {
            Debug.Log(guestCount);
            yield return new WaitForSeconds(batchInterval);
            accompanyings.Add(AddGuest(false, table, guestCount - 1));
        }
        orderer.accompanyings = accompanyings;
    }
}

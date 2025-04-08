using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GuestManager : Singleton<GuestManager>
{
    public Transform Guests;
    public GameObject guestPrefab;
    public Transform Bubbles;
    public GameObject bubblePrefab;
    public Menu menu;

    private bool isOperating;
    private float timer = 10f;
    private float newGuestInterval = 10f;
    private List<GameObject> guests = new List<GameObject>();
    private int guestCount;



    private void Start()
    {
        MissionManager.Instance.RegisterMission("guestCount", guestCount);
    }
    private void Update()
    {
        if (!isOperating)
            return;
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
    public void AddGuest() //��Ҫ��Ϊ��ÿ�����n���˿ͣ�����ֱ��������λ�����ɹ˿͸ĳ�һ��ʱ���һ���˿ͣ�
    {
        MissionManager.Instance.UpdateValue("guestCount", ++guestCount);//����ϵͳ����

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
    public void OnBeginReceivingGuests()
    {
        isOperating = true;
    }
    public void OnEndReceivingGuests()
    {
        isOperating = false;
    }
}

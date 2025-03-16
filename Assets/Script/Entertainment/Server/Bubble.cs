using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
    public Sprite order;
    public Sprite serve;
    public Sprite clean;
    public Transform server;
    private Image bubble;

    private Vector3 offset = new Vector3(0.4f, 0.55f, 0.36f);



    private void Start()
    {
        bubble = GetComponent<Image>(); 
        bubble.enabled = false;
    }
    private void Update()
    {
        if (bubble.enabled)
        {
            transform.position = server.position + offset;
        }
    }
    public void Hide()
    {
        bubble.enabled = false;
    }
    public void UpdateState(string state)
    {
        bubble.enabled = true;
        switch (state)
        {
            case "order":
                bubble.sprite = order;
                break;
            case "serve":
                bubble.sprite = serve;
                break;
            case "clean":
                bubble.sprite = clean;
                break;
            default:
                Debug.LogError("Invalid state");
                break;
        }
    }
}

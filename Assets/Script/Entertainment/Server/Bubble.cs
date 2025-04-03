using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
    public Sprite order;
    public Sprite serve;
    public Sprite clean;
    public Transform owner;
    private Image bubble;
    private TextMeshProUGUI text;

    private Vector3 offset = new Vector3(0.4f, 0.55f, 0.36f);
    //private Vector3 offset = new Vector3(0, 0.6f, 0);



    private void Start()
    {
        bubble = GetComponent<Image>(); 
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        if (text.enabled || bubble.enabled)
        {
            transform.position = owner.position + offset;
        }
    }
    public void Hide()
    {
        bubble.enabled = false;
        text.enabled = false;
    }
    public void UpdateState(string state)
    {
        if (text == null)
            text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        text.enabled = true;
        text.text = state;

        //bubble.enabled = true;
        //switch (state)
        //{
        //    case "order":
        //        bubble.sprite = order;
        //        break;
        //    case "serve":
        //        bubble.sprite = serve;
        //        break;
        //    case "clean":
        //        bubble.sprite = clean;
        //        break;
        //    default:
        //        Debug.LogError("Invalid state");
        //        break;
        //}
    }
}

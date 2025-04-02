using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class NewbieGuide : MonoBehaviour
{
    //public Canvas canvas;//��������������ֱ���� NewbieGuideManager �й��� Canvas��ֻ�õ�һ�� Canvas��
    public new string name;
    public string guide;
    public int priority;
    [HideInInspector]
    public RectTransform target;

    private bool isExecuted = false;

    //private void Awake()
    //{
    //    NewbieGuideManager.Instance.Register(this);
    //}
    private void Start()
    {
        target = GetComponent<RectTransform>();
        if (!isExecuted)
            StartCoroutine(TryShowGuide());
    }
    private IEnumerator TryShowGuide()
    {
        while (!NewbieGuideManager.Instance.TryShowGuide(this))
        {
            Debug.Log("Defeat " + name);
            yield return null;
            //yield return new WaitForSeconds(0.1f);
        }
        //isExecuted = true;
    }
    public void Failed()
    {
        StartCoroutine(TryShowGuide());
    }
    public void FinishGuide()
    {
        isExecuted = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HangoutFlow : MonoBehaviour
{
    [System.Serializable]
    public class FlowPath
    {
        public string flowName;
        public Transform[] waypoints;
        public float waitTimeAtPoint = 0f;
    }

    public FlowPath firstWalk;
    public FlowPath secondWalk;
    public float moveSpeed = 3f;

    private Queue<FlowPath> taskQueue = new Queue<FlowPath>();
    private bool isExecuting = false;
    private bool isAnyCloseButtonClicked = false;

    public Button closeButton1;
    public Button startButton;//go to the street
    public GameObject uiPanel;//�ײ���closebutton
    public QuickPurchase quickPurchase;

    void Start()
    {
        uiPanel.SetActive(false);
        ResetTaskQueue(); //����
        startButton.onClick.AddListener(StartFlow);
       
    }

    public void StartFlow()
    {
        if (!isExecuting)  //���startbutton������startbutton
        {
            startButton.gameObject.SetActive(false); 
            ResetTaskQueue(); 
            StartNextTask(); 
        }
    }

    private IEnumerator ExecuteFlow(FlowPath flow)
    {
        isExecuting = true;
        Debug.Log($"��ʼ���̣�{flow.flowName}");

        foreach (var waypoint in flow.waypoints)
        {
            yield return MoveToWaypoint(waypoint.position);
            yield return new WaitForSeconds(flow.waitTimeAtPoint);
        }

        isExecuting = false;

        if (flow == firstWalk)
        {
            
            Debug.Log("��һ�׶�firstwalk��������ʼ��ֳ鿨");

            quickPurchase.EnablePurchaseButtons();
            uiPanel.SetActive(false);
        //ʵʱ����closebutton�ļ���״̬
            closeButton1.onClick.RemoveListener(OnCloseButtonClicked);
            closeButton1.onClick.AddListener(OnCloseButtonClicked); 
        }
        else if (flow == secondWalk)
        {
          
            Debug.Log("�ڶ��׶ν��������̻�shop");
            startButton.gameObject.SetActive(true); 
            startButton.interactable = true; //�ɽ���

         //ʵʱ����startbutton�ļ���״̬
            startButton.onClick.RemoveListener(StartFlow); 
            startButton.onClick.AddListener(StartFlow);  
        }
    }

    public void OnCloseButtonClicked()
    {
        if (!isAnyCloseButtonClicked)
        {
            isAnyCloseButtonClicked = true;
            closeButton1.gameObject.SetActive(false);
            StartCoroutine(StartNextTaskWithDelay());
        }
    }

    private IEnumerator StartNextTaskWithDelay()
    {
        Debug.Log("��ҹرճ鿨 UI����ʼ����");
        yield return new WaitForSeconds(1f);  
        isAnyCloseButtonClicked = false; 
        StartNextTask();  
    }

    private void StartNextTask()
    {
        if (taskQueue.Count > 0)
        {
            StartCoroutine(ExecuteFlow(taskQueue.Dequeue()));
        }
        else
        {
          
            Debug.Log("�������̽��������¿�ʼ");
            ResetTaskQueue();
            startButton.gameObject.SetActive(true); 
            startButton.interactable = true; 
        }
    }

    private void ResetTaskQueue()
    {
        taskQueue.Clear();
        taskQueue.Enqueue(firstWalk);  
        taskQueue.Enqueue(secondWalk); 
    }

    private IEnumerator MoveToWaypoint(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}

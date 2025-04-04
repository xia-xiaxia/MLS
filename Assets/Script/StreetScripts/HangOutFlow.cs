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
    public float moveSpeed = 3f;

    private bool isExecuting = false;
    private bool hasSelectedMode = false;
    private bool isBigPurchaseMode = false;

    public Button startButton;
    public Button bigPurchaseButton;
    public Button quickPurchaseButton;
    public Button confirmButton;
    public Button closeButton;
    public GameObject uiPanel;

    public QuickPurchase quickPurchase;
    public AnimationController animationController;

    void Start()
    {
        hasSelectedMode = false;
        animationController = GetComponent<AnimationController>();
        animationController.SetIdleAnimation();
        uiPanel.SetActive(false);   
        startButton.onClick.AddListener(StartFirstStep);
        quickPurchaseButton.onClick.AddListener(() => SelectPurchaseMode(false));
        bigPurchaseButton.onClick.AddListener(() => SelectPurchaseMode(true));
        confirmButton.onClick.AddListener(ConfirmPurchase);
        closeButton.onClick.AddListener(CloseUI);

        quickPurchaseButton.gameObject.SetActive(false);
        bigPurchaseButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    private void StartFirstStep()
    {
        Debug.Log("开始逛街走到特定地方");
        startButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        StartCoroutine(ExecuteFlow(firstWalk));
      
    }

    private void ShowPurchaseButtons()
    {
        quickPurchaseButton.gameObject.SetActive(true);
        bigPurchaseButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        ResetButtons(); 
    }

    private IEnumerator ExecuteFlow(FlowPath flow)
    {
        isExecuting = true;
        Debug.Log($"开始流程：{flow.flowName}");

        foreach (var waypoint in flow.waypoints)
        {
            yield return MoveToWaypoint(waypoint.position);
            yield return new WaitForSeconds(flow.waitTimeAtPoint);
        }

        isExecuting = false;
        animationController.SetIdleAnimation();
        Debug.Log("开始抽卡");
        ShowPurchaseButtons();
    }

    private void SelectPurchaseMode(bool isBig)
    {
  

        isBigPurchaseMode = isBig;
        //hasSelectedMode = true;  
      
        quickPurchaseButton.image.color = isBig ? Color.white :Color.cyan;
        bigPurchaseButton.image.color = isBig ? Color.yellow: Color.white;
        confirmButton.gameObject.SetActive(true);
    }

    private void ConfirmPurchase()
    {
        if (!hasSelectedMode)
        {
           // Debug.Log("至少选择一个抽卡模式");
            return;
        }

        hasSelectedMode = true;
        quickPurchaseButton.interactable = false;
        bigPurchaseButton.interactable = false;
        confirmButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(true);  

    }

    private void CloseUI()
    {
        ResetButtons();
        ShowPurchaseButtons(); 
    }
    //reset 两个选购按钮，confirm按钮，close按钮
    private void ResetButtons()
    {
        quickPurchaseButton.image.color = Color.white;
        bigPurchaseButton.image.color = Color.white;
        quickPurchaseButton.interactable = true;
        bigPurchaseButton.interactable = true;
        hasSelectedMode = false;
        confirmButton.gameObject.SetActive(true);
    }

    private IEnumerator MoveToWaypoint(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            Vector3 moveDirection = (target - transform.position).normalized;
            float currentAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            animationController.UpdateAnimation(moveDirection, currentAngle);
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NewbieGuideUIManager : Singleton<NewbieGuideUIManager>, ICanvasRaycastFilter
{
    //每次打开一个界面，如果是第一次，会自动展示新手引导界面
    //如果一个界面有多个新手引导，会依次展示

    //自身组件
    private Image mat;
    private Material material;
    private TextMeshProUGUI guide;
    private TextMeshProUGUI skipNote;
    //引用组件
    public Canvas canvas;
    private RectTransform target;
    public RectTransform skipButton;
    public GameObject skipConfirm;

    private bool isScaling = false;
    private float radius;
    private float scale = 2;
    private float timer = 0;
    private float time = 1;
    private bool isSkipConfirmOn = false;



    protected override void Awake()
    {
        base.Awake();
        mat = GetComponent<Image>();
        material = mat.material;
        guide = transform.Find("Guide").GetComponent<TextMeshProUGUI>();
        skipNote = transform.Find("SkipNote").GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        if (isScaling)
        {
            timer += Time.deltaTime * 1 / time;
            material.SetFloat("_Slider", Mathf.Lerp(radius * scale, radius, timer));
            if (timer >= 1)
            {
                timer = 0;
                isScaling = false;
            }
        }
    }
    public void UpdateGuideUI(NewbieGuide guide)
    {
        EnableUI();
        this.target = guide.target;
        this.guide.text = guide.guide;
        LocateCircle(guide.target);
    }
    public void LocateCircle(RectTransform target)
    {
        this.target = target;
        float z = target.eulerAngles.z;
        target.eulerAngles = new Vector3(target.eulerAngles.x, target.eulerAngles.y, 0);

        Vector3[] targetCorners = new Vector3[4];//获取目标的中心点
        target.GetWorldCorners(targetCorners);//获取目标的四个角的 世界坐标
        for (int i = 0; i < 4; i++)
            targetCorners[i] = WorldToScreenPoint(targetCorners[i]);
        Vector3 center = new Vector3();
        center = (targetCorners[0] + targetCorners[1] + targetCorners[2] + targetCorners[3]) / 4;//计算中心点

        material.SetVector("_Center", center); //设置材质的中心点

        float width = targetCorners[3].x - targetCorners[0].x; //计算宽度
        float height = targetCorners[1].y - targetCorners[0].y; //计算高度
        radius = Mathf.Sqrt(width * width + height * height) / 2;//计算半径

        material.SetFloat("_Slider", radius * scale); //设置材质的半径
        isScaling = true;

        target.eulerAngles = new Vector3(target.eulerAngles.x, target.eulerAngles.y, z);
    }
    public Vector2 WorldToScreenPoint(Vector3 world)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, world);//把世界坐标转换为屏幕坐标
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), world, canvas.worldCamera, out localPoint);//把屏幕坐标转成局部坐标
        return localPoint;
    }
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) //处理事件渗透 //注意这个方法不止点击时会调用，所以一帧内可能会调用多次
    {
        if (target == null || !mat.enabled || !skipNote.enabled)
            return false;// 如果没有目标，则可以射线穿透，事件渗透，return false to block raycast
        if (Input.GetMouseButtonDown(0))
        {
            if (!isSkipConfirmOn) // 如果没打开确认框，就判断是否按在了跳过键
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(skipButton, sp)) // 按到了跳过键，就不遮挡
                {
                    skipConfirm.SetActive(true);
                    isSkipConfirmOn = true;
                }
                else // 没有打开确认框，且没点击跳过键，则完成当前引导，并且
                {
                    //StartCoroutine(NewbieGuideManager.Instance.FinishCurGuide());
                    NewbieGuideManager.Instance.FinishCurGuide();
                    StartCoroutine(DelayDisableUI());
                }
            }
            else // 如果打开了确认框，就用确认框内的按钮事件
            {
                return true;
            }
        }
        return !RectTransformUtility.RectangleContainsScreenPoint(target, sp); //检查射线是否在目标矩形内
    }
    public void EnableUI()
    {
        skipButton.gameObject.SetActive(true);
        isSkipConfirmOn = false;
        mat.enabled = true;
        skipNote.enabled = true;
        guide.enabled = true;
    }
    public IEnumerator DelayDisableUI()
    {
        yield return null;
        skipButton.gameObject.SetActive(false);
        skipConfirm.SetActive(false);
        isSkipConfirmOn = false;
        isScaling = false;
        target = null;
        mat.enabled = false;
        skipNote.enabled = false;
        guide.enabled = false;
    }

    public void OnSkipYButtonClicked()
    {
        NewbieGuideManager.Instance.FinishCurGuide();
        NewbieGuideManager.Instance.SkipGuide();
        StartCoroutine(DelayDisableUI());
    }
    public void OnSkipNButtonClicked()
    {
        isSkipConfirmOn = false;
        skipConfirm.SetActive(false);
    }
}

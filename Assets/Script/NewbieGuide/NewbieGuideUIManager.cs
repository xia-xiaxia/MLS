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
    //ÿ�δ�һ�����棬����ǵ�һ�Σ����Զ�չʾ������������
    //���һ�������ж������������������չʾ

    //�������
    private Image mat;
    private Material material;
    private TextMeshProUGUI guide;
    private TextMeshProUGUI skip;
    //�������
    public Canvas canvas;
    private RectTransform target;

    private bool isScaling = false;
    private float radius;
    private float scale = 2;
    private float timer = 0;
    private float time = 1;



    protected override void Awake()
    {
        base.Awake();
        mat = GetComponent<Image>();
        material = mat.material;
        guide = transform.Find("Guide").GetComponent<TextMeshProUGUI>();
        skip = transform.Find("Skip").GetComponent<TextMeshProUGUI>();
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

        Vector3[] targetCorners = new Vector3[4];//��ȡĿ������ĵ�
        target.GetWorldCorners(targetCorners);//��ȡĿ����ĸ��ǵ� ��������
        for (int i = 0; i < 4; i++)
            targetCorners[i] = WorldToScreenPoint(targetCorners[i]);
        Vector3 center = new Vector3();
        center = (targetCorners[0] + targetCorners[1] + targetCorners[2] + targetCorners[3]) / 4;//�������ĵ�

        material.SetVector("_Center", center); //���ò��ʵ����ĵ�

        float width = targetCorners[3].x - targetCorners[0].x; //������
        float height = targetCorners[1].y - targetCorners[0].y; //����߶�
        radius = Mathf.Sqrt(width * width + height * height) / 2;//����뾶

        material.SetFloat("_Slider", radius * scale); //���ò��ʵİ뾶
        isScaling = true;

        target.eulerAngles = new Vector3(target.eulerAngles.x, target.eulerAngles.y, z);
    }
    public Vector2 WorldToScreenPoint(Vector3 world)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, world);//����������ת��Ϊ��Ļ����
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), world, canvas.worldCamera, out localPoint);//����Ļ����ת�ɾֲ�����
        return localPoint;
    }
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) //�����¼���͸
    {
        if (target == null || !mat.enabled || !skip.enabled)
            return false;// ���û��Ŀ�꣬��������ߴ�͸���¼���͸��return false to block raycast
        if (Input.GetMouseButtonDown(0))
        {
            //StartCoroutine(NewbieGuideManager.Instance.FinishCurGuide());
            NewbieGuideManager.Instance.FinishCurGuide();
            StartCoroutine(DelayDisableUI());
        }
        return !RectTransformUtility.RectangleContainsScreenPoint(target, sp); //��������Ƿ���Ŀ�������
    }
    public void EnableUI()
    {
        mat.enabled = true;
        skip.enabled = true;
        guide.enabled = true;
    }
    public IEnumerator DelayDisableUI()
    {
        yield return null;
        isScaling = false;
        target = null;
        mat.enabled = false;
        skip.enabled = false;
        guide.enabled = false;
    }
}

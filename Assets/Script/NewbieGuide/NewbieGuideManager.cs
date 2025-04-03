using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewbieGuideManager : Singleton<NewbieGuideManager>
{
    private Dictionary<string, bool> guideStates = new Dictionary<string, bool>();//�ظ����س����ᵼ��TryShowGuide�ظ����ã�������Ҫ�ֵ�
    private NewbieGuide curGuide;

    private bool isSelecting = false;
    private float timer = 0;
    private float time = 0.3f;



    //����ͨ�����ȼ���������һ��UIչʾ
    private void Update()
    {
        if (isSelecting)
        {
            timer += Time.deltaTime;
            if (timer >= time)
            {
                timer = 0;
                isSelecting = false;
                if (curGuide != null)
                    NewbieGuideUIManager.Instance.UpdateGuideUI(curGuide);
            }
        }
    }
    public bool TryShowGuide(NewbieGuide guide)
    {
        //���û�еĻ������
        if (!guideStates.ContainsKey(guide.name))
            guideStates.Add(guide.name, false);
        //����Ѿ�����˾Ͳ�չʾ��
        if (guideStates[guide.name])
            return true;
        //���û������ҵ�ǰû�о�չʾ
        else if (curGuide == null)
        {
            curGuide = guide;
            //NewbieGuideUIManager.Instance.UpdateGuideUI(curGuide);
            isSelecting = true;
            return true;
        }
        //�����ǰ���˾Ͳ�չʾ
        else
        {
            if (guide.priority > curGuide.priority)
            {
                curGuide.Failed();
                curGuide = guide;
                return true;
            }
            else
                return false;
        }
    }
    public void FinishCurGuide()
    {
        if (curGuide != null)
        {
            curGuide.FinishGuide();
            curGuide = null;
        }
    }
    //public IEnumerator FinishCurGuide()
    //{
    //    //yield return new WaitForSeconds(0.2f);
    //    yield return null;
    //    if (curGuide != null)
    //    {
    //        curGuide.FinishGuide();
    //        curGuide = null;
    //    }
    //}
}

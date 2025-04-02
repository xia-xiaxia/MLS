using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewbieGuideManager : Singleton<NewbieGuideManager>
{
    private Dictionary<string, bool> guideStates = new Dictionary<string, bool>();//重复加载场景会导致TryShowGuide重复调用，所以需要字典
    private NewbieGuide curGuide;

    private bool isSelecting = false;
    private float timer = 0;
    private float time = 0.3f;



    //可以通过优先级决定是哪一个UI展示
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
        //如果没有的话先添加
        if (!guideStates.ContainsKey(guide.name))
            guideStates.Add(guide.name, false);
        //如果已经完成了就不展示了
        if (guideStates[guide.name])
            return true;
        //如果没有完成且当前没有就展示
        else if (curGuide == null)
        {
            curGuide = guide;
            //NewbieGuideUIManager.Instance.UpdateGuideUI(curGuide);
            isSelecting = true;
            return true;
        }
        //如果当前有了就不展示
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

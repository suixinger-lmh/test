using DG.Tweening;
using FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftUIPanel : UIPanel
{
    /// <summary>
    /// 多级判断依赖打开和关闭次数，如果多次无效执行打开关闭会造成异常 TODO:打开/关闭的面板才能关闭/打开 避免多次执行
    /// </summary>
    public static int hasOnePanelIndex = 0;

    Tween openTw;
    public override void OpenUIPanel()
    {
        if (isActive) return;
        isActive = true;

        //以及存在打开的面板，往右延长
        if (hasOnePanelIndex == 1)
        {
            if (openTw != null)
                openTw.Kill(true);
            hasOnePanelIndex++;
            base.OpenUIPanel();
            openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(361, 0), 1f);
        }
        else
        {
            if (openTw != null)
                openTw.Kill(true);
            hasOnePanelIndex++;
            base.OpenUIPanel();
            openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(120, 0), 1f);

        }

      //  Debug.Log(hasOnePanelIndex);
    }

    public override void CloseUIPanel()
    {
        if (!isActive) return;
        isActive = false;
        if (openTw != null)
            openTw.Kill(false);//关闭的时候不需要上个完成，当前位置关闭
        hasOnePanelIndex--;
        if (hasOnePanelIndex < 0)
            hasOnePanelIndex = 0;
       // Debug.Log(hasOnePanelIndex);
        openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-120, 0), 1f).OnComplete(() =>
        {
          
            AfterCloseTweenDo();
            base.CloseUIPanel();
      
        });
    }

    public virtual void AfterCloseTweenDo()
    {

    }
}

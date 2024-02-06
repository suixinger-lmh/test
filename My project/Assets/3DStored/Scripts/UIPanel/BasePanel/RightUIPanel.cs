using DG.Tweening;
using FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightUIPanel : UIPanel
{
    public static int hasOnePanelIndex = 0;
    Tween openTw;
    public override void OpenUIPanel()
    {
        if (isActive) return;
        isActive = true;
        if (hasOnePanelIndex == 1)
        {
            if (openTw != null)
                openTw.Kill(true);
            hasOnePanelIndex++;
            base.OpenUIPanel();
            openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-361, 0), 1f);
        }
        else
        {
            if (openTw != null)
                openTw.Kill(true);
            hasOnePanelIndex++;
            base.OpenUIPanel();
            openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-120, 0), 1f);

        }
        //if (openTw != null)
        //    openTw.Kill(true);
        //base.OpenUIPanel();
        //openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-120, 0), 1f);
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
        openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(120, 0), 1f).OnComplete(() =>
        {

            AfterCloseTweenDo();
            base.CloseUIPanel();

        });
        //if (openTw != null)
        //    openTw.Kill(false);//关闭的时候不需要上个完成，当前位置关闭
        //openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(120, 0), 1f).OnComplete(() =>
        //{
        //    AfterCloseTweenDo();
        //    base.CloseUIPanel();
        //});
    }

    public virtual void AfterCloseTweenDo()
    {

    }
}

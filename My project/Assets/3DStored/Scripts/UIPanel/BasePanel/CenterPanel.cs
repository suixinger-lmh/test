using DG.Tweening;
using FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPanel : UIPanel
{
    Tween openTw;
    public override void OpenUIPanel()
    {
        if (openTw != null)
            openTw.Kill(true);
        base.OpenUIPanel();
        openTw = transform.GetChild(0).GetComponent<RectTransform>().DOScale(1, 1f);
    }
    public override void CloseUIPanel()
    {
        if (openTw != null)
            openTw.Kill(false);//关闭的时候不需要上个完成，当前位置关闭
        openTw = transform.GetChild(0).GetComponent<RectTransform>().DOScale(0, 1f).OnComplete(() =>
        {
            AfterCloseTweenDo();
            base.CloseUIPanel();
        });
    }

    public virtual void AfterCloseTweenDo()
    {

    }
}

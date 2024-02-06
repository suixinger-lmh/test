using DG.Tweening;
using FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftUIPanel : UIPanel
{
    /// <summary>
    /// �༶�ж������򿪺͹رմ�������������Чִ�д򿪹رջ�����쳣 TODO:��/�رյ������ܹر�/�� ������ִ��
    /// </summary>
    public static int hasOnePanelIndex = 0;

    Tween openTw;
    public override void OpenUIPanel()
    {
        if (isActive) return;
        isActive = true;

        //�Լ����ڴ򿪵���壬�����ӳ�
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
            openTw.Kill(false);//�رյ�ʱ����Ҫ�ϸ���ɣ���ǰλ�ùر�
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

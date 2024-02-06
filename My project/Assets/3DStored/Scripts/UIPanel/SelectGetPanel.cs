using DG.Tweening;
using FrameWork;
using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectGetPanel : LeftUIPanel
{
    public Text title;
    public Button exitBtn;

    GameObject selectObj;

    UnityAction onCancelAction;
    UnityAction<GameObject> onSelectAction;


    //选中依赖两个组件 拖拽和列表
    DragManager _dragmanager;

    public void InitSelectGetPanel(DragManager dm)
    {
        _dragmanager = dm;
    }

    protected override void Start()
    {
        base.Start();
       
        //取消选择
        exitBtn.onClick.AddListener(() => {
            ResetPanel();
            if (onCancelAction != null)
                onCancelAction();
        });

        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.SelectGetObj, ReceiveListSelectObj);
    }

    //列表选中后执行
    void ReceiveListSelectObj(CoreEvent ce)
    {
        GameObject obj = (GameObject)ce.EventParam;
        DoSelect(obj);
    }





    void DoSelect(GameObject obj)
    {
        ResetPanel();
        if (onSelectAction != null)
            onSelectAction(obj);
     
    }
    void ResetPanel()
    {
        //启用列表选择
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("关闭", null)));
        _dragmanager.SetDragManager(false);//关闭
        //_dragmanager.ChangeDrayType(DragManager.DragType.None);
        startSelect = false;
        //onCancelAction = null;
        //onSelectAction = null;
        CloseUIPanel();
    }

    public void OpenSelectPanelType(OpPanelparam op,UnityAction oncancel = null,UnityAction<GameObject> onselect =null)
    {
        switch (op.tag)
        {
            case "仓库":
                title.text = "选择仓库：";
                //启用拖拽选择
                _dragmanager.SetDragManager(true, CommonHelper.Factory, CommonHelper.Floor, DragManager.DragType.FindOnce);
                //_dragmanager.ChangeDrayType(DragManager.DragType.Find);
                startSelect = true;
                OpenUIPanel();
                break;
            case "区域":
                title.text = "选择区域：";
                //启用列表选择
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("区域选择", op.param)));
                OpenUIPanel();
                break;
        }

        onCancelAction = oncancel;
        onSelectAction = onselect;
    }

    bool startSelect = false;

    private void Update()
    {
        if (startSelect)
        {
            if (_dragmanager != null)
            {
                if (_dragmanager.GetSelectObj() != null)
                {
                    DoSelect(_dragmanager.GetSelectObj());
                }
            }
        }
    }

}

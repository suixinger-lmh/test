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


    //ѡ������������� ��ק���б�
    DragManager _dragmanager;

    public void InitSelectGetPanel(DragManager dm)
    {
        _dragmanager = dm;
    }

    protected override void Start()
    {
        base.Start();
       
        //ȡ��ѡ��
        exitBtn.onClick.AddListener(() => {
            ResetPanel();
            if (onCancelAction != null)
                onCancelAction();
        });

        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.SelectGetObj, ReceiveListSelectObj);
    }

    //�б�ѡ�к�ִ��
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
        //�����б�ѡ��
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("�ر�", null)));
        _dragmanager.SetDragManager(false);//�ر�
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
            case "�ֿ�":
                title.text = "ѡ��ֿ⣺";
                //������קѡ��
                _dragmanager.SetDragManager(true, CommonHelper.Factory, CommonHelper.Floor, DragManager.DragType.FindOnce);
                //_dragmanager.ChangeDrayType(DragManager.DragType.Find);
                startSelect = true;
                OpenUIPanel();
                break;
            case "����":
                title.text = "ѡ������";
                //�����б�ѡ��
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("����ѡ��", op.param)));
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

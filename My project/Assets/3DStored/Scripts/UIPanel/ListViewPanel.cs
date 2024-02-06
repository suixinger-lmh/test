using DG.Tweening;
using FrameWork;
using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListViewPanel : UIPanel
{

    protected override void Start()
    {
        base.Start();
        if (itemPrefab == null)
        {
            itemPrefab = Resources.Load("UIItemPrefab/arealistItem") as GameObject;
        }
        
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ViewListCall, RecreateViewListEvent);
    }

    //protected override void OnDestroy()
    //{
    //    base.OnDestroy();
    //    Manager3DStored.Instance.GetStoredComponent<EventComponent>().RemoveEventListener(CoreEventId.ViewListCall, RecreateViewListEvent);
    //}
    Tween openTw;
    public override void OpenUIPanel()
    {
        if (openTw != null)
            openTw.Kill(true);
        base.OpenUIPanel();
        openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-143.345f, -175f), 1f);
    }

    public override void CloseUIPanel()
    {
        _factoryObj = null;

        if (openTw != null)
            openTw.Kill(false);//�رյ�ʱ����Ҫ�ϸ���ɣ���ǰλ�ùر�
        openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-143.345f, 315), 1f).OnComplete(() =>
        {
            base.CloseUIPanel();
        });
    }






    void RecreateViewListEvent(CoreEvent ce)
    {
        OpPanelparam op = (OpPanelparam)ce.EventParam;

        switch (op.tag)
        {

            case "�ر�":
                CloseUIPanel();
                break;

            //���ɵ�ǰ�ֿ�������б�
            case "����༭":
                title.text = "����༭";
                _factoryObj = (GameObject)op.param;
                CreateList_Editor();
                OpenUIPanel();
                break;
            case "����༭_ˢ��":
                _factoryObj = (GameObject)op.param;
                CreateList_Editor();
                break;
            case "����ѡ��":
                title.text = "����ѡ��";
                _factoryObj = (GameObject)op.param;
                CreateList_Select();
                OpenUIPanel();
                break;

            case "�ֿ�ɾ��":
                facRoot = (Transform)op.param;
                title.text = "�ֿ�ɾ��";
                CreateList(itemPrefab, facRoot.childCount, DeleteFacItem);
                OpenUIPanel();
                break;
            case "�ֿ�ɾ��_ˢ��":
                facRoot = (Transform)op.param;

                CreateList(itemPrefab, facRoot.childCount, DeleteFacItem);
                
                break;
        }
       // Debug.LogError("1111");
       
      
    }


    void DeleteFacItem(List<GameObject> objs)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            GameObject tempItem = objs[i];
            tempItem.GetComponentInChildren<Text>().text = facRoot.GetChild(i).name;

            //�����Ӧ�¼�
            Button btn = tempItem.GetComponentInChildren<Button>();
            int count = i;
            btn.onClick.AddListener(() =>
            {
                DestroyImmediate(facRoot.GetChild(count).gameObject);
                DestroyImmediate(tempItem);
                CreateList(itemPrefab, facRoot.childCount, DeleteFacItem);
                //CreateList(itemPrefab, facRoot.childCount,);
            });
            btn.GetComponentInChildren<Text>().text = "ɾ��";

        }
    }


    public Text title;

    Transform facRoot;

    public Transform itemParent;

    //��ȡ��Ӧ�ֿ⣬�жϲֿ��λ���������б�
    public GameObject _factoryObj;

    GameObject itemPrefab;




    void CreateList(GameObject itemPrefab,int listCount, UnityAction<List<GameObject>> ac)
    {
        //��յ�ǰ
        for (int i = 0; i < itemParent.childCount; i++)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }

        List<GameObject> createListObj = new List<GameObject>();
        for (int i = 0; i < listCount; i++)
        {
            GameObject tempItem = Instantiate(itemPrefab, itemParent);
            tempItem.name = i.ToString();
            createListObj.Add(tempItem);
        }

        if (ac != null)
            ac(createListObj);
    }



    void CreateList_Editor()
    {
        for (int i = 0; i < itemParent.childCount; i++)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }

        if (_factoryObj != null)
        {
            Transform objTempArea = CommonHelper.GetFactoryAreaParent(_factoryObj);
            if (objTempArea != null)
            {
                for (int i = 0; i < objTempArea.childCount; i++)
                {
                    GameObject tempItem = Instantiate(itemPrefab, itemParent);
                    tempItem.GetComponentInChildren<Text>().text = objTempArea.GetChild(i).name;
                    tempItem.name = i.ToString();
                    //�����Ӧ�¼�
                    Button btn = tempItem.GetComponentInChildren<Button>();
                    int count = i;
                    btn.onClick.AddListener(() =>
                    {
                        DestroyImmediate(objTempArea.GetChild(count).gameObject);
                        CreateList_Editor();
                    });
                    btn.GetComponentInChildren<Text>().text = "ɾ��";
                    //�����¼�����긲�ǣ��ı���ɫ�������:����

                    EventTrigger tempTrigger = tempItem.GetComponent<EventTrigger>();
                    if (tempTrigger == null)
                        tempTrigger = tempItem.AddComponent<EventTrigger>();
                    EventTrigger.Entry entryPointE = new EventTrigger.Entry();
                    entryPointE.eventID = EventTriggerType.PointerEnter;
                    entryPointE.callback.AddListener((bed) =>
                    {
                        objTempArea.GetChild(count).GetComponent<LineRenderer>().startColor = Color.red;
                        objTempArea.GetChild(count).GetComponent<LineRenderer>().endColor = Color.red;
                    });
                    EventTrigger.Entry entryPointO = new EventTrigger.Entry();
                    entryPointO.eventID = EventTriggerType.PointerExit;
                    entryPointO.callback.AddListener((bed) =>
                    {
                        objTempArea.GetChild(count).GetComponent<LineRenderer>().startColor = Color.yellow;
                        objTempArea.GetChild(count).GetComponent<LineRenderer>().endColor = Color.yellow;
                    });
                    tempTrigger.triggers.Add(entryPointE);
                    tempTrigger.triggers.Add(entryPointO);
                }
            }
        }
    }





    void CreateList_Select()
    {
        for (int i = 0; i < itemParent.childCount; i++)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }

        if (_factoryObj != null)
        {
            Transform objTempArea = CommonHelper.GetFactoryAreaParent(_factoryObj);
            if (objTempArea != null)
            {
                for (int i = 0; i < objTempArea.childCount; i++)
                {
                    GameObject tempItem = Instantiate(itemPrefab, itemParent);
                    tempItem.GetComponentInChildren<Text>().text = objTempArea.GetChild(i).name;
                    tempItem.name = i.ToString();
                    //�����Ӧ�¼�
                    Button btn = tempItem.GetComponentInChildren<Button>();
                    int count = i;
                    btn.onClick.AddListener(() =>
                    {
                        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.SelectGetObj, objTempArea.GetChild(count).gameObject));
                    });
                    btn.GetComponentInChildren<Text>().text = "ѡ��";
                    //�����¼�����긲�ǣ��ı���ɫ�������:����

                    EventTrigger tempTrigger = tempItem.GetComponent<EventTrigger>();
                    if (tempTrigger == null)
                        tempTrigger = tempItem.AddComponent<EventTrigger>();
                    EventTrigger.Entry entryPointE = new EventTrigger.Entry();
                    entryPointE.eventID = EventTriggerType.PointerEnter;
                    entryPointE.callback.AddListener((bed) =>
                    {
                        objTempArea.GetChild(count).GetComponent<LineRenderer>().startColor = Color.red;
                        objTempArea.GetChild(count).GetComponent<LineRenderer>().endColor = Color.red;
                    });
                    EventTrigger.Entry entryPointO = new EventTrigger.Entry();
                    entryPointO.eventID = EventTriggerType.PointerExit;
                    entryPointO.callback.AddListener((bed) =>
                    {
                        objTempArea.GetChild(count).GetComponent<LineRenderer>().startColor = Color.yellow;
                        objTempArea.GetChild(count).GetComponent<LineRenderer>().endColor = Color.yellow;
                    });
                    tempTrigger.triggers.Add(entryPointE);
                    tempTrigger.triggers.Add(entryPointO);
                }
            }
        }
    }
}

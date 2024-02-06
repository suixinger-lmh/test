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
            openTw.Kill(false);//关闭的时候不需要上个完成，当前位置关闭
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

            case "关闭":
                CloseUIPanel();
                break;

            //生成当前仓库的区域列表
            case "区域编辑":
                title.text = "区域编辑";
                _factoryObj = (GameObject)op.param;
                CreateList_Editor();
                OpenUIPanel();
                break;
            case "区域编辑_刷新":
                _factoryObj = (GameObject)op.param;
                CreateList_Editor();
                break;
            case "区域选择":
                title.text = "区域选择";
                _factoryObj = (GameObject)op.param;
                CreateList_Select();
                OpenUIPanel();
                break;

            case "仓库删除":
                facRoot = (Transform)op.param;
                title.text = "仓库删除";
                CreateList(itemPrefab, facRoot.childCount, DeleteFacItem);
                OpenUIPanel();
                break;
            case "仓库删除_刷新":
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

            //点击响应事件
            Button btn = tempItem.GetComponentInChildren<Button>();
            int count = i;
            btn.onClick.AddListener(() =>
            {
                DestroyImmediate(facRoot.GetChild(count).gameObject);
                DestroyImmediate(tempItem);
                CreateList(itemPrefab, facRoot.childCount, DeleteFacItem);
                //CreateList(itemPrefab, facRoot.childCount,);
            });
            btn.GetComponentInChildren<Text>().text = "删除";

        }
    }


    public Text title;

    Transform facRoot;

    public Transform itemParent;

    //获取对应仓库，判断仓库库位数，生成列表
    public GameObject _factoryObj;

    GameObject itemPrefab;




    void CreateList(GameObject itemPrefab,int listCount, UnityAction<List<GameObject>> ac)
    {
        //清空当前
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
                    //点击响应事件
                    Button btn = tempItem.GetComponentInChildren<Button>();
                    int count = i;
                    btn.onClick.AddListener(() =>
                    {
                        DestroyImmediate(objTempArea.GetChild(count).gameObject);
                        CreateList_Editor();
                    });
                    btn.GetComponentInChildren<Text>().text = "删除";
                    //自身事件【鼠标覆盖：改变颜色；鼠标点击:看】

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
                    //点击响应事件
                    Button btn = tempItem.GetComponentInChildren<Button>();
                    int count = i;
                    btn.onClick.AddListener(() =>
                    {
                        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.SelectGetObj, objTempArea.GetChild(count).gameObject));
                    });
                    btn.GetComponentInChildren<Text>().text = "选择";
                    //自身事件【鼠标覆盖：改变颜色；鼠标点击:看】

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

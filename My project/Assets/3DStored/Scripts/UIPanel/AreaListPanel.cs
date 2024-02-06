using DG.Tweening;
using FrameWork;
using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//展示仓库所包含的区位列表
public class AreaListPanel : UIPanel
{

    protected override void Start()
    {
        base.Start();
        if(itemPrefab == null)
        {
            itemPrefab = Resources.Load("UIItemPrefab/arealistItem") as GameObject;
        }


        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.CallUIPanel, RecreateViewListEvent);
    }


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
        GameObject tempfac = (GameObject)ce.EventParam;

        _factoryObj = tempfac;
        ReCreateList();
    }

    public Transform itemParent;

    //获取对应仓库，判断仓库库位数，生成列表
    public GameObject _factoryObj;

    GameObject itemPrefab;
    void ReCreateList()
    {
        for(int i = 0; i < itemParent.childCount; i++)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }

        if (_factoryObj != null)
        {
            Transform objTempArea = CommonHelper.GetFactoryAreaParent(_factoryObj);
            if (objTempArea != null)
            {
                for(int i = 0; i < objTempArea.childCount; i++)
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
                        ReCreateList();
                    });
                    //自身事件【鼠标覆盖：改变颜色；鼠标点击:看】

                    EventTrigger tempTrigger = tempItem.GetComponent<EventTrigger>();
                    if (tempTrigger == null)
                        tempTrigger = tempItem.AddComponent<EventTrigger>();
                    EventTrigger.Entry entryPointE = new EventTrigger.Entry();
                    entryPointE.eventID = EventTriggerType.PointerEnter;
                    entryPointE.callback.AddListener((bed) => {
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

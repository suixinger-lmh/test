using DG.Tweening;
using FrameWork;
using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateViewPanel : LeftUIPanel
{
    public Text title;
    public Transform itemparent;

    //toggle打开
    public override void TogglePanel(bool isOn)
    {
        if (isOn)
            OpenUIPanel();
        else
            CloseUIPanel();
    }
    //事件打开

    protected override void Start()
    {
        base.Start();

        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener( CoreEventId.CreateViewPanelOpen, OpenPanelEvent);
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.CreateViewPanelClose, ClosePanelEvent);

        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.CreateViewPanelOBJEventChange, ReSetAllObjEvent);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Manager3DStored.Instance != null)
        {
            Manager3DStored.Instance.GetStoredComponent<EventComponent>().RemoveEventListener(CoreEventId.CreateViewPanelOpen, OpenPanelEvent);
            Manager3DStored.Instance.GetStoredComponent<EventComponent>().RemoveEventListener(CoreEventId.CreateViewPanelClose, ClosePanelEvent);

            Manager3DStored.Instance.GetStoredComponent<EventComponent>().RemoveEventListener(CoreEventId.CreateViewPanelOBJEventChange, ReSetAllObjEvent);
        }
    }


    void OpenPanelEvent(CoreEvent ce)
    {
        //根据不同面板字符串参数，区分要打开的面板
        string str = (string)ce.EventParam;
        if(title.text == str)
        {
            OpenUIPanel();
        }
    }
    void ClosePanelEvent(CoreEvent ce)
    {
        string str = (string)ce.EventParam;
        if (title.text == str)
        {
            CloseUIPanel();
        }
    }
    //public void CreateViewByDate<T>(string titleName,GameObject itemPrefab,List<T> datalist,Action<List<GameObject>> ac)
    //{
    //    title.text = titleName;

    //    List<GameObject> tempInits = new List<GameObject>();
    //    if(datalist!=null && datalist.Count > 0)
    //    {
    //        for(int i = 0; i < datalist.Count; i++)
    //        {
    //            GameObject tempInstance = Instantiate(itemPrefab, itemparent);
    //            tempInits.Add(tempInstance);
    //        }
    //    }

    //    if (ac != null)
    //        ac(tempInits);
    //    //Debug.Log(titleName);
    //}

    /// <summary>
    /// 根据指定数量生成选项，并给选项添加点击事件
    /// </summary>
    /// <param name="titleName">面板类型</param>
    /// <param name="itemPrefab">对应选项预制体</param>
    /// <param name="createCount">指定数量</param>
    /// <param name="ac">点击事件</param>
    public void CreateViewByDate(string titleName, GameObject itemPrefab, int createCount, Action<List<GameObject>> ac)
    {
        title.text = titleName;

        List<GameObject> tempInits = new List<GameObject>();
        for(int i= 0; i < createCount; i++)
        {
            GameObject tempInstance = Instantiate(itemPrefab, itemparent);
            tempInits.Add(tempInstance);
        }

        if (ac != null)
            ac(tempInits);
        Debug.Log("自定义模型选择面板：" + titleName);
    }

   
    //重新切换,重新赋予响应事件
    public void ReSetAllObjEvent(CoreEvent ce)
    {
        CreateViewEvent cve = (CreateViewEvent)ce.EventParam;

        if (title.text == cve.title)
        {
            List<GameObject> objs = new List<GameObject>();
            for (int i = 0; i < itemparent.childCount; i++)
            {
                objs.Add(itemparent.GetChild(i).gameObject);
            }

            if (cve.ac != null)
                cve.ac(objs);
        }
    }


    public class CreateViewEvent
    {
        public string title;
        public UnityAction<List<GameObject>> ac;
    }

}

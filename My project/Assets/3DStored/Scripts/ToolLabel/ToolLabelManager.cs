using FrameWork;
using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolLabelManager : MonoBehaviour, IBaseComponent
{
    //标签两种显示方式：
    //1.world模式:canvas为world；标签位于三维坐标位置，标签大小根据距离关系设置，方向朝向相机
    //2.UI模式:canvas为相机；标签在UI层渲染，根据世界坐标位置的屏幕位置映射到UI位置，大小根据距离缩放，角度大的不展示
    public enum ToolLabelType
    {
        World = 1,
        UI = 2,
    }

    //标签参数:
    public class LabelCreateParam
    {
        public string name;//标签展示内容
        public string setName;//标签组
        public GameObject obj;//标签展示对象
        public Vector3 pos;//标签位置
        public Vector3 offset;//标签偏移
        public float scaleRef;//标签缩放参照【相机到观察点合适距离】
        public ToolLabelType labelType;//标签展示类型

        public UnityAction labelClickDo;//标签点击事件
    }
    public class LabelCMDParam
    {
        public string setName;//响应集合
        public bool isView;
        public ToolLabelType labelType;
        public ToolLabel toolLabel;
        public string name;
    }

    public Transform uiParent;
    public Transform worldParent;
    public Camera uiCamera;
    public Camera worldCamera;

    public GameObject labelPrefab;

    public void InitComponent()
    {
        if(Manager3DStored.Instance.GetStoredComponent<EventComponent>() == null)
        {
            Debug.LogError("缺少功能依赖组件，请确认并添加依赖！");
            return;
        }

        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ToolLabel_Create, CreateLabel);//生成标签
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ToolLabel_Delete, DeleteLabel);//删除标签
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ToolLabel_View, ViewLabel);//显隐
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ToolLabel_Mode, ChangeLabelType);//模式切换

        if (labelPrefab == null)
            labelPrefab = Resources.Load("UIItemPrefab/labelItem") as GameObject;
    }


    public void InitManagerRefObj(Transform uiP,Camera uiC,Transform worldP,Camera worldC)
    {
        uiParent = uiP;
        uiCamera = uiC;
        worldParent = worldP;
        worldCamera = worldC;
    }


    Dictionary<GameObject, ToolLabel> objMap = new Dictionary<GameObject, ToolLabel>();//非全量，只记录存在obj的标签
    Dictionary<string, List<ToolLabel>> setMap = new Dictionary<string, List<ToolLabel>>();//全量记录，根据类型分别

    //生成参数：标签关联物体，类型，name，展示参数
    void CreateLabel(CoreEvent ce)
    {
        LabelCreateParam labelCreate = (LabelCreateParam)ce.EventParam;

        string setStr = labelCreate.setName;
        //标签是否需要生成：
        if (setMap.ContainsKey(setStr) && setMap[setStr] !=null)
        {
            ToolLabel TL = setMap[setStr].Find(p => p.labelName == labelCreate.name);
            if(TL != null)
            {
                //存在
                TL.InitLabel(labelCreate);//刷新参数
                return;
            }
        }

        //生成
        GameObject obj = Instantiate(labelPrefab);
        ToolLabel label = obj.GetComponent<ToolLabel>();
        label.labelParent_World = worldParent;
        label.labelParent_UI = uiParent;
        label.uiCamera = uiCamera;
        label.worldCamera = worldCamera;
        label.InitLabel(labelCreate);

        AddSetMap(labelCreate,label);
        AddObjMap(labelCreate, label);
    }

    void AddSetMap(LabelCreateParam op, ToolLabel tl)
    {
        if (setMap.ContainsKey(op.setName))
        {
            if (setMap[op.setName] == null)
            {
                setMap[op.setName] = new List<ToolLabel>();
            }
        }
        else
        {
            setMap.Add(op.setName, new List<ToolLabel>());
        }
        setMap[op.setName].Add(tl);
    }
    void AddObjMap(LabelCreateParam op,ToolLabel tl)
    {
        if (op.obj != null)
        {
            //不在字典中
            if (!objMap.ContainsKey(op.obj))
            {
                //物体对应标签字典收集
                objMap.Add(op.obj, tl);
            }
        }
    }




    void DeleteLabel(CoreEvent ce)
    {
        foreach(var tt in setMap.Keys)
        {
            foreach(var tool in setMap[tt])
            {
                Destroy(tool.gameObject);
            }
        }

        setMap.Clear();
        objMap.Clear();
    }
    void ViewLabel(CoreEvent ce)
    {
        LabelCMDParam op = (LabelCMDParam)ce.EventParam;
        
        foreach(ToolLabel label in GetToolLabelBySetName(op.setName))
        {
            if (op.isView)
            {
                label.ShowLabel();
            }
            else
            {
                label.HideLabel();
            }
        }
    }
    void ChangeLabelType(CoreEvent ce)
    {
        LabelCMDParam op = (LabelCMDParam)ce.EventParam;
        foreach (ToolLabel label in GetToolLabelBySetName(op.setName))
        {
            label.ChangeLabelType(op.labelType);
        }
    }


    //获取对应集合的标签
    List<ToolLabel> GetToolLabelBySetName(string setName)
    {
        List<ToolLabel> tempLabelList = new List<ToolLabel>();
        if (string.IsNullOrEmpty(setName))
        {
            //全部
            foreach(var tt in setMap.Keys)
            {
                tempLabelList.AddRange(setMap[tt]);
            }
        }
        else
        {
            if (setMap.ContainsKey(setName))//存在对应集合
            {
                tempLabelList.AddRange(setMap[setName]);
            }
        }

        return tempLabelList;
    }

}

using FrameWork;
using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolLabelManager : MonoBehaviour, IBaseComponent
{
    //��ǩ������ʾ��ʽ��
    //1.worldģʽ:canvasΪworld����ǩλ����ά����λ�ã���ǩ��С���ݾ����ϵ���ã����������
    //2.UIģʽ:canvasΪ�������ǩ��UI����Ⱦ��������������λ�õ���Ļλ��ӳ�䵽UIλ�ã���С���ݾ������ţ��Ƕȴ�Ĳ�չʾ
    public enum ToolLabelType
    {
        World = 1,
        UI = 2,
    }

    //��ǩ����:
    public class LabelCreateParam
    {
        public string name;//��ǩչʾ����
        public string setName;//��ǩ��
        public GameObject obj;//��ǩչʾ����
        public Vector3 pos;//��ǩλ��
        public Vector3 offset;//��ǩƫ��
        public float scaleRef;//��ǩ���Ų��ա�������۲����ʾ��롿
        public ToolLabelType labelType;//��ǩչʾ����

        public UnityAction labelClickDo;//��ǩ����¼�
    }
    public class LabelCMDParam
    {
        public string setName;//��Ӧ����
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
            Debug.LogError("ȱ�ٹ��������������ȷ�ϲ����������");
            return;
        }

        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ToolLabel_Create, CreateLabel);//���ɱ�ǩ
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ToolLabel_Delete, DeleteLabel);//ɾ����ǩ
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ToolLabel_View, ViewLabel);//����
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ToolLabel_Mode, ChangeLabelType);//ģʽ�л�

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


    Dictionary<GameObject, ToolLabel> objMap = new Dictionary<GameObject, ToolLabel>();//��ȫ����ֻ��¼����obj�ı�ǩ
    Dictionary<string, List<ToolLabel>> setMap = new Dictionary<string, List<ToolLabel>>();//ȫ����¼���������ͷֱ�

    //���ɲ�������ǩ�������壬���ͣ�name��չʾ����
    void CreateLabel(CoreEvent ce)
    {
        LabelCreateParam labelCreate = (LabelCreateParam)ce.EventParam;

        string setStr = labelCreate.setName;
        //��ǩ�Ƿ���Ҫ���ɣ�
        if (setMap.ContainsKey(setStr) && setMap[setStr] !=null)
        {
            ToolLabel TL = setMap[setStr].Find(p => p.labelName == labelCreate.name);
            if(TL != null)
            {
                //����
                TL.InitLabel(labelCreate);//ˢ�²���
                return;
            }
        }

        //����
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
            //�����ֵ���
            if (!objMap.ContainsKey(op.obj))
            {
                //�����Ӧ��ǩ�ֵ��ռ�
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


    //��ȡ��Ӧ���ϵı�ǩ
    List<ToolLabel> GetToolLabelBySetName(string setName)
    {
        List<ToolLabel> tempLabelList = new List<ToolLabel>();
        if (string.IsNullOrEmpty(setName))
        {
            //ȫ��
            foreach(var tt in setMap.Keys)
            {
                tempLabelList.AddRange(setMap[tt]);
            }
        }
        else
        {
            if (setMap.ContainsKey(setName))//���ڶ�Ӧ����
            {
                tempLabelList.AddRange(setMap[setName]);
            }
        }

        return tempLabelList;
    }

}

using FrameWork;
using Stored3D;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelvesEditPanel : LeftUIPanel
{
    //功能配置信息
    [System.Serializable]
    public class EditExFunc
    {
        public string btnName;
        public string resourcesPath;
        public string title;

        public string panelType;

        public GameObject panelObj;
    }


  




    //功能列表
    public List<EditExFunc> exfuncList;

    public Text title;
    public Transform exBtnP;
    public Transform panelSetP;//面板集合父物体
    public GameObject exView;
    public Button btn_exit;


    ExFuncPanelBase panelBase;//当前打开的功能面板



    protected override void Start()
    {
        base.Start();

        //自动生成添加的所有功能面板，按钮
        CreateExPanel();



        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ShelvesEdit_Close, OpenPanelByType);
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ShelvesEdit_Open, OpenPanelByType);


        btn_exit.onClick.AddListener(() => {
            if (panelBase != null)
                BackEx();//返回
            else
                CloseEx();//关闭
        });


    }

    ExFuncPanelBase.ExFuncParam exParam;//功能面板的通用参数
    void CreateExPanel()
    {
        if (exfuncList != null)
        {
            GameObject exbtnObj = Resources.Load("EXFunc/exBtn") as GameObject;


            for(int i = 0;i< exfuncList.Count; i++)
            {
                //生成按钮
                GameObject tempexBtn = Instantiate(exbtnObj, exBtnP);
                tempexBtn.GetComponentInChildren<Text>().text = exfuncList[i].btnName;
                int index = i;
                //每个功能按钮 打开 对应功能面板
                tempexBtn.GetComponent<Button>().onClick.AddListener(() => {
                    panelBase = exfuncList[index].panelObj.GetComponent<ExFuncPanelBase>();
                    panelBase.InitExFunc(exParam);
                    title.text = exfuncList[index].title;
                    OpenEx();
                });

                //不存在panel,resources生成
                if (exfuncList[i].panelObj == null)
                {
                    GameObject tempobj = Resources.Load(exfuncList[i].resourcesPath) as GameObject;
                    exfuncList[i].panelObj = Instantiate(tempobj, panelSetP);
                }
            }
            
            Resources.UnloadUnusedAssets();
        }
    }


    /// <summary>
    /// 展示功能面板
    /// </summary>
    void OpenEx()
    {
        exView.SetActive(false);
        panelSetP.gameObject.SetActive(true);
    }



    void BackEx()//返回
    {
        if (panelBase != null)//返回一级
        {
            panelBase.ExitExPanel();
            panelBase = null;
            title.text = "功能选择";
            exView.SetActive(true);
            panelSetP.gameObject.SetActive(false);
        }
    }
    void CloseEx()//直接退出
    {
        if (panelBase != null)//返回一级
        {
            panelBase.ExitExPanel();
            panelBase = null;
        }
        exParam = null;
        exView.SetActive(true);
        title.text = "功能选择";
        panelSetP.gameObject.SetActive(false);
        CloseUIPanel();
    }


    void OpenPanelByType(CoreEvent ce)
    {
        if(ce.EventID == CoreEventId.ShelvesEdit_Close)
        {
            CloseEx();
        }

        if (ce.EventID == CoreEventId.ShelvesEdit_Open)
        {
            exParam = (ExFuncPanelBase.ExFuncParam)ce.EventParam;
            exView.SetActive(true);
            panelSetP.gameObject.SetActive(false);
            OpenUIPanel();
        }
    }




  

   



}

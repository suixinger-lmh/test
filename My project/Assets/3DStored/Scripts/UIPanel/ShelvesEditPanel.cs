using FrameWork;
using Stored3D;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelvesEditPanel : LeftUIPanel
{
    //����������Ϣ
    [System.Serializable]
    public class EditExFunc
    {
        public string btnName;
        public string resourcesPath;
        public string title;

        public string panelType;

        public GameObject panelObj;
    }


  




    //�����б�
    public List<EditExFunc> exfuncList;

    public Text title;
    public Transform exBtnP;
    public Transform panelSetP;//��弯�ϸ�����
    public GameObject exView;
    public Button btn_exit;


    ExFuncPanelBase panelBase;//��ǰ�򿪵Ĺ������



    protected override void Start()
    {
        base.Start();

        //�Զ�������ӵ����й�����壬��ť
        CreateExPanel();



        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ShelvesEdit_Close, OpenPanelByType);
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ShelvesEdit_Open, OpenPanelByType);


        btn_exit.onClick.AddListener(() => {
            if (panelBase != null)
                BackEx();//����
            else
                CloseEx();//�ر�
        });


    }

    ExFuncPanelBase.ExFuncParam exParam;//��������ͨ�ò���
    void CreateExPanel()
    {
        if (exfuncList != null)
        {
            GameObject exbtnObj = Resources.Load("EXFunc/exBtn") as GameObject;


            for(int i = 0;i< exfuncList.Count; i++)
            {
                //���ɰ�ť
                GameObject tempexBtn = Instantiate(exbtnObj, exBtnP);
                tempexBtn.GetComponentInChildren<Text>().text = exfuncList[i].btnName;
                int index = i;
                //ÿ�����ܰ�ť �� ��Ӧ�������
                tempexBtn.GetComponent<Button>().onClick.AddListener(() => {
                    panelBase = exfuncList[index].panelObj.GetComponent<ExFuncPanelBase>();
                    panelBase.InitExFunc(exParam);
                    title.text = exfuncList[index].title;
                    OpenEx();
                });

                //������panel,resources����
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
    /// չʾ�������
    /// </summary>
    void OpenEx()
    {
        exView.SetActive(false);
        panelSetP.gameObject.SetActive(true);
    }



    void BackEx()//����
    {
        if (panelBase != null)//����һ��
        {
            panelBase.ExitExPanel();
            panelBase = null;
            title.text = "����ѡ��";
            exView.SetActive(true);
            panelSetP.gameObject.SetActive(false);
        }
    }
    void CloseEx()//ֱ���˳�
    {
        if (panelBase != null)//����һ��
        {
            panelBase.ExitExPanel();
            panelBase = null;
        }
        exParam = null;
        exView.SetActive(true);
        title.text = "����ѡ��";
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

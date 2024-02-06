using System.Collections.Generic;
using UnityEngine;
using Stored3D;
using FrameWork;
using UnityEngine.UI;
using Sxer.Camera;
using System;
using LitJson;
using Custom.UIWidgets;
using UnityEngine.Events;

//�������ɲ��֣�1.�ֶ�����   2.��json������  3.��������

//ȷ����λ���в���,��λ��С��


//�߶����ݣ�
//1.�����߸߶� -0.01  (�����ģ���غ���˸)
//2.����߶� 0   �ֿ���ø߶�
//3.��λ�߶�     �ڲֿ�����Ϸ�һ�㣬����


//TODO:
//�ֿ�ɾ��
//�������Ʊ༭
public partial class StoredLogicProcess : MonoBehaviour
{
  
    public Camera MainCamera;

    [Header("��������ť��")]
    public Toggle cangku_Select;
    public Toggle area_Add;
    public Toggle shelves_Add;
    public Toggle view_Tg;
    public Toggle edit_Tg;
    public Toggle create_tg;
    public Toggle data_tg;
    public GameObject maskBG;

    public Button save_Btn;
    public Button read_Btn;
    public Button delete_Btn;

    public Button TestBtn;
    [Header("����λ�ã�")]
    public Transform _FactorRoot;
    public Transform _TempAreaRoot;
    public Transform _TempShelvesRoot;

    [Header("������������")]
    [Header("-----��ǩ����-----")]
    public Transform _toolLabel_UIParent;
    public Camera _toolLabel_UICamera;
    public Transform _toolLabel_WorldParent;


    //��������¼
    AreaSetPanel tempUIPanel_AreaSetPanel;//����������
    CreateViewPanel tempUIPanel_FactorView;//�ֿ��������
    CreateViewPanel tempUIPanel_GoodsView;//��������
    SelectGetPanel tempUIPanel_SelectGetPanel;//ѡ�����
    ShelvesSetPanel tempUIPanel_ShelvesSetPanel;//�������ɻ������
    ShowInfPanel tempUIPanel_ShowInfPanel;//չʾ
    QuickAutoCreatePanel tempUIPanel_AutoCreatePanel;//��������
    //���������¼
    FloorGridSet tempFunc_FloorGrid;//��������
    DragManager tempFunc_DragManager;//��ק����
    DrawTool tempFunc_DrawTool;//������ƹ���
    ToolLabelManager tempFunc_ToolLabelManager;//��ǩ����




    // Start is called before the first frame update
    void Start()
    {
        //�����ȡ
        GetRunComponent();

        //��ع��ܳ�ʼ��
        Init();

        //UIչʾ���ݴ�����UI�߼���
        CreateUIVC();

        //������
        IsClickLock(false);
        //���ذ������ק���
        tempFunc_DragManager.GetOrAddDragItem(tempFunc_FloorGrid._Floor).InitDragFunc(CommonHelper.Floor, CommonHelper.Floor,true);
        Manager3DStored.Instance.DoFadeOut();

        if (Manager3DStored.Instance != null)
        {
            Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.RefreshTreeData, RefreshTree);
            Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.CameraLookAt, CameraLookCallBack);
        }

    }
    void GetRunComponent()
    {
        #region UI������
        //UIPanel�����ݽű����������������ַ���ע��
        //ͬ������������壬����UI�������ƻ�ȡ��Ӧ���
        tempUIPanel_FactorView = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel("CreateFactorPanel") as CreateViewPanel;
        tempUIPanel_GoodsView = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel("CreateGoodsPanel") as CreateViewPanel;
        //����Ψһ���ֱ�Ӹ������ͻ�ȡ
        tempUIPanel_AreaSetPanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<AreaSetPanel>();
        tempUIPanel_SelectGetPanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<SelectGetPanel>();
        tempUIPanel_ShelvesSetPanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<ShelvesSetPanel>();
        tempUIPanel_ShowInfPanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<ShowInfPanel>();
        tempUIPanel_AutoCreatePanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>();
        #endregion

        #region �������

        tempFunc_FloorGrid = Manager3DStored.Instance.GetStoredComponent<FloorGridSet>();
        tempFunc_DragManager = Manager3DStored.Instance.GetStoredComponent<DragManager>();
        tempFunc_DrawTool = Manager3DStored.Instance.GetStoredComponent<DrawTool>();
        tempFunc_ToolLabelManager = Manager3DStored.Instance.GetStoredComponent<ToolLabelManager>();
        #endregion
    }

    void Init()
    {
        //��ʼ����������
        tempFunc_FloorGrid.CreateFloorGrid(1000,1000);
        //��ק������ �����������
        tempFunc_DragManager.SetViewCamera(MainCamera);
        //���������� �����������
        tempFunc_DrawTool.SetNeeded(MainCamera, _TempAreaRoot);
        //ѡ����� ������ק���
        tempUIPanel_SelectGetPanel.InitSelectGetPanel(tempFunc_DragManager);
        //��ǩ����
        tempFunc_ToolLabelManager.InitManagerRefObj(_toolLabel_UIParent, _toolLabel_UICamera, _toolLabel_WorldParent, MainCamera);
    }

    //ע�����е����ṹ��嶼ˢ���ˣ�û������
    void RefreshTree(CoreEvent ce)
    {
        //ֻ�ı��Ӧ��node������ˢ��
        string nameStr = (string)ce.EventParam;
        nowSelectNode.Item.Name = nameStr;
        
        //�����ṹ��������
        //ObservableList<TreeNode<TreeViewItem>> stored = CreateViewListByScene_new();
        //tempUIPanel_ShowInfPanel.RefreshViewData(stored);
    }

    void CameraLookCallBack(CoreEvent ce)
    {
        GameObject obj = (GameObject)ce.EventParam;
        DoLookCreateObj(obj);
    }

    void CreateUIVC()
    {
        #region �ֿ��������

        List<Factory> data_Fac = Manager3DStored.Instance._factories;//��ȡ����
        //����ѡ���������ѡ�����¼���
        tempUIPanel_FactorView.CreateViewByDate("�ֿ��������", Resources.Load("UIItemPrefab/facItem") as GameObject, data_Fac.Count, (objs) =>
        {
            for (int i = 0; i < objs.Count; i++)
            {
                Factory tempFac = data_Fac[i];
                //չʾ����
                objs[i].GetComponentInChildren<Text>().text = tempFac.abRes.LabelName;
                objs[i].name = tempFac.abRes.LabelName;

                objs[i].GetComponentInChildren<RawImage>().texture = tempFac.GetThumbnail();
                //����¼�
                //string abName = tempFac.ABPath;
                //string assetName = tempFac.AssetName;
                objs[i].GetComponent<Button>().onClick.RemoveAllListeners();
                objs[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    //�ظ�λ�ò�����
                    for (int i = 0; i < _FactorRoot.childCount; i++)
                    {
                        if (_FactorRoot.GetChild(i).position == Vector3.zero)
                        {
                            Debug.LogError("�����ظ����أ�");
                            return;
                        }
                    }

                    //���ض�Ӧab��
                    //������ɺ�ж��ab��������ʵ��
                    Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Factory/", tempFac.abRes.ABPath, tempFac.abRes.AssetName, (go) =>
                    {
                        //��Դ����ʹ��
                        if (CommonHelper.IsSuitFactory(go,tempFac))
                        {  
                            //����
                            GameObject insObj = Instantiate(go, _FactorRoot);
                            //��ʼ��
                            StoredItemFactory facItem = CommonHelper.BindFactory_(insObj, tempFac);
                            facItem.InitOp();

                            //��ͷ�ƶ����������������㣩
                            //CameraLookAtObj_temp(Vector3.zero, 0, 30, CommonHelper.GetObjSuitDistance(insObj), 0);
                            //�ӽ�
                            DoLookCreateObj(insObj);
                            //�����б�ѡ�����
                            Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("�ֿ�ɾ��_ˢ��", _FactorRoot)));
                        }
                    });
                });
            }
        });
        //����忪�ذ�ť
        BindToggleWithPanel(cangku_Select, tempUIPanel_FactorView);

        #endregion

        //�ֿⰴť�������߼� �����л�
        cangku_Select.onValueChanged.AddListener((ison) =>
        {
            if (ison)//����ֿ�״̬
                ChangeStoredState(Stored3DState.AddFactory);
            else//�˻ؿ�״̬
                ChangeStoredState(Stored3DState.None);
        });




        tempUIPanel_AreaSetPanel.InitAreaSetPanel(tempFunc_DrawTool, MainCamera.GetComponent<CameraOperateSet>(), () => {
            area_Add.isOn = false;
        });
        //����忪�ذ�ť
        //BindToggleWithPanel(area_Add, tempAreaSetPanel);

        //������Ӱ�ť �¼����������+ѡȡ
        area_Add.onValueChanged.AddListener((ison) =>
        {
            if (ison)//��������״̬
                ChangeStoredState(Stored3DState.AddArea_select);
            else//�˻ؿ�״̬   �ر��¼�ת���رհ�ť��
                ChangeStoredState(Stored3DState.None);
        });



      





        //��ӻ���
        shelves_Add.onValueChanged.AddListener((ison) => {
            if (ison)
                ChangeStoredState(Stored3DState.AddShelves_select);
            else
                ChangeStoredState(Stored3DState.None);
        });

        //������������ʼ�����������ݣ����ɻ������Ͷ�Ӧ�Ĵ�����壩
        tempUIPanel_ShelvesSetPanel.InitShelvesSetPanel(Manager3DStored.Instance._shelves, _TempShelvesRoot, tempFunc_DragManager, () => {
            shelves_Add.isOn = false;
        });


        view_Tg.onValueChanged.AddListener((ison) => {

            if (ison)
            {
                if (_mainState == Stored3DState.None)
                    ChangeStoredState(Stored3DState.View);
                else
                    view_Tg.isOn = false;
            }
            else
                if(_mainState == Stored3DState.View)
                    ChangeStoredState(Stored3DState.None);
        });

        edit_Tg.onValueChanged.AddListener((ison) => {
            if (ison)
            {
                if (_mainState == Stored3DState.None)
                    ChangeStoredState(Stored3DState.Edit);
                else
                    edit_Tg.isOn = false;
            }
            else
              if (_mainState == Stored3DState.Edit)
                ChangeStoredState(Stored3DState.None);
        });

        create_tg.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                if (_mainState == Stored3DState.None)
                    ChangeStoredState(Stored3DState.Create);
                else
                    create_tg.isOn = false;
            }
            else
              if (_mainState == Stored3DState.Create)
                ChangeStoredState(Stored3DState.None);
        });

        data_tg.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                if (_mainState == Stored3DState.None)
                    ChangeStoredState(Stored3DState.GetData);
                else
                    create_tg.isOn = false;
            }
            else
              if (_mainState == Stored3DState.GetData)
                ChangeStoredState(Stored3DState.None);
        });



        save_Btn.onClick.AddListener(() => {
            SaveRecordData();
        });

        read_Btn.onClick.AddListener(() => {
            //Test
            //string str = System.IO.File.ReadAllText(@"C:\Users\PC5837\Desktop\temp.json");
            //RecordStored rs = JsonMapper.ToObject<RecordStored>(str);
            //DoReadCreate(rs);
            ReadRecordData();
        });

        delete_Btn.onClick.AddListener(() => {
            for(int i = 0; i < _FactorRoot.childCount; i++)
            {
                Destroy(_FactorRoot.GetChild(i).gameObject);
            }

            //Resources.UnloadUnusedAssets();
            //GC.Collect();
        });


        TestBtn.onClick.AddListener(() =>
        {

           
        });


        #region ���ɻ���

        List<Goods> data_Goods = Manager3DStored.Instance._goods;//��ȡ����
        //����ѡ���������ѡ�����¼���
        tempUIPanel_GoodsView.CreateViewByDate("����ѡ�����", Resources.Load("UIItemPrefab/facItem") as GameObject, data_Goods.Count, (objs) =>
        {
            for (int i = 0; i < objs.Count; i++)
            {
                Goods tempgoods = data_Goods[i];

                //չʾ����
                objs[i].GetComponentInChildren<Text>().text = tempgoods.abRes.LabelName;
                objs[i].name = tempgoods.abRes.LabelName;
                //����¼�
                //string abName = tempFac.ABPath;
                //string assetName = tempFac.AssetName;
                objs[i].GetComponent<Button>().onClick.RemoveAllListeners();
                objs[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent( CoreEventId.GoodsSelect,tempgoods));
                });
            }
        });
        #endregion



    }




    void DoLookCreateObj(GameObject obj)
    {
        //MainCamera.GetComponent<CameraOperateSet>().LookAtTargetOnce(obj);
        MainCamera.GetComponent<CameraOperateSet>().LookAtTargetWithSafePos(obj, CommonHelper.GetObjSuitDistance(obj));
    }


    /// <summary>
    /// ��ť�������
    /// </summary>
    /// <param name="isLock"></param>
    void IsClickLock(bool isLock)
    {
        maskBG.SetActive(isLock);
    }

    //#region UI��ť�¼�

    //void ButtonClickEvent_FactorySelectDo()
    //{

    //}


    //#endregion






    public void BindToggleWithPanel(Toggle tg,UIPanel panel)
    {
        tg.onValueChanged.RemoveAllListeners();
        tg.onValueChanged.AddListener(panel.TogglePanel);
    }


    //�����ṹ�Ĳ㼶��ϵ ����ȡ����
    GameObject FindNodeObj(TreeNode<TreeViewItem> node)
    {
        Transform findObj = null;
        switch (node.Path.Count)
        {
            case 0://�Ҳֿ�
                findObj = _FactorRoot.transform.Find(node.Item.Name);
                break;
            case 1://������
                findObj = FindNodeObj(node.Path[0]).transform;//���Ҳֿ�
                findObj = CommonHelper.GetFactoryAreaParent(findObj.gameObject);
                findObj = findObj.Find(node.Item.Name);
                break;
            case 2://�һ���
                findObj = FindNodeObj(node.Path[0]).transform;//��������
                findObj = CommonHelper.GetFactoryShelvesParent(findObj.gameObject);
                findObj = findObj.Find(node.Item.Name);
                break;
            case 3://�һ��ܲ�
                findObj = FindNodeObj(node.Path[0]).transform;//���һ���
                string indexStr = node.Item.Name.Split("_")[1];//���ư������
                int index = int.Parse(indexStr);
                findObj = findObj.GetComponentsInChildren<GDGoods>()[index-1].transform;
                break;
        }


        //Transform findObj = _FactorRoot;

        //for(int i = node.Path.Count-1; i >= 0; i--)
        //{
        //    findObj = findObj.transform.Find(node.Path[i].Item.Name);
        //    if (findObj == null)
        //        return null;
        //}

        //findObj = findObj.transform.Find(node.Item.Name);

        return findObj.gameObject;
    }

    void CameraLookAtObj(Vector3 center,float x,float y,float dis,float height)
    {
        MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraOpState.LookTarget);
        MainCamera.GetComponent<CameraOperateSet>().SetLookAtObj(center);
        MainCamera.GetComponent<CameraOperateSet>().SetLookAtParams(x, y, dis, height);
    }
    //void CameraLookAtObj_temp(Vector3 center, float x, float y, float dis, float height)
    //{
    //    MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraState.LookTarget);
    //    MainCamera.GetComponent<CameraOperateSet>().SetLookAtObj(center);
    //    MainCamera.GetComponent<CameraOperateSet>().SetLookAtParams(x, y, dis, height);

    //    MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraState.SimpleFly_UNITY);
    //}

    /// <summary>
    /// ��ǩ���ɲ���
    /// </summary>
    /// <param name="name">��ǩ����</param>
    /// <param name="setName">��������</param>
    /// <param name="obj">��ǩչʾ����</param>
    /// <param name="pos">��ǩλ��</param>
    /// <param name="offset">ƫ��</param>
    /// <param name="scaleRef">���Ų���</param>
    /// <param name="toolLabelType">��ǩ����</param>
    void CreateLabel(string name,string setName, GameObject obj,Vector3 pos,Vector3 offset,float scaleRef, ToolLabelManager.ToolLabelType toolLabelType,UnityAction clickAction)
    {
        if (obj == null && pos == null)
            return;
        ToolLabelManager.LabelCreateParam createInf = new ToolLabelManager.LabelCreateParam();
        createInf.name = name;
        createInf.setName = setName;
        createInf.obj = obj;
        createInf.pos = pos;
        createInf.offset = offset;
        createInf.scaleRef = scaleRef;
        createInf.labelType = toolLabelType;
        createInf.labelClickDo = clickAction;
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ToolLabel_Create, createInf));
    }







   

    GameObject nowFindFactory;
    GameObject nowArea;

    TreeNode<TreeViewItem> nowSelectNode;

    // Update is called once per frame
    void Update()
    {



    }
}

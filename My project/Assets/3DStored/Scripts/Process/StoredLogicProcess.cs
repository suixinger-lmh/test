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

//三个生成部分：1.手动生成   2.从json中生成  3.快速生成

//确定库位行列参数,库位大小，


//高度数据：
//1.网格线高度 -0.01  (避免和模型重合闪烁)
//2.地面高度 0   仓库放置高度
//3.库位高度     在仓库地面上方一点，悬浮


//TODO:
//仓库删除
//区域名称编辑
public partial class StoredLogicProcess : MonoBehaviour
{
  
    public Camera MainCamera;

    [Header("工具栏按钮：")]
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
    [Header("物体位置：")]
    public Transform _FactorRoot;
    public Transform _TempAreaRoot;
    public Transform _TempShelvesRoot;

    [Header("功能依赖物体")]
    [Header("-----标签管理-----")]
    public Transform _toolLabel_UIParent;
    public Camera _toolLabel_UICamera;
    public Transform _toolLabel_WorldParent;


    //功能面板记录
    AreaSetPanel tempUIPanel_AreaSetPanel;//区域绘制面板
    CreateViewPanel tempUIPanel_FactorView;//仓库生成面板
    CreateViewPanel tempUIPanel_GoodsView;//货物生成
    SelectGetPanel tempUIPanel_SelectGetPanel;//选中面板
    ShelvesSetPanel tempUIPanel_ShelvesSetPanel;//区域生成货架面板
    ShowInfPanel tempUIPanel_ShowInfPanel;//展示
    QuickAutoCreatePanel tempUIPanel_AutoCreatePanel;//快速生成
    //功能组件记录
    FloorGridSet tempFunc_FloorGrid;//地面网格
    DragManager tempFunc_DragManager;//拖拽管理
    DrawTool tempFunc_DrawTool;//区域绘制工具
    ToolLabelManager tempFunc_ToolLabelManager;//标签管理




    // Start is called before the first frame update
    void Start()
    {
        //组件获取
        GetRunComponent();

        //相关功能初始化
        Init();

        //UI展示内容创建，UI逻辑绑定
        CreateUIVC();

        //黑屏出
        IsClickLock(false);
        //给地板添加拖拽组件
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
        #region UI面板组件
        //UIPanel面板根据脚本所挂载物体名称字符串注册
        //同类多个数量的面板，根据UI物体名称获取对应组件
        tempUIPanel_FactorView = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel("CreateFactorPanel") as CreateViewPanel;
        tempUIPanel_GoodsView = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel("CreateGoodsPanel") as CreateViewPanel;
        //数量唯一面板直接根据类型获取
        tempUIPanel_AreaSetPanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<AreaSetPanel>();
        tempUIPanel_SelectGetPanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<SelectGetPanel>();
        tempUIPanel_ShelvesSetPanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<ShelvesSetPanel>();
        tempUIPanel_ShowInfPanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<ShowInfPanel>();
        tempUIPanel_AutoCreatePanel = Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>();
        #endregion

        #region 功能组件

        tempFunc_FloorGrid = Manager3DStored.Instance.GetStoredComponent<FloorGridSet>();
        tempFunc_DragManager = Manager3DStored.Instance.GetStoredComponent<DragManager>();
        tempFunc_DrawTool = Manager3DStored.Instance.GetStoredComponent<DrawTool>();
        tempFunc_ToolLabelManager = Manager3DStored.Instance.GetStoredComponent<ToolLabelManager>();
        #endregion
    }

    void Init()
    {
        //初始化地面网格
        tempFunc_FloorGrid.CreateFloorGrid(1000,1000);
        //拖拽管理功能 依赖物体添加
        tempFunc_DragManager.SetViewCamera(MainCamera);
        //绘制区域功能 依赖物体添加
        tempFunc_DrawTool.SetNeeded(MainCamera, _TempAreaRoot);
        //选中面板 依赖拖拽组件
        tempUIPanel_SelectGetPanel.InitSelectGetPanel(tempFunc_DragManager);
        //标签物体
        tempFunc_ToolLabelManager.InitManagerRefObj(_toolLabel_UIParent, _toolLabel_UICamera, _toolLabel_WorldParent, MainCamera);
    }

    //注意所有的树结构面板都刷新了，没做区分
    void RefreshTree(CoreEvent ce)
    {
        //只改变对应的node，进行刷新
        string nameStr = (string)ce.EventParam;
        nowSelectNode.Item.Name = nameStr;
        
        //整个结构重新生成
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
        #region 仓库生成面板

        List<Factory> data_Fac = Manager3DStored.Instance._factories;//获取数据
        //生成选项，并添加面板选项点击事件：
        tempUIPanel_FactorView.CreateViewByDate("仓库生成面板", Resources.Load("UIItemPrefab/facItem") as GameObject, data_Fac.Count, (objs) =>
        {
            for (int i = 0; i < objs.Count; i++)
            {
                Factory tempFac = data_Fac[i];
                //展示内容
                objs[i].GetComponentInChildren<Text>().text = tempFac.abRes.LabelName;
                objs[i].name = tempFac.abRes.LabelName;

                objs[i].GetComponentInChildren<RawImage>().texture = tempFac.GetThumbnail();
                //点击事件
                //string abName = tempFac.ABPath;
                //string assetName = tempFac.AssetName;
                objs[i].GetComponent<Button>().onClick.RemoveAllListeners();
                objs[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    //重复位置不生成
                    for (int i = 0; i < _FactorRoot.childCount; i++)
                    {
                        if (_FactorRoot.GetChild(i).position == Vector3.zero)
                        {
                            Debug.LogError("请勿重复加载！");
                            return;
                        }
                    }

                    //加载对应ab包
                    //加载完成后卸载ab包并生成实例
                    Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Factory/", tempFac.abRes.ABPath, tempFac.abRes.AssetName, (go) =>
                    {
                        //资源可以使用
                        if (CommonHelper.IsSuitFactory(go,tempFac))
                        {  
                            //生成
                            GameObject insObj = Instantiate(go, _FactorRoot);
                            //初始化
                            StoredItemFactory facItem = CommonHelper.BindFactory_(insObj, tempFac);
                            facItem.InitOp();

                            //镜头移动（距离根据体积计算）
                            //CameraLookAtObj_temp(Vector3.zero, 0, 30, CommonHelper.GetObjSuitDistance(insObj), 0);
                            //视角
                            DoLookCreateObj(insObj);
                            //启用列表选择面板
                            Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("仓库删除_刷新", _FactorRoot)));
                        }
                    });
                });
            }
        });
        //绑定面板开关按钮
        BindToggleWithPanel(cangku_Select, tempUIPanel_FactorView);

        #endregion

        //仓库按钮绑定其他逻辑 功能切换
        cangku_Select.onValueChanged.AddListener((ison) =>
        {
            if (ison)//进入仓库状态
                ChangeStoredState(Stored3DState.AddFactory);
            else//退回空状态
                ChangeStoredState(Stored3DState.None);
        });




        tempUIPanel_AreaSetPanel.InitAreaSetPanel(tempFunc_DrawTool, MainCamera.GetComponent<CameraOperateSet>(), () => {
            area_Add.isOn = false;
        });
        //绑定面板开关按钮
        //BindToggleWithPanel(area_Add, tempAreaSetPanel);

        //区域添加按钮 事件：自由相机+选取
        area_Add.onValueChanged.AddListener((ison) =>
        {
            if (ison)//进入区域状态
                ChangeStoredState(Stored3DState.AddArea_select);
            else//退回空状态   关闭事件转到关闭按钮上
                ChangeStoredState(Stored3DState.None);
        });



      





        //添加货架
        shelves_Add.onValueChanged.AddListener((ison) => {
            if (ison)
                ChangeStoredState(Stored3DState.AddShelves_select);
            else
                ChangeStoredState(Stored3DState.None);
        });

        //货架生成面板初始化（根据数据，生成货架类型对应的创建面板）
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


        #region 生成货物

        List<Goods> data_Goods = Manager3DStored.Instance._goods;//获取数据
        //生成选项，并添加面板选项点击事件：
        tempUIPanel_GoodsView.CreateViewByDate("货物选择面板", Resources.Load("UIItemPrefab/facItem") as GameObject, data_Goods.Count, (objs) =>
        {
            for (int i = 0; i < objs.Count; i++)
            {
                Goods tempgoods = data_Goods[i];

                //展示内容
                objs[i].GetComponentInChildren<Text>().text = tempgoods.abRes.LabelName;
                objs[i].name = tempgoods.abRes.LabelName;
                //点击事件
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
    /// 按钮点击屏蔽
    /// </summary>
    /// <param name="isLock"></param>
    void IsClickLock(bool isLock)
    {
        maskBG.SetActive(isLock);
    }

    //#region UI按钮事件

    //void ButtonClickEvent_FactorySelectDo()
    //{

    //}


    //#endregion






    public void BindToggleWithPanel(Toggle tg,UIPanel panel)
    {
        tg.onValueChanged.RemoveAllListeners();
        tg.onValueChanged.AddListener(panel.TogglePanel);
    }


    //用树结构的层级关系 来获取物体
    GameObject FindNodeObj(TreeNode<TreeViewItem> node)
    {
        Transform findObj = null;
        switch (node.Path.Count)
        {
            case 0://找仓库
                findObj = _FactorRoot.transform.Find(node.Item.Name);
                break;
            case 1://找区域
                findObj = FindNodeObj(node.Path[0]).transform;//先找仓库
                findObj = CommonHelper.GetFactoryAreaParent(findObj.gameObject);
                findObj = findObj.Find(node.Item.Name);
                break;
            case 2://找货架
                findObj = FindNodeObj(node.Path[0]).transform;//先找区域
                findObj = CommonHelper.GetFactoryShelvesParent(findObj.gameObject);
                findObj = findObj.Find(node.Item.Name);
                break;
            case 3://找货架层
                findObj = FindNodeObj(node.Path[0]).transform;//先找货架
                string indexStr = node.Item.Name.Split("_")[1];//名称包含序号
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
    /// 标签生成参数
    /// </summary>
    /// <param name="name">标签内容</param>
    /// <param name="setName">所属集合</param>
    /// <param name="obj">标签展示物体</param>
    /// <param name="pos">标签位置</param>
    /// <param name="offset">偏移</param>
    /// <param name="scaleRef">缩放参照</param>
    /// <param name="toolLabelType">标签类型</param>
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

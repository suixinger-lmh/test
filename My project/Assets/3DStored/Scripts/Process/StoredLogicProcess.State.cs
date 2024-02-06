using FrameWork;
using LitJson;
using Stored3D;
using Sxer.Camera;
using UnityEngine;

public partial class StoredLogicProcess
{
    public enum Stored3DState
    {
        None,//空状态=相机自由移动
        AddFactory,//添加仓库状态=相机自由移动+捡取放置功能
        AddArea_select,//添加区域状态=选择仓库
        AddArea_edit,//添加区域状态=绘制区域
        AddShelves_select,//添加货架=选择仓库
        AddShelves_select_area,//添加货架=选择区域
        AddShelves_add,//添加

        SelectFactory,//选择仓库

        View,//观察模式
        Edit,//编辑
        Create,//xx
        GetData,//平台数据    【可配置项：库位(货架模型，货架层数，间隔)】
    }

    [Header("模式")]
    public Stored3DState _mainState = Stored3DState.None;


    //切换状态，控制组件变动
    void ChangeStoredState(Stored3DState nextState)
    {
        //当前状态结束
        switch (_mainState)
        {
            case Stored3DState.None:
                break;
            case Stored3DState.AddFactory:
                //放置功能-选取
                tempFunc_DragManager.SetDragManager(false);

                //关闭区域列表面板
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("关闭", null)));


                //Manager3DStored.Instance.GetStoredComponent<DragManager>().ChangeDrayType(DragManager.DragType.None);
                break;

            case Stored3DState.AddArea_select:
                if (nextState == Stored3DState.AddArea_edit)
                {
                   

                }
                else
                {
                    //结束
                    IsClickLock(false);
                    //Manager3DStored.Instance.GetStoredComponent<DragManager>().ChangeDrayType(DragManager.DragType.None);
                }
                break;
            case Stored3DState.AddArea_edit:
                nowFindFactory = null;
                //关闭区域列表面板
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("关闭", null)));


                //其他factory
                for (int i = 0; i < _FactorRoot.childCount; i++)
                    _FactorRoot.GetChild(i).gameObject.SetActive(true);

                //结束
                IsClickLock(false);
                tempFunc_DragManager.SetDragManager(false);
                
                MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate( CameraOpState.SimpleFly_UNITY);
                break;
            case Stored3DState.AddShelves_select:
                if (nextState == Stored3DState.AddShelves_select_area)
                {
                }
                else
                {
                    //结束
                    IsClickLock(false);
                }

                break;

            case Stored3DState.AddShelves_select_area:
                if(nextState == Stored3DState.AddShelves_add)
                {

                }
                else
                {
                    nowFindFactory = null;
                    //其他factory
                    for (int i = 0; i < _FactorRoot.childCount; i++)
                        _FactorRoot.GetChild(i).gameObject.SetActive(true);
                    //结束
                    IsClickLock(false);
                }
               
                break;

            case Stored3DState.AddShelves_add:
                //区域打开
                Transform tempareaP = CommonHelper.GetFactoryAreaParent(nowFindFactory);
                for (int i = 0; i < tempareaP.childCount; i++)
                        tempareaP.GetChild(i).gameObject.SetActive(true);

                nowFindFactory = null;
                //其他factory
                for (int i = 0; i < _FactorRoot.childCount; i++)
                    _FactorRoot.GetChild(i).gameObject.SetActive(true);
                //结束
                IsClickLock(false);

                break;

            case Stored3DState.View:

                IsClickLock(false);
                tempUIPanel_ShowInfPanel.CloseUIPanel();
                MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
                //标签删除
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ToolLabel_Delete,null));
                break;

            case Stored3DState.Edit:

                IsClickLock(false);
                tempUIPanel_ShowInfPanel.CloseUIPanel();
                MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
                //标签删除
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ToolLabel_Delete, null));
                break;

            case Stored3DState.Create:
                IsClickLock(false);
                tempUIPanel_AutoCreatePanel.CloseUIPanel();

                MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
                //标签删除
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ToolLabel_Delete, null));
                break;
            case Stored3DState.GetData:
                IsClickLock(false);
                tempUIPanel_ShowInfPanel.CloseUIPanel();
                MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
                //关闭面板
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.OpenStockInf, null));
                break;
        }

        //新状态初始
        switch (nextState)
        {
            case Stored3DState.None:
                break;

            case Stored3DState.AddFactory://添加仓库状态
                //相机

                //启用列表选择面板
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("仓库删除", _FactorRoot)));
                

                //放置功能-选取
                tempFunc_DragManager.SetDragManager(true, CommonHelper.Factory, CommonHelper.Floor, DragManager.DragType.PickPut);
                //Manager3DStored.Instance.GetStoredComponent<DragManager>().ChangeDrayType(DragManager.DragType.PickUp,CommonHelper.Factory, "Floor");
                break;
            case Stored3DState.AddArea_select:  //进入区域编辑状态，选择仓库

                //打开选择面板
                tempUIPanel_SelectGetPanel.OpenSelectPanelType(new OpPanelparam("仓库", null), () => {
                    //取消
                    area_Add.isOn = false;
                }, 
                (selectObj) => {
                    //选中
                    nowFindFactory = selectObj;
                    ChangeStoredState(Stored3DState.AddArea_edit);
                });

                IsClickLock(true);
              
                break;
            case Stored3DState.AddArea_edit:

                //nowFindFactory = Manager3DStored.Instance.GetStoredComponent<DragManager>().findObj;
                //绘制面板打开二级
                tempUIPanel_AreaSetPanel.ShowAreaSetStyle(nowFindFactory);


                //启用列表选择面板
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("区域编辑", nowFindFactory)));
                

                //隐藏其他factory
                for (int i = 0; i < _FactorRoot.childCount; i++)
                    if (_FactorRoot.GetChild(i).gameObject != nowFindFactory)
                        _FactorRoot.GetChild(i).gameObject.SetActive(false);
                //下一状态

                //MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraState.LookTarget);

                //CameraOpLookTarget cameralookat = (CameraOpLookTarget)MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraState.LookTarget);
                //cameralookat.target = nowFindFactory.transform;

                break;

            case Stored3DState.AddShelves_select:
               
                //打开选择面板
                tempUIPanel_SelectGetPanel.OpenSelectPanelType(new OpPanelparam("仓库", null), () =>
                {
                    //取消
                    shelves_Add.isOn = false;
                },
                (selectObj) =>
                {
                    //选中
                    nowFindFactory = selectObj;
                    ChangeStoredState(Stored3DState.AddShelves_select_area);
                });

                IsClickLock(true);
                break;

            case Stored3DState.AddShelves_select_area://选择区域

                //隐藏其他factory
                for (int i = 0; i < _FactorRoot.childCount; i++)
                    if (_FactorRoot.GetChild(i).gameObject != nowFindFactory)
                        _FactorRoot.GetChild(i).gameObject.SetActive(false);

                //打开选择面板
                tempUIPanel_SelectGetPanel.OpenSelectPanelType(new OpPanelparam("区域", nowFindFactory), () =>
                {
                    //取消
                    shelves_Add.isOn = false;
                },
                (selectObj) =>
                {
                    nowArea = selectObj;
                    ChangeStoredState(Stored3DState.AddShelves_add);
                });


                break;

            case Stored3DState.AddShelves_add:
                Transform tempareaP = CommonHelper.GetFactoryAreaParent(nowFindFactory);
                for(int i = 0; i < tempareaP.childCount; i++)
                    if (tempareaP.GetChild(i).gameObject != nowArea)
                        tempareaP.GetChild(i).gameObject.SetActive(false);

                //打开货架生成面板
                tempUIPanel_ShelvesSetPanel.OpenPanel(nowFindFactory,nowArea);
                break;

            case Stored3DState.View:
                if(_mainState == Stored3DState.None)
                {
                    IsClickLock(true);
                    //打开展示模式
                    //生成列表数据
                    //计算当前场景生成树结构【结构树TreeViewItem只有Name信息,用变体存储信息】
                    tempUIPanel_ShowInfPanel.InitViewData(CreateViewListByScene_new(), (node) => {//列表选中事件
                        //获取数据对应物体
                        GameObject obj = FindNodeObj(node);

                        //变体存储信息
                        StoredTreeViewItem storedTVI = node.Item as StoredTreeViewItem;
                        // Debug.Log(storedTVI.ID + storedTVI.Name);

                       // Debug.Log(node.Path.Count);


                        if (obj == null) return;

                        //数据计算
                        Bounds box = CommonHelper.GetObjBounds(obj);//获取边界
                        float scaleValue = CommonHelper.GetObjSuitDistance(obj);//半径

                        Vector3 labelOffset = new Vector3(0, box.size.y/2 + Mathf.Clamp(box.size.y,0,1), 0);
                         //生成标签（标签响应事件）
                        CreateLabel(node.Item.Name,node.Path.Count.ToString(), obj,  box.center, labelOffset, scaleValue, ToolLabelManager.ToolLabelType.UI, () => {
                            //标签点击事件、
                            //打开信息显示面板
                            Debug.Log(storedTVI.ID);

                            Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewData, node));
                        });
                        //镜头移动（距离根据体积计算）
                        CameraLookAtObj(box.center+ labelOffset, 0, 30, scaleValue, 0);
                    }, 
                    (node) => { 

                        //未选中

                    });
                    //CreateViewListByScene();
                    //tempUIPanel_ShowInfPanel.InitViewData(shelvesList);
                }
                break;


            case Stored3DState.Edit:
                if (_mainState == Stored3DState.None)
                {
                    IsClickLock(true);
                    //打开展示模式
                    //生成列表数据
                    //计算当前场景生成树结构【结构树只有Name信息】
                    tempUIPanel_ShowInfPanel.InitViewData(CreateViewListByScene_new(), (node) =>
                    {
                        nowSelectNode = node;

                        GameObject obj = FindNodeObj(node);
                        StoredItemBase dabase = obj.GetComponent<StoredItemBase>();
                        
                        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.EditorPanel, dabase));
                        ////列表选中事件
                        ////获取数据对应物体
                        //GameObject obj = FindNodeObj(node);

                        ////变体存储信息
                        //StoredTreeViewItem storedTVI = node.Item as StoredTreeViewItem;
                        ////Debug.Log(storedTVI.ID + storedTVI.Name);

                        //if (obj == null) return;

                        ////数据计算
                        //Bounds box = CommonHelper.GetObjBounds(obj);//获取边界
                        //float scaleValue = box.size.magnitude / 2 + 1;//半径

                        //Vector3 labelOffset = new Vector3(0, box.size.y / 2 + Mathf.Clamp(box.size.y, 0, 1), 0);
                        ////生成标签（标签响应事件）
                        //CreateLabel(node.Item.Name, node.Path.Count.ToString(), obj, box.center, labelOffset, scaleValue, ToolLabelManager.ToolLabelType.UI, () =>
                        //{
                        //    //标签点击事件、
                        //    //打开信息显示面板
                        //    Debug.Log(storedTVI.ID);
                        //});
                        ////镜头移动（距离根据体积计算）
                        //CameraLookAtObj(box.center + labelOffset, 0, 30, scaleValue, 0);
                    },
                    (node) =>
                    {

                        //未选中

                    });
                }
                break;


            case Stored3DState.Create:
                if (_mainState == Stored3DState.None)
                {
                    tempUIPanel_AutoCreatePanel.OpenUIPanel();
                }

                break;
            case Stored3DState.GetData:
                if (_mainState == Stored3DState.None)
                {
                    //读取数据，生成仓库表
                    string path = PathExtensions.StreamingAssetsPath() + "/SceneData/stock.json";
                    StartCoroutine(CommonHelper.GetText(path, (textStr) =>
                    {
                        //平台数据结构
                        StockInf rs = JsonMapper.ToObject<StockInf>(textStr);
                        //DoReadCreate(rs);
                        Debug.Log(rs);
                        Debug.Log("仓库数量:" + rs.WAREHOUSES.Count);

                        //平台数据解析为树结构，相关信息存储在树结构里
                        //生成树视图
                        tempUIPanel_ShowInfPanel.InitViewData(CreateStockList(rs), (node) =>
                        {
                            //选择对应的仓库，根据id去字典中获取 该仓库的树结构
                            StoredTreeViewItem storedTVI = node.Item as StoredTreeViewItem;
                            if (facListTemp.ContainsKey(storedTVI.ID))
                                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.OpenStockInf, facListTemp[storedTVI.ID]));

                        },
                        (node) =>
                        {
                            //未选中
                        });

                    }, (errormsg) => {
                        Debug.LogError("数据获取失败");
                    }));
                }
                break;
        }

        _mainState = nextState;
    }






}

using FrameWork;
using Stored3D;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//初始状态：find模式
//存在selectObj：pickUP模式(仅一个)

//需要拖拽

public class ShelvesSetPanel : RightUIPanel
{
    public Transform shelvesTypeParent;

    public GameObject panel1;
    public GameObject panel2;


    public Button moveBtn;
    public Button saveBtn;
    public Button deleteBtn;

    public Button exBtn;

    public Button quitBtn;
    //public Button exbtn_level;
    //public Button exbtn_full;


    Transform createParent;

    //当前操作物体
    GameObject nowFactory;
    GameObject nowArea;
    GameObject nowSetObj;


    DragManager _dragManager;

    Dictionary<string, List<Shelves>> typeMap = new Dictionary<string, List<Shelves>>();
    protected override void Start()
    {
        base.Start();

        moveBtn.onClick.AddListener(() =>
        {
            if (nowSetObj != null)
            {
                //_dragManager.targetObj = nowSetObj;
                //_dragManager.ChangeDrayType( DragManager.DragType.PutDown, CommonHelper.TempShelves, nowArea.name);
                
                _dragManager.SetDragManager(true, CommonHelper.TempShelves, CommonHelper.TempArea, DragManager.DragType.PutOnce);
                _dragManager.SetPutObj(nowSetObj);
            }
        });


        deleteBtn.onClick.AddListener(() =>
        {
            if (nowSetObj != null)
            {
                Destroy(nowSetObj);

               
                ChangePanelState(0);
            }
        });

        saveBtn.onClick.AddListener(() => {
            if (nowSetObj != null)
            {
                Transform objParent = CommonHelper.GetFactoryShelvesParent(nowArea);
                //取消高亮
                _dragManager.UnsetFindLightObj(nowSetObj);
                nowSetObj.transform.SetParent(objParent);

               
                ChangePanelState(0);
            }
        });



        exBtn.onClick.AddListener(() =>
        {
            if (isExPanelOpen)
            {
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ShelvesEdit_Close, null));//关闭编辑面板
            }
            else
            {
                //收集通用参数，并打开多功能面板
                ExFuncPanelBase.ExFuncParam exfP = new ExFuncPanelBase.ExFuncParam();
                exfP._factory = nowFactory;
                exfP._area = nowArea;
                exfP._shelves = nowSetObj;
                exfP.TempShelvesParent = createParent;
                exfP.FacShelvesParent = CommonHelper.GetFactoryShelvesParent(nowArea);
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ShelvesEdit_Open, exfP));
            }
            isExPanelOpen = !isExPanelOpen;
           
        });

        quitBtn.onClick.AddListener(() => {
            //先删除处理
            if (nowSetObj != null)
            {
                if(isInCache)
                    Destroy(nowSetObj);
                else
                    _dragManager.UnsetFindLightObj(nowSetObj);
            }
            //退出
            ClosePanel();
        });
    }

    bool startSelect = false;
    private void Update()
    {
        if (startSelect)
        {
            //等待选中
            if (nowSetObj == null)
            {
                if (_dragManager != null)
                {
                    if (_dragManager.selectObj != null)
                    {
                        startSelect = false;
                        DoSelectObj(_dragManager.selectObj, _dragManager.selectObj.GetComponent<StoredItemBase>().GetData<Shelves>(),false);
                    }
                }
            }
        }
      
        //else
        //{
        //    if (_dragManager != null)
        //    {
        //        if(_dragManager.fi)
        //    }
        //}
    }


    void ClosePanel()
    {
        AreaRemoveComponent();//移除区域的box

        //初始样式
        ChangePanelState(0);

        nowFactory = null;
        nowArea = null;
        isInCache = false;

        _dragManager.SetDragManager(false);
        startSelect = false;
        CloseUIPanel();

        if (quitCallBack != null)
            quitCallBack();
    }

    public void OpenPanel(GameObject factory,GameObject area)
    {
        nowFactory = factory;
        nowArea = area;

        //添加box和拖拽
        //area.GetComponent<StoredItemArea>().
        AreaComponentChange();

        //初始样式
        ChangePanelState(0);

        //操作模式
        //_dragManager.ChangeDrayType(DragManager.DragType.Find_Light, CommonHelper.TempShelves, nowArea.name);
        //_dragManager.SetDragManager(true, CommonHelper.TempShelves, CommonHelper.TempArea, DragManager.DragType.FindOnce_Light);
        //startSelect = true;

        OpenUIPanel();//打开
    }

    #region 相关计算

    
    void AreaComponentChange()
    {
        StoredItemArea areaItem = nowArea.GetComponent<StoredItemArea>();
        //库位box改变
        areaItem.ChangeAreaBoxCollider(true);
        //作为拖拽地面
        areaItem.SetAreaFloor(true);
        //添加拖拽
        //_dragManager.GetOrAddDragItem(nowArea).InitDragFunc(CommonHelper.TempArea, CommonHelper.TempArea, true);
    }
    void AreaRemoveComponent()
    {
        StoredItemArea areaItem = nowArea.GetComponent<StoredItemArea>();
        //库位box改变
        areaItem.ChangeAreaBoxCollider(false);
        //作为拖拽地面
        areaItem.SetAreaFloor(false);
    }


    #endregion


    UnityAction quitCallBack;
    //初始化生成货架面板
    public void InitShelvesSetPanel(List<Shelves> shelves,Transform shelvesParent,DragManager dg,UnityAction ac = null)
    {
        _dragManager = dg;
        createParent = shelvesParent;
        quitCallBack = ac;

        //归类
        typeMap.Clear();
        for(int i = 0; i < shelves.Count; i++)
        {
            if (!typeMap.ContainsKey(shelves[i].Usage))
                typeMap.Add(shelves[i].Usage, new List<Shelves>());
            typeMap[shelves[i].Usage].Add(shelves[i]);   
        }

        //根据类别创建类别标签和对应生成面板
        GameObject typetogglePrefab = Resources.Load("UIItemPrefab/ShelvesToggle") as GameObject;
        GameObject panelTempObj = Resources.Load("Panel/SelectView") as GameObject;
        foreach(var tag in typeMap.Keys)
        {
            //标签
            GameObject typeTG = Instantiate(typetogglePrefab, shelvesTypeParent);
            typeTG.GetComponentInChildren<Text>().text = tag;
            typeTG.name = tag;
            Toggle tg = typeTG.GetComponent<Toggle>();
            tg.group = shelvesTypeParent.GetComponent<ToggleGroup>();

            //面板
            panelTempObj.name = tag + "SelectPanel";//修改预制体物体名称
            GameObject panelTemp = Instantiate(panelTempObj, transform.parent);
            panelTemp.name = tag + "SelectPanel";//去掉clone
            CreateViewPanel cvPtemp = panelTemp.GetComponent<CreateViewPanel>();
            List<Shelves> tagData = typeMap[tag];
            cvPtemp.CreateViewByDate(tag, Resources.Load("UIItemPrefab/facItem") as GameObject, tagData.Count, (objs) => {
                for (int i = 0; i < objs.Count; i++)
                {
                    //展示内容
                    objs[i].GetComponentInChildren<Text>().text = tagData[i].abRes.LabelName;
                    objs[i].name = tagData[i].abRes.LabelName;
                    objs[i].GetComponentInChildren<RawImage>().texture = tagData[i].GetThumbnail();
                    //点击事件
                    string abName = tagData[i].abRes.ABPath;
                    string assetName = tagData[i].abRes.AssetName;
                    Shelves data = tagData[i];
                    objs[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    objs[i].GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (nowSetObj != null)
                            return;

                        //加载对应ab包
                        //加载完成后卸载ab包并生成实例
                        Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Shelves/", abName, assetName, (go) =>
                        {
                            if(CommonHelper.IsSuitShelves(go, data))
                            {
                                GameObject insObj = Instantiate(go, createParent);

                                //初始位置，区域的中心，地板的高度
                                Vector3 center = nowArea.GetComponent<BoxCollider>().bounds.center;
                                float height = nowFactory.GetComponent<BoxCollider>().bounds.center.y;

                                StoredItemShelves sheItem = CommonHelper.BindShelves_(insObj, data);
                                sheItem.InitOp(center, height);

                                //位置，放在区域xz坐标系，xz为原点上
                                //insObj.transform.position = GetAreaOriginPosition(nowArea.GetComponent<BoxCollider>(), nowFactory);
                                //insObj.name = data.LabelName + "_" + DateTime.Now.ToShortTimeString();


                                //_dragManager.GetOrAddDragItem(insObj).InitDragFunc(CommonHelper.TempShelves,CommonHelper.TempArea);
                                //if (insObj.GetComponent<JDataHelper>() == null)
                                //    insObj.AddComponent<JDataHelper>().SetData<Shelves>(data);
                                //else
                                //    insObj.GetComponent<JDataHelper>().SetData<Shelves>(data);

                                //生成后自动选中
                                DoSelectObj(insObj, data, true);
                                //DoLookCreateObj(insObj);
                            }
                        });
                    });
                }
            });

            //绑定事件
            tg.onValueChanged.AddListener(cvPtemp.TogglePanel);
        }


        Resources.UnloadUnusedAssets();
    }

    bool isInCache = false;
    void DoSelectObj(GameObject obj,Shelves data,bool incache)
    {
        isInCache = incache;
        //选中物体
        nowSetObj = obj;
        //高亮
        _dragManager.SetDragManager(false);
        _dragManager.SetFindLightObj(nowSetObj);
        //_dragManager.ChangeDrayType(DragManager.DragType.PickUpOnlyTarget, CommonHelper.TempShelves, nowArea.name);
        //物体面板
        GetObjInf(data);
        ChangePanelState(1);

    }

    bool isExPanelOpen = false;
    void ChangePanelState(int state)
    {
        if(state == 0)//初始面板,显示生成
        {
            nowSetObj = null;
            _dragManager.SetDragManager(true, CommonHelper.TempShelves, CommonHelper.TempArea, DragManager.DragType.FindOnce_Light);
            startSelect = true;
            if(isExPanelOpen)
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ShelvesEdit_Close, null));//关闭编辑面板
            isExPanelOpen = false;

            shelvesTypeParent.GetComponent<ToggleGroup>().SetAllTogglesOff();
            panel1.SetActive(true);
            panel2.SetActive(false);
        }

        if(state == 1)
        {
            shelvesTypeParent.GetComponent<ToggleGroup>().SetAllTogglesOff();
            panel1.SetActive(false);
            panel2.SetActive(true);
        }
    }
    
    void GetObjInf(Shelves data)
    {
        panel2.transform.Find("inf/name").GetComponent<InputField>().text = nowSetObj.name;
        panel2.transform.Find("inf/type").GetComponent<Text>().text = data.Usage;
    }


}

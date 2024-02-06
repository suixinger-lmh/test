using Custom.UIWidgets;
using FrameWork;
using LitJson;
using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class QuickAutoCreatePanel : RightUIPanel
{
    public class TempAutoAreaInf
    {
        public Shelves shelvesData;
        public int count = 1;
        public List<string> shelist;
        public List<string> idlist;

        public string id;

        public int level = 5;
        public float x = 1;
        public float y = 1;

        public GameObject itemObj;
    }

    public GameObject p0, p1, pEdit;

    [Header("p0")]
    public Button createFac_Btn;
    public Button createAll_Btn;

    public Button close_Btn;

    public InputField areaCount_Input;
    public InputField spaceX_Input;
    public InputField spaceY_Input;
    public Transform autoAreaListParent;

    [Header("p1")]
    public Text countText;
    public Button createAll_Btn1;
    public Button quitP1Btn;
    public Transform autoAreaListParent1;

    public Button saveBtn;

    [Header("pEdit")]
    public Text pE_name;
    public Button pE_select;
    public Text pE_sheInf;
    public InputField pE_count;
    public InputField pE_level;
    public InputField pE_x, pE_y;
    public Button pE_sureBtn;


    float spaceX = 0, spaceY = 0;
    int areaCount;
    public List<TempAutoAreaInf> tempAutoInf = new List<TempAutoAreaInf>();
    public List<GameObject> areaList = new List<GameObject>();

    GameObject _defaultFacObj;//默认仓库预制体
    GameObject _linePrefab;//默认库位预制体
    GameObject _autoAreaItemPrefab;//列表选项预制体

    GameObject _autoAreaItemEditPrefab;//

    GameObject newFac;//当前生成的仓库


    Factory defaultFac;
    Shelves defaultShelves;


    //
    SaveStockInf_Fac saveData;



 
    public void QuickCreate(ObservableList<TreeNode<TreeViewItem>> facTree, SaveStockInf_Fac mapData = null,bool isOpenEdit = false)
    {
        //计算区域数量
        if (facTree[0].Nodes == null || facTree[0].Nodes.Count == 0)
        {
            Debug.LogError("不存在库位数据！");
            return;
        }

        StoredTreeViewItem facTreeData = facTree[0].Item as StoredTreeViewItem;
        int areaCount = facTree[0].Nodes.Count;
        //根据记录表更新生成数据

        //生成仓库
        CreateFactory(50, 50 / areaCount, facTreeData.Name, facTreeData.ID);
        //按指定数量生成库位(只有默认信息)
        CreateAreaList(areaCount, autoAreaListParent1);
        //指定信息绑定
        SetData2Inf(facTree, mapData);



        //生成货架
        StartCoroutine(CreateAllShe());





        if (isOpenEdit)//打开编辑
        {
            //编辑模式下，生成记录表
            //记录
            //记录仓库数据ID
            saveData = new SaveStockInf_Fac();
            saveData.ID = facTreeData.ID;
            //saveData.modelData = defaultFac;
            saveData.areaList = new List<SaveStockInf_Area>();

            //记录库位信息：
            for (int i = 0; i < tempAutoInf.Count; i++) {
                SaveStockInf_Area saveAreadata = new SaveStockInf_Area();
                saveAreadata.ID = tempAutoInf[i].id;
                saveAreadata.modelData = tempAutoInf[i].shelvesData;
                saveAreadata.level = tempAutoInf[i].level;
                saveAreadata.x = tempAutoInf[i].x.ToString();
                saveAreadata.y = tempAutoInf[i].y.ToString();
                saveAreadata.sheList = new List<SaveStockInf_She>();
                //记录库位下货架信息：
                for (int m = 0; m < tempAutoInf[i].idlist.Count; m++) {
                    SaveStockInf_She savesheldata = new SaveStockInf_She();
                    savesheldata.ID = tempAutoInf[i].idlist[m];//用库位ID作为默认货架ID
                    saveAreadata.sheList.Add(savesheldata);
                }
               
                saveData.areaList.Add(saveAreadata);
            }
            

            p0.SetActive(false);
            p1.SetActive(true);
            OpenUIPanel();
        }
        else
        {

        }
    }


    void SetData2Inf(ObservableList<TreeNode<TreeViewItem>> facTree, SaveStockInf_Fac mapData = null)
    {
        //计算区域信息并填入
        for (int i = 0; i < facTree[0].Nodes.Count; i++)
        {
            TempAutoAreaInf inf = tempAutoInf[i];
            TreeNode<TreeViewItem> areaNode = facTree[0].Nodes[i];
            StoredTreeViewItem areaTree = areaNode.Item as StoredTreeViewItem;
          
            //记录货架数量
            inf.shelist = new List<string>();
            inf.idlist = new List<string>();
            int sheCount;

            //计算货架数量
            if (areaNode.Nodes == null || areaNode.Nodes.Count == 0)//（库位下不存在货架，补充一个）
            {
                sheCount = 1;
                inf.shelist.Add(areaTree.Name + "_默认货架");
                inf.idlist.Add(areaTree.ID);
            }
            else
            {
                sheCount = areaNode.Nodes.Count;
                for (int x = 0; x < sheCount; x++)
                {
                    StoredTreeViewItem sheTree = areaNode.Nodes[x].Item as StoredTreeViewItem;
                    inf.shelist.Add(sheTree.Name);
                    inf.idlist.Add(sheTree.ID);
                }
            }
           
            inf.count = sheCount;
            inf.id = areaTree.ID;

            //展示信息
            string name = areaTree.Name;
            //编辑按钮
            inf.itemObj.transform.GetChild(0).GetComponent<Text>().text = name;
            inf.itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                nowSelectInf = inf;
                //打开编辑面板
                OpenAreaEditPanel(name, nowSelectInf.count, nowSelectInf.level, nowSelectInf.x, nowSelectInf.y);
            });

            //区域信息
            areaList[i].GetComponent<StoredItemArea>().SetName(name);
            areaList[i].GetComponent<StoredItemArea>().SetID(areaTree.ID);

            //记录表信息填充
            if (mapData != null)
            {
                SaveStockInf_Area finddata = mapData.areaList.Find(p => p.ID == areaTree.ID);
                if (finddata != null)
                {
                    inf.shelvesData = finddata.modelData;
                    inf.x = float.Parse(finddata.x);
                    inf.y = float.Parse(finddata.y);
                    inf.level = finddata.level;
                }
            }
           


            //记录库位信息：
            //SaveStockInf_Area saveAreadata = new SaveStockInf_Area();
            //saveAreadata.ID = treeDataArea.ID;
            ////记录库位下货架信息：
            //saveAreadata.sheList = new List<SaveStockInf_She>();
            //if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
            //{
            //    SaveStockInf_She savesheldata = new SaveStockInf_She();
            //    savesheldata.ID = treeDataArea.ID;//用库位ID作为默认货架ID
            //    saveAreadata.sheList.Add(savesheldata);
            //}
            //else
            //{
            //    for (int m = 0; m < tree[0].Nodes[i].Nodes.Count; m++)
            //    {
            //        SaveStockInf_She savesheldata = new SaveStockInf_She();
            //        StoredTreeViewItem sheDataArea = tree[0].Nodes[i].Nodes[m].Item as StoredTreeViewItem;
            //        savesheldata.ID = sheDataArea.ID;
            //        saveAreadata.sheList.Add(savesheldata);
            //    }
            //}
            //saveData.areaList.Add(saveAreadata);




        }
    }
    

    public void DoCheckData(ObservableList<TreeNode<TreeViewItem>> tree,bool isOpenEdit = false)
    {
        if (isActive) return;

        //计算区域数量
        if (tree[0].Nodes == null || tree[0].Nodes.Count==0)
        {
            Debug.LogError("不存在库位数据！");
            return;
        }

        StoredTreeViewItem facTreeData = tree[0].Item as StoredTreeViewItem;
        string id = facTreeData.ID;
        int areaCount = tree[0].Nodes.Count;//库位数量
        countText.text = areaCount.ToString();


        //记录
        //记录仓库数据ID
        saveData = new SaveStockInf_Fac();
        saveData.ID = id;
        //saveData.modelData = defaultFac;
        saveData.areaList = new List<SaveStockInf_Area>();


       // if (IsRecoredDataExist(id))//存在记录表
        {
            //获取记录表数据
            string path = PathExtensions.StreamingAssetsPath() + "/RecoredDir/" + id + ".json";
            StartCoroutine(CommonHelper.GetText(path, (textStr) =>
            {
                SaveStockInf_Fac rs = JsonMapper.ToObject<SaveStockInf_Fac>(textStr);

                //仓库记录信息还原
                //defaultFac = rs.modelData;
                //生成仓库
                CreateFactory(50, 50 / areaCount, tree[0].Item.Name,id);

                //生成库位
                CreateAreaList(areaCount, autoAreaListParent1);

                //计算区域信息并填入
                for (int i = 0; i < areaCount; i++)
                {
                    TempAutoAreaInf inf = tempAutoInf[i];

                    //找记录表 直接赋值
                    StoredTreeViewItem treeDataArea = tree[0].Nodes[i].Item as StoredTreeViewItem;
                    SaveStockInf_Area finddata = rs.areaList.Find(p => p.ID == treeDataArea.ID);
                    if (finddata != null)
                    {
                        inf.shelvesData = finddata.modelData;
                        inf.x = float.Parse(finddata.x);
                        inf.y = float.Parse(finddata.y);
                        inf.level = finddata.level;
                    }

                    //记录货架数量
                    inf.shelist = new List<string>();
                    inf.idlist = new List<string>();

                    int sheCount;
                    //计算货架数量
                    if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
                    {
                        sheCount = 1;
                        inf.shelist.Add(tree[0].Nodes[i].Item.Name+"_默认货架");
                        inf.idlist.Add(treeDataArea.ID);
                    }
                    else
                    {
                        sheCount = tree[0].Nodes[i].Nodes.Count;
                        for(int x = 0; x < sheCount; x++)
                        {
                            inf.shelist.Add(tree[0].Nodes[i].Nodes[x].Item.Name);
                            StoredTreeViewItem sheDataArea = tree[0].Nodes[i].Nodes[x].Item as StoredTreeViewItem;
                            inf.idlist.Add(sheDataArea.ID);
                        }
                    }
                        
                    //默认库位只有一个货架
                    if (sheCount == 0)
                        inf.count = 1;
                    else
                        inf.count = sheCount;


                    //记录库位信息：
                    SaveStockInf_Area saveAreadata = new SaveStockInf_Area();
                    saveAreadata.ID = treeDataArea.ID;
                    //记录库位下货架信息：
                    saveAreadata.sheList = new List<SaveStockInf_She>();
                    if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
                    {
                        SaveStockInf_She savesheldata = new SaveStockInf_She();
                        savesheldata.ID = treeDataArea.ID;//用库位ID作为默认货架ID
                        saveAreadata.sheList.Add(savesheldata);
                    }
                    else
                    {
                        for (int m = 0; m < tree[0].Nodes[i].Nodes.Count; m++)
                        {
                            SaveStockInf_She savesheldata = new SaveStockInf_She();
                            StoredTreeViewItem sheDataArea = tree[0].Nodes[i].Nodes[m].Item as StoredTreeViewItem;
                            savesheldata.ID = sheDataArea.ID;
                            saveAreadata.sheList.Add(savesheldata);
                        }
                    }
                    saveData.areaList.Add(saveAreadata);



                    //展示信息
                    string name = tree[0].Nodes[i].Item.Name;
                    //编辑按钮
                    inf.itemObj.transform.GetChild(0).GetComponent<Text>().text = name;
                    inf.itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        nowSelectInf = inf;

                        //打开编辑面板
                        OpenAreaEditPanel(name, nowSelectInf.count, nowSelectInf.level, nowSelectInf.x, nowSelectInf.y);
                    });

                    //区域信息
                    areaList[i].GetComponent<StoredItemArea>().SetName(name);
                    areaList[i].GetComponent<StoredItemArea>().SetID(treeDataArea.ID);
                }

                if (isOpenEdit)
                {
                    p0.SetActive(false);
                    p1.SetActive(true);
                    OpenUIPanel();
                }

                //重新排列刷新大小
                StartCoroutine(CreateAllShe());
            }));


        }
        //else//不存在记录表 直接按照数据生成
        {
            //生成仓库
            CreateFactory(50, 50 / areaCount, tree[0].Item.Name,id);

            //生成区域列表和库位
            CreateAreaList(areaCount, autoAreaListParent1);

            //计算区域信息并填入
            for (int i = 0; i < areaCount; i++)
            {
                TempAutoAreaInf inf = tempAutoInf[i];
                StoredTreeViewItem treeDataArea = tree[0].Nodes[i].Item as StoredTreeViewItem;

                //记录货架数量
                inf.shelist = new List<string>();
                inf.idlist = new List<string>();
                int sheCount;

                //计算货架数量
                if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
                {
                    sheCount = 1;
                    inf.shelist.Add(tree[0].Nodes[i].Item.Name + "_默认货架");

                    inf.idlist.Add(treeDataArea.ID);
                }
                else
                {
                    sheCount = tree[0].Nodes[i].Nodes.Count;
                    for (int x = 0; x < sheCount; x++)
                    {
                        inf.shelist.Add(tree[0].Nodes[i].Nodes[x].Item.Name);
                        StoredTreeViewItem sheDataArea = tree[0].Nodes[i].Nodes[x].Item as StoredTreeViewItem;
                        inf.idlist.Add(sheDataArea.ID);
                    }
                }
                //默认库位只有一个货架
                if (sheCount == 0)
                    inf.count = 1;
                else
                    inf.count = sheCount;


                //记录库位信息：
                SaveStockInf_Area saveAreadata = new SaveStockInf_Area();
            
                saveAreadata.ID = treeDataArea.ID;
                //记录库位下货架信息：
                saveAreadata.sheList = new List<SaveStockInf_She>();
                if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
                {
                    SaveStockInf_She savesheldata = new SaveStockInf_She();
                    savesheldata.ID = treeDataArea.ID;//用库位ID作为默认货架ID
                    saveAreadata.sheList.Add(savesheldata);
                }
                else
                {
                    for (int m = 0; m < tree[0].Nodes[i].Nodes.Count; m++)
                    {
                        SaveStockInf_She savesheldata = new SaveStockInf_She();
                        StoredTreeViewItem sheDataArea = tree[0].Nodes[i].Nodes[m].Item as StoredTreeViewItem;
                        savesheldata.ID = sheDataArea.ID;
                        saveAreadata.sheList.Add(savesheldata);
                    }
                }
                saveData.areaList.Add(saveAreadata);



                //展示信息
                string name = tree[0].Nodes[i].Item.Name;
                //编辑按钮
                inf.itemObj.transform.GetChild(0).GetComponent<Text>().text = name;
                inf.itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    nowSelectInf = inf;

                    //打开编辑面板
                    OpenAreaEditPanel(name, nowSelectInf.count, nowSelectInf.level, nowSelectInf.x, nowSelectInf.y);

                    //nowItemName = goodsSelectBtn.GetComponentInChildren<Text>();
                    //Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelOpen, "快速生成-货架"));

                });

                //区域信息
                areaList[i].GetComponent<StoredItemArea>().SetName(name);
                areaList[i].GetComponent<StoredItemArea>().SetID(treeDataArea.ID);
            }
            if (isOpenEdit)
            {
                p0.SetActive(false);
                p1.SetActive(true);
                OpenUIPanel();
            }

            //重新排列刷新大小
            StartCoroutine(CreateAllShe());
        }

       

       
    }
    void UpdateSelectGoodsInf()
    {
        string sheinf = string.Format("{0}\r\n{1}/{2}", nowSelectInf.shelvesData.abRes.LabelName, nowSelectInf.shelvesData.abRes.ABPath, nowSelectInf.shelvesData.abRes.AssetName);
        pE_sheInf.text = sheinf;
    }
    void OpenAreaEditPanel(string name,int count = 1,int level =1,float x=1,float y=1)
    {
        p1.SetActive(false);
        pEdit.SetActive(true);

        pE_name.text = name;

        UpdateSelectGoodsInf();

        pE_count.text = count.ToString();
        pE_level.text = level.ToString();
        pE_x.text = x.ToString();
        pE_y.text = y.ToString();
    }

    void CloseAreaEditPanel()
    {
        p1.SetActive(true);
        pEdit.SetActive(false);
    }





    public void CloseTreeQuick()
    {
        //if (isActive)
        {
            //清空
            ClearScene();

            p1.SetActive(false);
            p0.SetActive(true);
            CloseUIPanel();
        }
    }

    void CreateAreaList(int areaCount,Transform parentList)
    {
        //UI选项，缓存数据，库位物体

        //清除列表选项
        for (int i = 0; i < parentList.childCount;)
        {
            DestroyImmediate(parentList.GetChild(i).gameObject);
        }
        //清除缓存数据
        tempAutoInf.Clear();
        //清除物体
        for (int i = 0; i < areaList.Count; i++)
        {
            DestroyImmediate(areaList[i]);
        }
        areaList.Clear();



        Transform areaP = CommonHelper.GetFactoryAreaParent(newFac);
        //仓库尺寸
        float height = newFac.GetComponent<StoredItemFactory>().GetRelativeHeightValue();
        Bounds facBounds = newFac.GetComponent<StoredItemFactory>().GetGroundBounds();
        Vector3 size = facBounds.size;
        //第一个点位置
        //此处相当于接触面放置，加上浮动高度（与正常生成一致）
        Vector3 posOrigin = facBounds.center + new Vector3(-size.x / 2, height + CommonHelper.AreaLineFloatHeight, size.z / 2);//仓库左上角坐标

        //列表生成
        for (int i = 0; i < areaCount; i++)
        {
            TempAutoAreaInf inf = new TempAutoAreaInf();

            GameObject tempItem = Instantiate(_autoAreaItemEditPrefab, parentList);

            inf.itemObj = tempItem;

            inf.shelvesData = defaultShelves;
           


            tempAutoInf.Add(inf);


            //生成线框
            LineRenderer tempLine = Instantiate(_linePrefab, areaP).GetComponent<LineRenderer>();
            //默认
            tempLine.positionCount = 5;
            tempLine.SetPosition(0, posOrigin);
            tempLine.SetPosition(1, posOrigin + new Vector3(size.x / areaCount, 0, 0));
            tempLine.SetPosition(2, posOrigin + new Vector3(size.x / areaCount, 0, -size.z));
            tempLine.SetPosition(3, posOrigin + new Vector3(0, 0, -size.z));
            tempLine.SetPosition(4, posOrigin);
            posOrigin = posOrigin + new Vector3(size.x / areaCount, 0, 0);

            Area Areadata = new Area();
            Areadata.Usage = "货架";
            Areadata.AreaShape = "矩形";
            StoredItemArea areaItem = CommonHelper.BindArea_(tempLine.gameObject, Areadata);
            areaItem.InitOp();
            tempLine.name += i;
            areaList.Add(tempLine.gameObject);
        }
    }




    void GetSaveData()
    {
        //记录库位编辑信息：
        for(int i = 0; i < tempAutoInf.Count; i++)
        {
            saveData.areaList[i].modelData = tempAutoInf[i].shelvesData;
            saveData.areaList[i].level = tempAutoInf[i].level;
            saveData.areaList[i].x = tempAutoInf[i].x.ToString();
            saveData.areaList[i].y = tempAutoInf[i].y.ToString();
        }

        string str = JsonMapper.ToJson(saveData);

        string path = PathExtensions.StreamingAssetsPath() + "/RecoredDir/" + saveData.ID+".json";

        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
        System.IO.File.WriteAllText(path, str);


    }








    protected override void Start()
    {
        base.Start();

        defaultFac = new Factory();
        defaultFac.abRes.LabelName = "默认仓库";
        defaultFac.abRes.ABPath = "默认";
        defaultFac.abRes.AssetName = "默认仓库";


        defaultShelves = new Shelves();
        defaultShelves.abRes.LabelName = "默认货架";
        defaultShelves.abRes.ABPath = "she";
        defaultShelves.abRes.AssetName = "shelves_";
        defaultShelves.Usage = "货架";



        if (_defaultFacObj == null)
        {
            //加载完成后卸载ab包并生成实例
            Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Factory/", defaultFac.abRes.ABPath, defaultFac.abRes.AssetName, (go) =>
            {
                if (CommonHelper.IsSuitFactory(go, defaultFac))
                {
                    _defaultFacObj = go;
                }
                else
                {
                    Debug.LogError("外部默认仓库数据异常！");
                    _defaultFacObj = Resources.Load("obj/DefaultFactory") as GameObject;
                }
            });
        }
        

        if(_linePrefab == null)
            _linePrefab = Resources.Load("AreaLine") as GameObject;

        if (_autoAreaItemPrefab == null)
            _autoAreaItemPrefab = Resources.Load("UIItemPrefab/AutoSheItem") as GameObject;


        if(_autoAreaItemEditPrefab == null)
            _autoAreaItemEditPrefab = Resources.Load("UIItemPrefab/AutoSheItem_Edit") as GameObject;


        quitP1Btn.onClick.AddListener(() => {
            CloseTreeQuick();
        });
        createAll_Btn1.onClick.AddListener(() => {
            StartCoroutine(CreateAllShe());
        });

        //保存场景
        saveBtn.onClick.AddListener(() => {
            GetSaveData();
        });



        createFac_Btn.onClick.AddListener(()=> {
            CreateFactory(15,15,"默认仓库","");
        });

        //close_Btn.onClick.AddListener(() => {
        //    //删除

        //    //关闭面板

        //    CloseUIPanel();

        //});

        //货架生成
        createAll_Btn.onClick.AddListener(() => {

            //Shelves tempData = new Shelves();
            //tempData.ABPath = "she";
            //tempData.AssetName = "shelves_";
            //for (int i = 0; i < tempAutoInf.Count; i++)
            //{
            //    if (tempAutoInf[i].shelvesData == null)
            //        tempAutoInf[i].shelvesData = tempData;
            //}

            StartCoroutine(CreateAllShe());
        });

        //区域生成
        areaCount_Input.onEndEdit.AddListener((str) => {
            if(int.TryParse(str,out areaCount))
            {

                CreateAreaList(areaCount,autoAreaListParent);

                //计算区域信息并填入
                for (int i = 0; i < areaCount; i++)
                {
                    TempAutoAreaInf inf = tempAutoInf[i];

                    //展示信息
                    inf.itemObj.transform.GetChild(0).GetComponent<Text>().text = "默认库位"+i;
                    inf.itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        nowSelectInf = inf;
                        //nowItemName = goodsSelectBtn.GetComponentInChildren<Text>();
                        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelOpen, "快速生成-货架"));

                    });
                }


                //ClearArea();

                ////列表生成
                //for(int i = 0; i < areaCount; i++)
                //{
                //    TempAutoAreaInf inf = new TempAutoAreaInf();
                //    tempAutoInf.Add(inf);
                //    GameObject tempItem = Instantiate(_autoAreaItemPrefab, autoAreaListParent);

                //    //生成参数
                //    InputField[] inputTexts = tempItem.GetComponentsInChildren<InputField>();
                //    inputTexts[0].onEndEdit.AddListener((str) => {
                //        if(!int.TryParse(str, out inf.count))
                //        {
                //            inf.count = 1;
                //        }
                //    });
                //    inputTexts[1].onEndEdit.AddListener((str) =>
                //    {
                //        if(!int.TryParse(str, out inf.level))
                //        {
                //            inf.level = 1;
                //        }
                //    });

                //    //选择货架事件
                //    Button goodsSelectBtn = tempItem.GetComponentInChildren<Button>();
                //    goodsSelectBtn.onClick.AddListener(() => {

                //        nowSelectInf = inf;
                //        nowItemName = goodsSelectBtn.GetComponentInChildren<Text>();
                //        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelOpen, "快速生成-货架"));
                //        //CreateViewPanel.CreateViewEvent cve = new CreateViewPanel.CreateViewEvent();
                //        //cve.title = "货架";
                //        //cve.ac = (objs) => {
                //        //    ////for
                //        //};


                //        //打开货物选中面板

                //        //选中后的信息记录在inf里
                //        //inf.shelvesData
                //    });


                //}

                ////平均生成区域
                //CreateArea();
            }
        });


        spaceX_Input.onEndEdit.AddListener((str) => {
            if(!float.TryParse(str, out spaceX))
            {
                spaceX = 0;
            }
        });
        spaceY_Input.onEndEdit.AddListener((str) =>
        {
            if(!float.TryParse(str, out spaceY))
            {
                spaceY = 0;
            }
        });


        pE_select.onClick.AddListener(() => {
            isCreatePanelOpen = !isCreatePanelOpen;
            if (isCreatePanelOpen)
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelOpen, "快速生成-货架"));
            else
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelClose, "快速生成-货架"));
           
        });
        pE_sureBtn.onClick.AddListener(CloseAreaEditPanel);


        pE_count.onEndEdit.AddListener((str) => {
            if (nowSelectInf != null)
            {
                if(!int.TryParse(str,out nowSelectInf.count))
                {
                    nowSelectInf.count = 1;
                }
            }
        });
        pE_level.onEndEdit.AddListener((str) =>
        {
            if (nowSelectInf != null)
            {
                if (!int.TryParse(str, out nowSelectInf.level))
                {
                    nowSelectInf.level = 1;
                }
            }
        });
        pE_x.onEndEdit.AddListener((str) =>
        {
            if (nowSelectInf != null)
            {
                if (!float.TryParse(str, out nowSelectInf.x))
                {
                    nowSelectInf.x = 1;
                }
            }
        });
        pE_y.onEndEdit.AddListener((str) =>
        {
            if (nowSelectInf != null)
            {
                if (!float.TryParse(str, out nowSelectInf.y))
                {
                    nowSelectInf.y = 1;
                }
            }
        });


      

        CreateShelvesPanel();
    }

    bool isCreatePanelOpen = false;



    TempAutoAreaInf nowSelectInf;
    Text nowItemName;
    void CreateShelvesPanel()
    {
        GameObject panelTempObj = Resources.Load("Panel/SelectView") as GameObject;
        //面板
        panelTempObj.name = "QuickShelvesListPanel";
        GameObject panelTemp = Instantiate(panelTempObj, transform.parent);
        panelTemp.name = "QuickShelvesListPanel";
        CreateViewPanel cvPtemp = panelTemp.GetComponent<CreateViewPanel>();
        List<Shelves> tagData = Manager3DStored.Instance._shelves.FindAll(p => p.Usage == "货架");
        cvPtemp.CreateViewByDate("快速生成-货架", Resources.Load("UIItemPrefab/facItem") as GameObject, tagData.Count, (objs) =>
        {
            for (int i = 0; i < objs.Count; i++)
            {
                //展示内容
                objs[i].GetComponentInChildren<Text>().text = tagData[i].abRes.LabelName;
                objs[i].name = tagData[i].abRes.LabelName;
                //点击事件
                string abName = tagData[i].abRes.ABPath;
                string assetName = tagData[i].abRes.AssetName;
                Shelves data = tagData[i];
                objs[i].GetComponent<Button>().onClick.RemoveAllListeners();
                objs[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (nowSelectInf != null)
                    {
                        nowSelectInf.shelvesData = data;
                        UpdateSelectGoodsInf();
                        //nowItemName.text = data.LabelName;
                        cvPtemp.CloseUIPanel();
                        isCreatePanelOpen = false;

                        //Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelClose, "快速生成-货架"));
                    }
                });
            }
        });
    }




    //void ReplaceObjEvent(List<GameObject> objs)
    //{
    //    for(int i = 0; i < objs.Count; i++)
    //    {
    //        Button tempBtn = objs[i].GetComponent<Button>();
    //        tempBtn.onClick.AddListener()
    //    }

    //}













    

    void CreateFactory(float sizeX,float sizeY,string name,string id)
    {
        if (newFac == null)
        {
            newFac = Instantiate(_defaultFacObj, CommonHelper.GetFactoryRoot());
            CommonHelper.CreateDefaultFactorySize(newFac,sizeX,sizeY);//指定仓库尺寸
            defaultFac.Scale = CommonHelper.Vector3toString(new Vector3(sizeX,1,sizeY));
            StoredItemFactory facItem = CommonHelper.BindFactory_(newFac, defaultFac);
            facItem.InitOp();
            facItem.SetName(name);
            facItem.SetID(id);
        }
        else
        {
            Debug.LogError("已经存在！");
        }
    }

    void CreateAreaObject(int count)
    {
       
    }

    IEnumerator CreateAllShe()
    {

        int waitNumber = 0;
        int maxNumber = areaList.Count;
        //生成每个区域对应的货架
        for (int i = 0; i < areaList.Count; i++)
        {
            //删除已有货架
            DestroyImmediate(CommonHelper.GetFactoryShelvesParent(areaList[i]).gameObject);



            TempAutoAreaInf inf = tempAutoInf[i];

            int index = i;
            //加载对应ab包
            //加载完成后卸载ab包并生成实例
            Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Shelves/", inf.shelvesData.abRes.ABPath, inf.shelvesData.abRes.AssetName, (go) =>
            {
                if (CommonHelper.IsSuitShelves(go, inf.shelvesData))
                {
                    try
                    {
                        CreateShelves(inf, areaList[index].GetComponent<LineRenderer>(), go, inf.x, inf.y);
                        //Debug.Log(waitNumber);
                    }
                    catch
                    {
                        Debug.LogError("参数异常" + index);
                    }
                    finally
                    {
                        waitNumber++;
                    }
                }
                else
                {
                    Debug.LogError("生成货架失败！");
                    waitNumber++;
                }

              
            });
        }

        while (waitNumber < maxNumber)
        {
            yield return null;
        }


        //刷新仓库大小
        RefreshFac();
        //库位重新排列
        ReSetAreaPos();


        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent( CoreEventId.CameraLookAt, newFac));

    }








    void RefreshFac()
    {
        //面积 x=所有区域x  y=最大区域y
        float newX = 0, newY = 0;
        for (int i = 0; i < areaList.Count; i++)
        {
            Bounds bound = CommonHelper.GetObjBounds(areaList[i]);
            newX += bound.size.x;
            if (newY < bound.size.z)
                newY = bound.size.z;
        }

        //重置大小
        CommonHelper.CreateDefaultFactorySize(newFac,newX,newY);
        //数据改变
        Factory ttfac = newFac.GetComponent<StoredItemBase>().GetData<Factory>();
        ttfac.Scale = CommonHelper.Vector3toString(new Vector3(newX, 1, newY));
        newFac.GetComponent<StoredItemBase>().SetData(ttfac);
        //defaultdata.Scale = CommonHelper.Vector3toString(new Vector3(sizeX, 1, sizeY));
        //Transform floorObj = newFac.transform.GetChild(0);
        //floorObj.localScale = new Vector3(newX * 0.1f, 1, newY * 0.1f);
        //newFac.GetComponent<BoxCollider>().size = new Vector3(newX, 0, newY);
    }
    void ReSetAreaPos()
    {

        for (int i = 0; i < areaList.Count; i++)
            areaList[i].transform.SetParent(null);

        //仓库地面
        Bounds facBounds = newFac.GetComponent<BoxCollider>().bounds;

        Vector3 originPos = facBounds.center + new Vector3(-(facBounds.size.x) / 2, 0, (facBounds.size.z) / 2);//区域边界左上角
        Debug.Log(originPos);
        float x = 0, y = 0;
        for (int i = 0; i < areaList.Count; i++)
        {
            //获取区域尺寸
            Bounds bound = areaList[i].GetComponent<StoredItemArea>().GetBoxBounds();

            //Bounds bound = CommonHelper.GetObjBounds(areaList[i]);
            if (i > 0)
                x += areaList[i - 1].GetComponent<StoredItemArea>().GetBoxBounds().size.x;


            //世界坐标 = 自身位置+box.center;
            //这里减去bounds.center是为了将box移动到原点位置(即初始的偏移量)
            Vector3 newPos = originPos + new Vector3(x + bound.size.x / 2, 0, -bound.size.z / 2) - bound.center;
            newPos.y = areaList[i].transform.position.y;
            areaList[i].transform.position = newPos;

            //TODO:不移动，只改点位

            //Debug.LogError(newPos);
            //for(int tt = 0; tt < areaList[i].GetComponent<LineRenderer>().positionCount; tt++)
            //{
            //    Vector3 tempV = areaList[i].GetComponent<LineRenderer>().GetPosition(tt) + originPos + new Vector3(x + bound.size.x / 2, 0, -bound.size.z / 2);
            //    areaList[i].GetComponent<LineRenderer>().SetPosition(tt,tempV);
            //}

            //areaList[i].transform.position = originPos - bound.center;
        }
        //areaList[0].transform.position = originPos;

        for (int i = 0; i < areaList.Count; i++)
            areaList[i].transform.SetParent(CommonHelper.GetFactoryAreaParent(newFac));
    }

    //177 499 1993//质数

    void GetSuitRowColumn(int count, out int row, out int column)
    {
        List<int> getlineC = new List<int>();
        int xx = 1;
        while (xx <= count)
        {
            if (count % xx == 0)
            {
                getlineC.Add(xx);
            }
            xx++;
        }

        //质数转换//避免质数类型 过长
        //if (count > 6 && getlineC.Count == 2)
        //{
        //    GetSuitRowColumn(count+1,out row,out column);
        //}
        //else
        {
            if (getlineC.Count % 2 == 0)//偶数个
            {
                int index = Mathf.FloorToInt(getlineC.Count / 2.0f);
                row = getlineC[index - 1];
                column = getlineC[index];
            }
            else//奇数个
            {
                int index = Mathf.FloorToInt(getlineC.Count / 2.0f);
                row = column = getlineC[index];
            }
        }

        //避免行列差距过大 
        if (Mathf.Abs(row - column) > 5)
        {
            GetSuitRowColumn(count + 1, out row, out column);
        }
        //TODO：按显示效果的话，可以算长度

    }



    void CreateShelves(TempAutoAreaInf inf, LineRenderer area, GameObject she, float x, float y)
    {
        int count = inf.count;
        if (inf.shelist != null)
            count = inf.shelist.Count;

        Shelves data = inf.shelvesData;
        int level = inf.level;
            
        //求合适的行列
        int lineCount;
        int maxLineCount;

        GetSuitRowColumn(count, out lineCount, out maxLineCount);

        //maxLineCount = Mathf.CeilToInt(count*1.0f / lineCount);//每行个数

        //Debug.Log(maxLineCount);
        int splineCount = maxLineCount + 1;//间距数量


        //计算区域尺寸
        Bounds shelvesBounds = CommonHelper.GetObjBounds(she);
        float xLength = shelvesBounds.size.x * maxLineCount + x * splineCount;
        float yLength = shelvesBounds.size.z * lineCount + y * (lineCount + 1);

        //指定点位绘制中心世界坐标
        Vector3 lineViewCenter = Vector3.up * area.GetComponent<StoredItemArea>().GetViewHeight();
        //重新生成尺寸
        CommonHelper.CreateAreaBoundsByLength(area, xLength, yLength, lineViewCenter);
        //刷新boxcollider
        area.GetComponent<StoredItemArea>().RefreshBoxCollider();

        Bounds areaBounds = area.GetComponent<StoredItemArea>().GetBoxBounds();
        //if(areaBounds.size.x<xLength || areaBounds.size.z < yLength)//重新生成区域


        Vector3 originPos = areaBounds.center + new Vector3(-(areaBounds.size.x ) / 2, 0, (areaBounds.size.z ) / 2);//区域边界左上角
        Debug.Log(originPos);
        Vector3 objoriginPos = originPos + new Vector3(x + shelvesBounds.size.x / 2, 0, -y - shelvesBounds.size.z / 2);//物体位置左上角
        Transform objParent = CommonHelper.GetFactoryShelvesParent(area.gameObject);
        int allCount = 0;
        for (int i = 0; i < lineCount && allCount < count; i++)
        {
            for (int m = 0; m < maxLineCount && allCount < count; m++)
            {
                GameObject objtemp = Instantiate(she, objParent);

                StoredItemShelves sheItem = CommonHelper.BindShelves_(objtemp, data);
                sheItem.InitOp(inf.shelist[allCount], objoriginPos + new Vector3((shelvesBounds.size.x + x) * m, 0, -(shelvesBounds.size.z + y) * i));
                sheItem.SetID(inf.idlist[allCount]);
                //objtemp.name += i + "_" + m;
                //objtemp.transform.position = objoriginPos + new Vector3((shelvesBounds.size.x + x) * m, 0, -(shelvesBounds.size.z + y) * i);

             
                //Manager3DStored.Instance.GetStoredComponent<DragManager>().GetOrAddDragItem(objtemp).InitDragFunc(CommonHelper.TempShelves, CommonHelper.TempArea);
                //JDataHelper dataHelper = objtemp.AddComponent<JDataHelper>();
                //dataHelper.SetData<Shelves>(data);

                //存在分层
                if (level > 1)
                {
                    ShelvesLevel levelComponent = objtemp.AddComponent<ShelvesLevel>();
                    levelComponent.SaveOrigin(objtemp);
                    levelComponent.SetLevel_new(level);

                    //货物部分--
                    //for (int ii = 0; ii < levelComponent.level; ii++)
                    //{
                    //    //Goods data = dataInfs[i].Datasource;
                    //    GDGoods tempgd = objtemp.transform.GetChild(ii).Find("GD").AddComponent<GDGoods>();
                    //    tempgd.InitSelf();
                    //    //if (data != null)
                    //    //{
                    //    //    int index = i;
                    //    //    Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Goods/", data.ABPath, data.AssetName, (go) =>
                    //    //    {
                    //    //        tempgd.CreateAddN(data, go, dataInfs[index].count);
                    //    //    });
                    //    //}

                    //}
                }
                else
                {
                    //货物
                    //objtemp.transform.Find("GD").AddComponent<GDGoods>().InitSelf();
                }


                //objtemp.transform.position = CommonHelper.GetObjPositionByGroundHeight(objtemp.transform.position, objtemp, areaBounds.center.y);




                allCount++;
            }
        }

    }























    //清空
    void ClearScene()
    {
        //仓库删除
        Destroy(newFac);
        newFac = null;

        //

    }










    void ClearArea()
    {
        //清除列表
        for(int i =0;i< autoAreaListParent.childCount;)
        {
            DestroyImmediate(autoAreaListParent.GetChild(i).gameObject);
        }
        tempAutoInf.Clear();
        
        //删除物体
        for (int i = 0; i < areaList.Count;i++)
        {
            DestroyImmediate(areaList[i]);
        }
        areaList.Clear();
    }


}

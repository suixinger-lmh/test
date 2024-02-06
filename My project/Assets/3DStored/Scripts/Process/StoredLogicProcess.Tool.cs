using Custom.UIWidgets;
using LitJson;
using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class StoredLogicProcess
{
    [DllImport("__Internal")]
    private static extern void HelloString(string str);
    [DllImport("__Internal")]
    private static extern void SaveData(string str);

    public void SaveRecordData()
    {
        string str = JsonMapper.ToJson(DoScene2RecordInf());
#if UNITY_EDITOR
        string path = PathExtensions.StreamingAssetsPath() + "/SceneData/RecordData.json";
        Debug.Log(path);
       
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
        System.IO.File.WriteAllText(path, str);
#elif UNITY_WEBGL

        SaveData(str);
        //HelloString( str);
        //Application.ExternalCall("SaveData", str, "name");

#endif
    }

    public void ReadRecordData()
    {
        string path = PathExtensions.StreamingAssetsPath() + "/SceneData/RecordData.json";

        StartCoroutine(CommonHelper.GetText(path, (textStr) => {
            RecordStored rs = JsonMapper.ToObject<RecordStored>(textStr);
            DoReadCreate(rs);
        }));
    }












    #region 保存

    //将当前场景转为存储数据
    public RecordStored DoScene2RecordInf()
    {
        RecordStored recordStored = new RecordStored();
        recordStored.factoryRecordInfs = new List<FactoryRecordInf>();
        //获取仓库
        for (int i = 0; i < _FactorRoot.childCount; i++)
        {
            FactoryRecordInf facRecord = new FactoryRecordInf();
            StoredItemBase facHelper = _FactorRoot.GetChild(i).GetComponent<StoredItemBase>();

            facRecord.Name = facHelper.GetName();//.gameObject.name;//名称
            facRecord.ID = facHelper.GetID();
            facRecord.Datasource = facHelper.GetData<Factory>();//数据源
            facRecord.Position = facHelper.GetPositionStr();//位置
            facRecord.AreaRecordInfs = SaveArea(facHelper.gameObject);//获取区域记录

            recordStored.factoryRecordInfs.Add(facRecord);
        }

        return recordStored;
    }

    List<AreaRecordInf> SaveArea(GameObject factory)
    {
        List<AreaRecordInf> tempAreaRecordInfs = new List<AreaRecordInf>();
        //获取区域
        Transform tempAreaP = CommonHelper.GetFactoryAreaParent(factory);
        for (int i = 0; i < tempAreaP.childCount; i++)
        {
            AreaRecordInf tempInf = new AreaRecordInf();
            StoredItemBase jd = tempAreaP.GetChild(i).GetComponent<StoredItemBase>();
            LineRenderer lineRender = tempAreaP.GetChild(i).GetComponent<LineRenderer>();
           

            tempInf.Name = jd.GetName();//lineRender.gameObject.name;
            tempInf.ID = jd.GetID();
            tempInf.PointPositions = new List<string>();
            tempInf.Position = jd.GetPositionStr();//CommonHelper.Vector3toString(lineRender.transform.position);//位置
            tempInf.Datasource = jd.GetData<Area>();
            for (int m = 0; m < lineRender.positionCount; m++)
            {
                tempInf.PointPositions.Add(CommonHelper.Vector3toString(lineRender.GetPosition(m)));
            }
            tempInf.ShelvesRecordInfs = SaveShelves(lineRender.gameObject);

            tempAreaRecordInfs.Add(tempInf);
        }
        return tempAreaRecordInfs;
    }


    List<ShelvesRecordInf> SaveShelves(GameObject area)
    {
        List<ShelvesRecordInf> tempShelvesRecordInfs = new List<ShelvesRecordInf>();
        //获取区域
        Transform tempShelvesP = CommonHelper.GetFactoryShelvesParent(area);
        for (int i = 0; i < tempShelvesP.childCount; i++)
        {
            //基础信息记录
            ShelvesRecordInf tempInf = new ShelvesRecordInf();
            //每个货架的信息
            StoredItemBase dataHelper = tempShelvesP.GetChild(i).GetComponent<StoredItemBase>();

            tempInf.ID = dataHelper.GetID();
            tempInf.Name = dataHelper.GetName();//.gameObject.name;
            tempInf.Position = dataHelper.GetPositionStr();//CommonHelper.Vector3toString(dataHelper.transform.position);
            tempInf.Datasource = dataHelper.GetData<Shelves>();


            //分级记录
            ShelvesLevel sl = tempShelvesP.GetChild(i).GetComponent<ShelvesLevel>();
            if (sl != null && sl.hasLevel)//存在分级
            {
                tempInf.LevleRecord = new LevelRecord();
                tempInf.LevleRecord.Level = sl.level;
            }

            //货物记录
            tempInf.GoodsRecordInfs = SaveGoods(dataHelper.gameObject);


            tempShelvesRecordInfs.Add(tempInf);
        }



        return tempShelvesRecordInfs;
    }

    List<GoodsRecordInf> SaveGoods(GameObject shelves)
    {
        List<GoodsRecordInf> goodsRecordInfs = new List<GoodsRecordInf>();
        //货物记录
        GDGoods[] gds = shelves.GetComponentsInChildren<GDGoods>();
        if (gds == null || gds.Length == 0)
            return null;
        for (int i = 0; i < gds.Length; i++)//货架每层的信息
        {
            GoodsRecordInf goodInf = new GoodsRecordInf();
            goodInf.Name = gds[i].LineName;
            goodInf.Names = gds[i].GetNames();
            goodInf.Datasource = gds[i].GetGoods();
            goodInf.count = gds[i].GetNowCount();

            goodsRecordInfs.Add(goodInf);
        }
        return goodsRecordInfs;
    }



    #endregion



    #region 读取


    RecordStored DoReadFile()
    {
        string str = System.IO.File.ReadAllText(@"C:\Users\PC5837\Desktop\temp.json");
        RecordStored rs = JsonMapper.ToObject<RecordStored>(str);
        return rs;
    }



    //根据数据生成场景
    void DoReadCreate(RecordStored rs)
    {
        GameObject lineRenderPrefab = Resources.Load("AreaLine") as GameObject;


        for (int i = 0; i < rs.factoryRecordInfs.Count; i++)
        {
            List<AreaRecordInf> areaRecord = rs.factoryRecordInfs[i].AreaRecordInfs;
            //创建 仓库
            CreateFactoryFromJson(rs.factoryRecordInfs[i], (factoryIns) =>
            {
                // (仓库加载完成后创建区域)
                for (int m = 0; m < areaRecord.Count; m++)
                {
                    CreateAreaFromJson(areaRecord[m], factoryIns, lineRenderPrefab);
                }

            });

        }
    }




    void CreateFactoryFromJson(FactoryRecordInf dataInf, UnityAction<GameObject> afterLoad)
    {
        Factory data = dataInf.Datasource;
        Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Factory/", data.abRes.ABPath, data.abRes.AssetName, (go) =>
        {
            if (CommonHelper.IsSuitFactory(go, data))
            {
                //创建 仓库
                GameObject insObj = Instantiate(go, _FactorRoot);
                //默认仓库
                if(data.abRes.AssetName == "默认仓库")
                {
                    Vector3 size = CommonHelper.String2Vector3(data.Scale);
                    CommonHelper.CreateDefaultFactorySize(insObj, size.x, size.z);//指定仓库尺寸
                }

                StoredItemFactory facItem = CommonHelper.BindFactory_(insObj, data);
                facItem.InitOp(dataInf.Name, CommonHelper.String2Vector3(dataInf.Position));
                
                //insObj.transform.position = CommonHelper.String2Vector3(dataInf.Position);
                //insObj.name = dataInf.Name;

                //完成后执行
                afterLoad(insObj);
            }
            else
            {
                Debug.LogError("生成失败！资源异常！");
            }
        });
    }

    void CreateAreaFromJson(AreaRecordInf dataInf, GameObject facObj, GameObject linePrefab)
    {
        //区域线生成
        Transform tempAreaP = CommonHelper.GetFactoryAreaParent(facObj);
        LineRenderer tempLine = Instantiate(linePrefab, tempAreaP).GetComponent<LineRenderer>();

        tempLine.gameObject.name = dataInf.Name;
        tempLine.positionCount = dataInf.PointPositions.Count;
        for (int i = 0; i < dataInf.PointPositions.Count; i++)
        {
            tempLine.SetPosition(i, CommonHelper.String2Vector3(dataInf.PointPositions[i]));
        }
        //tempLine.startColor = Color.yellow;
        //tempLine.endColor = Color.yellow;

        tempLine.transform.position = CommonHelper.String2Vector3(dataInf.Position);//位置

        //数据绑定
        StoredItemArea areaItem = CommonHelper.BindArea_(tempLine.gameObject, dataInf.Datasource);
        areaItem.InitOp();


        CreateShelvesFromJson(dataInf.ShelvesRecordInfs, tempLine.gameObject);
    }


    void CreateShelvesFromJson(List<ShelvesRecordInf> dataInfs, GameObject areaObj)
    {
        //货架挂点
        Transform tempShelvesP = CommonHelper.GetFactoryShelvesParent(areaObj);

        //生成货架
        for (int i = 0; i < dataInfs.Count; i++)
        {
            Shelves data = dataInfs[i].Datasource;
            ShelvesRecordInf dataRecordInf = dataInfs[i];
            Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Shelves/", data.abRes.ABPath, data.abRes.AssetName, (go) =>
            {
                if (CommonHelper.IsSuitShelves(go,data))
                {
                    GameObject insObj = Instantiate(go, tempShelvesP);

                    StoredItemShelves sheItem = CommonHelper.BindShelves_(insObj, data);
                    sheItem.InitOp(dataRecordInf.Name, CommonHelper.String2Vector3(dataRecordInf.Position));
                    //基础信息
                    //insObj.transform.position = CommonHelper.String2Vector3(dataRecordInf.Position);
                    //insObj.name = dataRecordInf.Name;

                  
                    ////组件添加
                    //Manager3DStored.Instance.GetStoredComponent<DragManager>().GetOrAddDragItem(insObj).InitDragFunc(CommonHelper.TempShelves, CommonHelper.TempArea);
                    //insObj.AddComponent<JDataHelper>().SetData<Shelves>(data);


                    //存在分层
                    if (dataRecordInf.LevleRecord != null && dataRecordInf.LevleRecord.Level > 1)
                    {
                        ShelvesLevel levelComponent = insObj.AddComponent<ShelvesLevel>();
                        levelComponent.SaveOrigin(insObj);
                        levelComponent.SetLevel_new(dataRecordInf.LevleRecord.Level);
                    }

                    //货物添加
                    CreateGoodsFromJson(dataRecordInf.GoodsRecordInfs, insObj);



                    //生成后自动选中
                    //DoSelectObj(insObj, data);
                    //DoLookCreateObj(insObj);
                }
                else
                {
                    Debug.LogError("生成失败！资源异常！");
                }
            });

        }



    }



    void CreateGoodsFromJson(List<GoodsRecordInf> dataInfs, GameObject shelvels)
    {
        //存在货物信息
        if (dataInfs != null && dataInfs.Count > 0)
        {
            ShelvesLevel levelComponent = shelvels.GetComponent<ShelvesLevel>();
            if (levelComponent != null && levelComponent.hasLevel)//分层
            {
                //记录每一层的挂点物体
                for (int i = 0; i < levelComponent.level; i++)
                {
                    Goods data = dataInfs[i].Datasource;
                    GDGoods tempgd = shelvels.transform.GetChild(i).Find("GD").AddComponent<GDGoods>();
                    tempgd.InitSelf();
                    if (data != null)
                    {
                        int index = i;
                        Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Goods/", data.abRes.ABPath, data.abRes.AssetName, (go) =>
                        {
                            tempgd.CreateAddN(data, go, dataInfs[index].count);
                        });
                    }

                }
            }
            else//未分层
            {
                Goods data = dataInfs[0].Datasource;
                GDGoods tempgd = shelvels.transform.Find("GD").AddComponent<GDGoods>();
                tempgd.InitSelf();
                Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Goods/", data.abRes.ABPath, data.abRes.AssetName, (go) =>
                {
                    tempgd.CreateAddN(data, go, dataInfs[0].count);
                });
            }
        }
    }

    #endregion


    #region 展示



    ObservableList<TreeNode<TreeViewItem>> facList = new ObservableList<TreeNode<TreeViewItem>>();
    ObservableList<TreeNode<TreeViewItem>> areaList = new ObservableList<TreeNode<TreeViewItem>>();
    ObservableList<TreeNode<TreeViewItem>> shelvesList = new ObservableList<TreeNode<TreeViewItem>>();
    ObservableList<TreeNode<TreeViewItem>> goodsList = new ObservableList<TreeNode<TreeViewItem>>();

    //根据当前场景 获取展示列表信息
    //public ObservableList<TreeNode<TreeViewItem>> CreateViewListByScene()
    //{
    //    //获取数据
    //    RecordStored record = DoScene2RecordInf();

    //    return GetFactoryList(record.factoryRecordInfs);

    //    ObservableList<TreeNode<TreeViewItem>> treeNodes = new ObservableList<TreeNode<TreeViewItem>>();
    //    TreeViewItem tvi = new TreeViewItem("仓储信息", null);
    //    TreeNode<TreeViewItem> treeNode = new TreeNode<TreeViewItem>(tvi);

    //    //仓库信息
    //    ObservableList<TreeNode<TreeViewItem>> nodes = new ObservableList<TreeNode<TreeViewItem>>();
    //    nodes.BeginUpdate();
    //    for (int i = 0; i < record.factoryRecordInfs.Count; i++)
    //    {
    //        TreeViewItem tvii = new TreeViewItem(record.factoryRecordInfs[i].Name, null);
    //        TreeNode<TreeViewItem> treeNode_xx = new TreeNode<TreeViewItem>(tvii);

    //        treeNode_xx.Nodes = GetAreaList(record.factoryRecordInfs[i].AreaRecordInfs);
    //        nodes.Add(treeNode_xx);
    //    }
    //    nodes.EndUpdate();

    //    treeNode.Nodes = nodes;
    //    treeNode.IsExpanded = true;
    //    treeNodes.BeginUpdate();
    //    treeNodes.Add(treeNode);
    //    treeNodes.EndUpdate();
    //    return treeNodes;
    //}

    #region TreeViewItem变体，加唯一标识，只有name不保险

    public ObservableList<TreeNode<TreeViewItem>> CreateViewListByScene_new()
    {
        //获取数据
        RecordStored record = DoScene2RecordInf();

        return GetFactoryList_new(record.factoryRecordInfs);
    }
    ObservableList<TreeNode<TreeViewItem>> GetFactoryList_new(List<FactoryRecordInf> factoryRecordInfs)
    {
        ObservableList<TreeNode<TreeViewItem>> nodes = new ObservableList<TreeNode<TreeViewItem>>();

        nodes.BeginUpdate();
        for (int i = 0; i < factoryRecordInfs.Count; i++)
        {
            StoredTreeViewItem tvi = new StoredTreeViewItem(factoryRecordInfs[i].Name, factoryRecordInfs[i].ID, null);
            TreeNode<TreeViewItem> treeNode = new TreeNode<TreeViewItem>(tvi);

            treeNode.Nodes = GetAreaList_new(factoryRecordInfs[i].AreaRecordInfs);

            nodes.Add(treeNode);

            //额外记录
            //facList.Add(treeNode);
        }
        nodes.EndUpdate();
        return nodes;

    }
    ObservableList<TreeNode<TreeViewItem>> GetAreaList_new(List<AreaRecordInf> areaRecordInfs)
    {
        ObservableList<TreeNode<TreeViewItem>> nodes = new ObservableList<TreeNode<TreeViewItem>>();

        nodes.BeginUpdate();
        for (int i = 0; i < areaRecordInfs.Count; i++)
        {
            StoredTreeViewItem tvi = new StoredTreeViewItem(areaRecordInfs[i].Name, areaRecordInfs[i].ID, null);
            TreeNode<TreeViewItem> treeNode = new TreeNode<TreeViewItem>(tvi);

            treeNode.Nodes = GetShelvesList_new(areaRecordInfs[i].ShelvesRecordInfs);

            nodes.Add(treeNode);

            //areaList.Add(treeNode);
        }
        nodes.EndUpdate();
        return nodes;
    }
    ObservableList<TreeNode<TreeViewItem>> GetShelvesList_new(List<ShelvesRecordInf> shelvesRecordInfs)
    {
        ObservableList<TreeNode<TreeViewItem>> nodes = new ObservableList<TreeNode<TreeViewItem>>();
        nodes.BeginUpdate();

        for (int i = 0; i < shelvesRecordInfs.Count; i++)
        {
            StoredTreeViewItem tvi = new StoredTreeViewItem(shelvesRecordInfs[i].Name, shelvesRecordInfs[i].ID, null);
            TreeNode<TreeViewItem> treeNode = new TreeNode<TreeViewItem>(tvi);

            treeNode.Nodes = GetGoodsList_new(shelvesRecordInfs[i].GoodsRecordInfs);

            nodes.Add(treeNode);

            //shelvesList.Add(treeNode);
        }
        nodes.EndUpdate();
        return nodes;
    }


    ObservableList<TreeNode<TreeViewItem>> GetGoodsList_new(List<GoodsRecordInf> goodsRecordInfs)
    {
        if (goodsRecordInfs == null) return null;
        ObservableList<TreeNode<TreeViewItem>> nodes = new ObservableList<TreeNode<TreeViewItem>>();
        nodes.BeginUpdate();

        //只是货物挂点量
        for (int i = 0; i < goodsRecordInfs.Count; i++)
        {
            //没有货物的行不显示
            if (goodsRecordInfs[i].count == 0)
                continue;

            //层级名称包含信息，下划线截取
            StoredTreeViewItem tvi = new StoredTreeViewItem("GD_"+(i+1),"货物ID", null);
            TreeNode<TreeViewItem> treeNode = new TreeNode<TreeViewItem>(tvi);
            nodes.Add(treeNode);

            //goodsList.Add(treeNode);
        }
        nodes.EndUpdate();
        return nodes;
    }


    #endregion


    /// <summary>
    /// 各个仓库独立树结构字典
    /// </summary>
    Dictionary<string, ObservableList<TreeNode<TreeViewItem>>> facListTemp = new Dictionary<string, ObservableList<TreeNode<TreeViewItem>>>();


    //解析后台数据，生成树结构
    public ObservableList<TreeNode<TreeViewItem>> CreateStockList(StockInf rs)
    {
        //清空字典
        facListTemp.Clear();


        //组织树
        ObservableList<TreeNode<TreeViewItem>> stored = new ObservableList<TreeNode<TreeViewItem>>();
        //组织信息
        StoredTreeViewItem itemf = new StoredTreeViewItem(rs.STOCKORGNAME, rs.STOCKORGID);
        TreeNode<TreeViewItem> facf = new TreeNode<TreeViewItem>(itemf);
        stored.Add(facf);
        //组织下的仓库
        facf.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        for (int i = 0; i < rs.WAREHOUSES.Count; i++)
        {
            //仓库信息
            StoredTreeViewItem item = new StoredTreeViewItem(rs.WAREHOUSES[i].name, rs.WAREHOUSES[i].id);
            TreeNode<TreeViewItem> fac = new TreeNode<TreeViewItem>(item);
            facf.Nodes.Add(fac);

            //每个仓库单独的树结构记录在仓库字典中
            ObservableList<TreeNode<TreeViewItem>> factoryTreeData = new ObservableList<TreeNode<TreeViewItem>>();
            //仓库的树结构
            TreeNode<TreeViewItem> fac2 = new TreeNode<TreeViewItem>(item);
            //仓库下的库位
            fac2.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
            
            factoryTreeData.Add(fac2);

            //不存在（补充一个默认库位）
            if (rs.WAREHOUSES[i].shelf == null || rs.WAREHOUSES[i].shelf.Count < 1)
            {
                //不存在库位数据的，自动补充一个默认库位，id为仓库id
                StoredTreeViewItem iitem = new StoredTreeViewItem("默认库位", rs.WAREHOUSES[i].id);
                TreeNode<TreeViewItem> ffac = new TreeNode<TreeViewItem>(iitem);
                fac2.Nodes.Add(ffac);

                //补充一个默认货架
                ffac.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
                StoredTreeViewItem iiitem = new StoredTreeViewItem("默认货架", rs.WAREHOUSES[i].id);
                TreeNode<TreeViewItem> fffac = new TreeNode<TreeViewItem>(iiitem);
                ffac.Nodes.Add(fffac);
            }
            else
            {
                //存在 （上级为空或者上级为仓库id的为库位，上级为库位的，作为该库位货架）
                //遍历一遍，将库位生成，并将上级对应字典生成
                Dictionary<string, List<Shelf>> pIDKV = new Dictionary<string, List<Shelf>>();
                for (int m = 0; m < rs.WAREHOUSES[i].shelf.Count; m++)
                {
                    Shelf tempShe = rs.WAREHOUSES[i].shelf[m];
                    if(string.IsNullOrEmpty(tempShe.pID) || tempShe.pID == rs.WAREHOUSES[i].id)//父id为空或父ID为仓库id
                    {
                        //记录为库位
                        StoredTreeViewItem iitem = new StoredTreeViewItem(tempShe.name, tempShe.id);
                        TreeNode<TreeViewItem> ffac = new TreeNode<TreeViewItem>(iitem);
                        fac2.Nodes.Add(ffac);
                    }
                    else
                    {
                        //记录库位-货架对应字典
                        if (!pIDKV.ContainsKey(tempShe.pID))
                            pIDKV.Add(tempShe.pID, new List<Shelf>());

                        pIDKV[tempShe.pID].Add(tempShe);
                    }
                }
                //生成库位下的货架结构
                foreach(var pkey in pIDKV.Keys)
                {
                    //找对应pid的库位节点
                    TreeNode<TreeViewItem> areaNode = FindListStr(fac2 , pkey);
                    if(areaNode !=null)
                    {
                        foreach(var pValue in pIDKV[pkey])
                        {
                            if(areaNode.Nodes == null)
                                areaNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();

                            //对应库位下的货架添加
                            StoredTreeViewItem iitem = new StoredTreeViewItem(pValue.name, pValue.id);
                            TreeNode<TreeViewItem> ffac = new TreeNode<TreeViewItem>(iitem);
                            areaNode.Nodes.Add(ffac);
                        }
                    }
                    else//不存在对应id库位的货架
                    {
                        Debug.LogError(pkey+"对应库位不存在！无法放置货架！");
                    }
                }


            }
            //仓库字典记录
            facListTemp.Add(rs.WAREHOUSES[i].id, factoryTreeData);
        }

        return stored;
    }


    TreeNode<TreeViewItem> FindListStr(TreeNode<TreeViewItem> listnode, string findstr)
    {
        StoredTreeViewItem tempItem = listnode.Item as StoredTreeViewItem;
        if (tempItem.ID == findstr)
        {
            return listnode;
        }


        if (listnode.Nodes != null && listnode.Nodes.Count > 0)
        {
            foreach (var tt in listnode.Nodes)
            {
                TreeNode<TreeViewItem> tempitem = FindListStr(tt, findstr);
                if (tempitem != null)
                    return tempitem;
            }
        }
        return null;
    }





    #endregion



}

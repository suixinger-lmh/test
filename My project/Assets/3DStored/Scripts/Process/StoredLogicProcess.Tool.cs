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












    #region ����

    //����ǰ����תΪ�洢����
    public RecordStored DoScene2RecordInf()
    {
        RecordStored recordStored = new RecordStored();
        recordStored.factoryRecordInfs = new List<FactoryRecordInf>();
        //��ȡ�ֿ�
        for (int i = 0; i < _FactorRoot.childCount; i++)
        {
            FactoryRecordInf facRecord = new FactoryRecordInf();
            StoredItemBase facHelper = _FactorRoot.GetChild(i).GetComponent<StoredItemBase>();

            facRecord.Name = facHelper.GetName();//.gameObject.name;//����
            facRecord.ID = facHelper.GetID();
            facRecord.Datasource = facHelper.GetData<Factory>();//����Դ
            facRecord.Position = facHelper.GetPositionStr();//λ��
            facRecord.AreaRecordInfs = SaveArea(facHelper.gameObject);//��ȡ�����¼

            recordStored.factoryRecordInfs.Add(facRecord);
        }

        return recordStored;
    }

    List<AreaRecordInf> SaveArea(GameObject factory)
    {
        List<AreaRecordInf> tempAreaRecordInfs = new List<AreaRecordInf>();
        //��ȡ����
        Transform tempAreaP = CommonHelper.GetFactoryAreaParent(factory);
        for (int i = 0; i < tempAreaP.childCount; i++)
        {
            AreaRecordInf tempInf = new AreaRecordInf();
            StoredItemBase jd = tempAreaP.GetChild(i).GetComponent<StoredItemBase>();
            LineRenderer lineRender = tempAreaP.GetChild(i).GetComponent<LineRenderer>();
           

            tempInf.Name = jd.GetName();//lineRender.gameObject.name;
            tempInf.ID = jd.GetID();
            tempInf.PointPositions = new List<string>();
            tempInf.Position = jd.GetPositionStr();//CommonHelper.Vector3toString(lineRender.transform.position);//λ��
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
        //��ȡ����
        Transform tempShelvesP = CommonHelper.GetFactoryShelvesParent(area);
        for (int i = 0; i < tempShelvesP.childCount; i++)
        {
            //������Ϣ��¼
            ShelvesRecordInf tempInf = new ShelvesRecordInf();
            //ÿ�����ܵ���Ϣ
            StoredItemBase dataHelper = tempShelvesP.GetChild(i).GetComponent<StoredItemBase>();

            tempInf.ID = dataHelper.GetID();
            tempInf.Name = dataHelper.GetName();//.gameObject.name;
            tempInf.Position = dataHelper.GetPositionStr();//CommonHelper.Vector3toString(dataHelper.transform.position);
            tempInf.Datasource = dataHelper.GetData<Shelves>();


            //�ּ���¼
            ShelvesLevel sl = tempShelvesP.GetChild(i).GetComponent<ShelvesLevel>();
            if (sl != null && sl.hasLevel)//���ڷּ�
            {
                tempInf.LevleRecord = new LevelRecord();
                tempInf.LevleRecord.Level = sl.level;
            }

            //�����¼
            tempInf.GoodsRecordInfs = SaveGoods(dataHelper.gameObject);


            tempShelvesRecordInfs.Add(tempInf);
        }



        return tempShelvesRecordInfs;
    }

    List<GoodsRecordInf> SaveGoods(GameObject shelves)
    {
        List<GoodsRecordInf> goodsRecordInfs = new List<GoodsRecordInf>();
        //�����¼
        GDGoods[] gds = shelves.GetComponentsInChildren<GDGoods>();
        if (gds == null || gds.Length == 0)
            return null;
        for (int i = 0; i < gds.Length; i++)//����ÿ�����Ϣ
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



    #region ��ȡ


    RecordStored DoReadFile()
    {
        string str = System.IO.File.ReadAllText(@"C:\Users\PC5837\Desktop\temp.json");
        RecordStored rs = JsonMapper.ToObject<RecordStored>(str);
        return rs;
    }



    //�����������ɳ���
    void DoReadCreate(RecordStored rs)
    {
        GameObject lineRenderPrefab = Resources.Load("AreaLine") as GameObject;


        for (int i = 0; i < rs.factoryRecordInfs.Count; i++)
        {
            List<AreaRecordInf> areaRecord = rs.factoryRecordInfs[i].AreaRecordInfs;
            //���� �ֿ�
            CreateFactoryFromJson(rs.factoryRecordInfs[i], (factoryIns) =>
            {
                // (�ֿ������ɺ󴴽�����)
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
                //���� �ֿ�
                GameObject insObj = Instantiate(go, _FactorRoot);
                //Ĭ�ϲֿ�
                if(data.abRes.AssetName == "Ĭ�ϲֿ�")
                {
                    Vector3 size = CommonHelper.String2Vector3(data.Scale);
                    CommonHelper.CreateDefaultFactorySize(insObj, size.x, size.z);//ָ���ֿ�ߴ�
                }

                StoredItemFactory facItem = CommonHelper.BindFactory_(insObj, data);
                facItem.InitOp(dataInf.Name, CommonHelper.String2Vector3(dataInf.Position));
                
                //insObj.transform.position = CommonHelper.String2Vector3(dataInf.Position);
                //insObj.name = dataInf.Name;

                //��ɺ�ִ��
                afterLoad(insObj);
            }
            else
            {
                Debug.LogError("����ʧ�ܣ���Դ�쳣��");
            }
        });
    }

    void CreateAreaFromJson(AreaRecordInf dataInf, GameObject facObj, GameObject linePrefab)
    {
        //����������
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

        tempLine.transform.position = CommonHelper.String2Vector3(dataInf.Position);//λ��

        //���ݰ�
        StoredItemArea areaItem = CommonHelper.BindArea_(tempLine.gameObject, dataInf.Datasource);
        areaItem.InitOp();


        CreateShelvesFromJson(dataInf.ShelvesRecordInfs, tempLine.gameObject);
    }


    void CreateShelvesFromJson(List<ShelvesRecordInf> dataInfs, GameObject areaObj)
    {
        //���ܹҵ�
        Transform tempShelvesP = CommonHelper.GetFactoryShelvesParent(areaObj);

        //���ɻ���
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
                    //������Ϣ
                    //insObj.transform.position = CommonHelper.String2Vector3(dataRecordInf.Position);
                    //insObj.name = dataRecordInf.Name;

                  
                    ////������
                    //Manager3DStored.Instance.GetStoredComponent<DragManager>().GetOrAddDragItem(insObj).InitDragFunc(CommonHelper.TempShelves, CommonHelper.TempArea);
                    //insObj.AddComponent<JDataHelper>().SetData<Shelves>(data);


                    //���ڷֲ�
                    if (dataRecordInf.LevleRecord != null && dataRecordInf.LevleRecord.Level > 1)
                    {
                        ShelvesLevel levelComponent = insObj.AddComponent<ShelvesLevel>();
                        levelComponent.SaveOrigin(insObj);
                        levelComponent.SetLevel_new(dataRecordInf.LevleRecord.Level);
                    }

                    //�������
                    CreateGoodsFromJson(dataRecordInf.GoodsRecordInfs, insObj);



                    //���ɺ��Զ�ѡ��
                    //DoSelectObj(insObj, data);
                    //DoLookCreateObj(insObj);
                }
                else
                {
                    Debug.LogError("����ʧ�ܣ���Դ�쳣��");
                }
            });

        }



    }



    void CreateGoodsFromJson(List<GoodsRecordInf> dataInfs, GameObject shelvels)
    {
        //���ڻ�����Ϣ
        if (dataInfs != null && dataInfs.Count > 0)
        {
            ShelvesLevel levelComponent = shelvels.GetComponent<ShelvesLevel>();
            if (levelComponent != null && levelComponent.hasLevel)//�ֲ�
            {
                //��¼ÿһ��Ĺҵ�����
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
            else//δ�ֲ�
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


    #region չʾ



    ObservableList<TreeNode<TreeViewItem>> facList = new ObservableList<TreeNode<TreeViewItem>>();
    ObservableList<TreeNode<TreeViewItem>> areaList = new ObservableList<TreeNode<TreeViewItem>>();
    ObservableList<TreeNode<TreeViewItem>> shelvesList = new ObservableList<TreeNode<TreeViewItem>>();
    ObservableList<TreeNode<TreeViewItem>> goodsList = new ObservableList<TreeNode<TreeViewItem>>();

    //���ݵ�ǰ���� ��ȡչʾ�б���Ϣ
    //public ObservableList<TreeNode<TreeViewItem>> CreateViewListByScene()
    //{
    //    //��ȡ����
    //    RecordStored record = DoScene2RecordInf();

    //    return GetFactoryList(record.factoryRecordInfs);

    //    ObservableList<TreeNode<TreeViewItem>> treeNodes = new ObservableList<TreeNode<TreeViewItem>>();
    //    TreeViewItem tvi = new TreeViewItem("�ִ���Ϣ", null);
    //    TreeNode<TreeViewItem> treeNode = new TreeNode<TreeViewItem>(tvi);

    //    //�ֿ���Ϣ
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

    #region TreeViewItem���壬��Ψһ��ʶ��ֻ��name������

    public ObservableList<TreeNode<TreeViewItem>> CreateViewListByScene_new()
    {
        //��ȡ����
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

            //�����¼
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

        //ֻ�ǻ���ҵ���
        for (int i = 0; i < goodsRecordInfs.Count; i++)
        {
            //û�л�����в���ʾ
            if (goodsRecordInfs[i].count == 0)
                continue;

            //�㼶���ư�����Ϣ���»��߽�ȡ
            StoredTreeViewItem tvi = new StoredTreeViewItem("GD_"+(i+1),"����ID", null);
            TreeNode<TreeViewItem> treeNode = new TreeNode<TreeViewItem>(tvi);
            nodes.Add(treeNode);

            //goodsList.Add(treeNode);
        }
        nodes.EndUpdate();
        return nodes;
    }


    #endregion


    /// <summary>
    /// �����ֿ�������ṹ�ֵ�
    /// </summary>
    Dictionary<string, ObservableList<TreeNode<TreeViewItem>>> facListTemp = new Dictionary<string, ObservableList<TreeNode<TreeViewItem>>>();


    //������̨���ݣ��������ṹ
    public ObservableList<TreeNode<TreeViewItem>> CreateStockList(StockInf rs)
    {
        //����ֵ�
        facListTemp.Clear();


        //��֯��
        ObservableList<TreeNode<TreeViewItem>> stored = new ObservableList<TreeNode<TreeViewItem>>();
        //��֯��Ϣ
        StoredTreeViewItem itemf = new StoredTreeViewItem(rs.STOCKORGNAME, rs.STOCKORGID);
        TreeNode<TreeViewItem> facf = new TreeNode<TreeViewItem>(itemf);
        stored.Add(facf);
        //��֯�µĲֿ�
        facf.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        for (int i = 0; i < rs.WAREHOUSES.Count; i++)
        {
            //�ֿ���Ϣ
            StoredTreeViewItem item = new StoredTreeViewItem(rs.WAREHOUSES[i].name, rs.WAREHOUSES[i].id);
            TreeNode<TreeViewItem> fac = new TreeNode<TreeViewItem>(item);
            facf.Nodes.Add(fac);

            //ÿ���ֿⵥ�������ṹ��¼�ڲֿ��ֵ���
            ObservableList<TreeNode<TreeViewItem>> factoryTreeData = new ObservableList<TreeNode<TreeViewItem>>();
            //�ֿ�����ṹ
            TreeNode<TreeViewItem> fac2 = new TreeNode<TreeViewItem>(item);
            //�ֿ��µĿ�λ
            fac2.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
            
            factoryTreeData.Add(fac2);

            //�����ڣ�����һ��Ĭ�Ͽ�λ��
            if (rs.WAREHOUSES[i].shelf == null || rs.WAREHOUSES[i].shelf.Count < 1)
            {
                //�����ڿ�λ���ݵģ��Զ�����һ��Ĭ�Ͽ�λ��idΪ�ֿ�id
                StoredTreeViewItem iitem = new StoredTreeViewItem("Ĭ�Ͽ�λ", rs.WAREHOUSES[i].id);
                TreeNode<TreeViewItem> ffac = new TreeNode<TreeViewItem>(iitem);
                fac2.Nodes.Add(ffac);

                //����һ��Ĭ�ϻ���
                ffac.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
                StoredTreeViewItem iiitem = new StoredTreeViewItem("Ĭ�ϻ���", rs.WAREHOUSES[i].id);
                TreeNode<TreeViewItem> fffac = new TreeNode<TreeViewItem>(iiitem);
                ffac.Nodes.Add(fffac);
            }
            else
            {
                //���� ���ϼ�Ϊ�ջ����ϼ�Ϊ�ֿ�id��Ϊ��λ���ϼ�Ϊ��λ�ģ���Ϊ�ÿ�λ���ܣ�
                //����һ�飬����λ���ɣ������ϼ���Ӧ�ֵ�����
                Dictionary<string, List<Shelf>> pIDKV = new Dictionary<string, List<Shelf>>();
                for (int m = 0; m < rs.WAREHOUSES[i].shelf.Count; m++)
                {
                    Shelf tempShe = rs.WAREHOUSES[i].shelf[m];
                    if(string.IsNullOrEmpty(tempShe.pID) || tempShe.pID == rs.WAREHOUSES[i].id)//��idΪ�ջ�IDΪ�ֿ�id
                    {
                        //��¼Ϊ��λ
                        StoredTreeViewItem iitem = new StoredTreeViewItem(tempShe.name, tempShe.id);
                        TreeNode<TreeViewItem> ffac = new TreeNode<TreeViewItem>(iitem);
                        fac2.Nodes.Add(ffac);
                    }
                    else
                    {
                        //��¼��λ-���ܶ�Ӧ�ֵ�
                        if (!pIDKV.ContainsKey(tempShe.pID))
                            pIDKV.Add(tempShe.pID, new List<Shelf>());

                        pIDKV[tempShe.pID].Add(tempShe);
                    }
                }
                //���ɿ�λ�µĻ��ܽṹ
                foreach(var pkey in pIDKV.Keys)
                {
                    //�Ҷ�Ӧpid�Ŀ�λ�ڵ�
                    TreeNode<TreeViewItem> areaNode = FindListStr(fac2 , pkey);
                    if(areaNode !=null)
                    {
                        foreach(var pValue in pIDKV[pkey])
                        {
                            if(areaNode.Nodes == null)
                                areaNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();

                            //��Ӧ��λ�µĻ������
                            StoredTreeViewItem iitem = new StoredTreeViewItem(pValue.name, pValue.id);
                            TreeNode<TreeViewItem> ffac = new TreeNode<TreeViewItem>(iitem);
                            areaNode.Nodes.Add(ffac);
                        }
                    }
                    else//�����ڶ�Ӧid��λ�Ļ���
                    {
                        Debug.LogError(pkey+"��Ӧ��λ�����ڣ��޷����û��ܣ�");
                    }
                }


            }
            //�ֿ��ֵ��¼
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

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

    GameObject _defaultFacObj;//Ĭ�ϲֿ�Ԥ����
    GameObject _linePrefab;//Ĭ�Ͽ�λԤ����
    GameObject _autoAreaItemPrefab;//�б�ѡ��Ԥ����

    GameObject _autoAreaItemEditPrefab;//

    GameObject newFac;//��ǰ���ɵĲֿ�


    Factory defaultFac;
    Shelves defaultShelves;


    //
    SaveStockInf_Fac saveData;



 
    public void QuickCreate(ObservableList<TreeNode<TreeViewItem>> facTree, SaveStockInf_Fac mapData = null,bool isOpenEdit = false)
    {
        //������������
        if (facTree[0].Nodes == null || facTree[0].Nodes.Count == 0)
        {
            Debug.LogError("�����ڿ�λ���ݣ�");
            return;
        }

        StoredTreeViewItem facTreeData = facTree[0].Item as StoredTreeViewItem;
        int areaCount = facTree[0].Nodes.Count;
        //���ݼ�¼�������������

        //���ɲֿ�
        CreateFactory(50, 50 / areaCount, facTreeData.Name, facTreeData.ID);
        //��ָ���������ɿ�λ(ֻ��Ĭ����Ϣ)
        CreateAreaList(areaCount, autoAreaListParent1);
        //ָ����Ϣ��
        SetData2Inf(facTree, mapData);



        //���ɻ���
        StartCoroutine(CreateAllShe());





        if (isOpenEdit)//�򿪱༭
        {
            //�༭ģʽ�£����ɼ�¼��
            //��¼
            //��¼�ֿ�����ID
            saveData = new SaveStockInf_Fac();
            saveData.ID = facTreeData.ID;
            //saveData.modelData = defaultFac;
            saveData.areaList = new List<SaveStockInf_Area>();

            //��¼��λ��Ϣ��
            for (int i = 0; i < tempAutoInf.Count; i++) {
                SaveStockInf_Area saveAreadata = new SaveStockInf_Area();
                saveAreadata.ID = tempAutoInf[i].id;
                saveAreadata.modelData = tempAutoInf[i].shelvesData;
                saveAreadata.level = tempAutoInf[i].level;
                saveAreadata.x = tempAutoInf[i].x.ToString();
                saveAreadata.y = tempAutoInf[i].y.ToString();
                saveAreadata.sheList = new List<SaveStockInf_She>();
                //��¼��λ�»�����Ϣ��
                for (int m = 0; m < tempAutoInf[i].idlist.Count; m++) {
                    SaveStockInf_She savesheldata = new SaveStockInf_She();
                    savesheldata.ID = tempAutoInf[i].idlist[m];//�ÿ�λID��ΪĬ�ϻ���ID
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
        //����������Ϣ������
        for (int i = 0; i < facTree[0].Nodes.Count; i++)
        {
            TempAutoAreaInf inf = tempAutoInf[i];
            TreeNode<TreeViewItem> areaNode = facTree[0].Nodes[i];
            StoredTreeViewItem areaTree = areaNode.Item as StoredTreeViewItem;
          
            //��¼��������
            inf.shelist = new List<string>();
            inf.idlist = new List<string>();
            int sheCount;

            //�����������
            if (areaNode.Nodes == null || areaNode.Nodes.Count == 0)//����λ�²����ڻ��ܣ�����һ����
            {
                sheCount = 1;
                inf.shelist.Add(areaTree.Name + "_Ĭ�ϻ���");
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

            //չʾ��Ϣ
            string name = areaTree.Name;
            //�༭��ť
            inf.itemObj.transform.GetChild(0).GetComponent<Text>().text = name;
            inf.itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                nowSelectInf = inf;
                //�򿪱༭���
                OpenAreaEditPanel(name, nowSelectInf.count, nowSelectInf.level, nowSelectInf.x, nowSelectInf.y);
            });

            //������Ϣ
            areaList[i].GetComponent<StoredItemArea>().SetName(name);
            areaList[i].GetComponent<StoredItemArea>().SetID(areaTree.ID);

            //��¼����Ϣ���
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
           


            //��¼��λ��Ϣ��
            //SaveStockInf_Area saveAreadata = new SaveStockInf_Area();
            //saveAreadata.ID = treeDataArea.ID;
            ////��¼��λ�»�����Ϣ��
            //saveAreadata.sheList = new List<SaveStockInf_She>();
            //if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
            //{
            //    SaveStockInf_She savesheldata = new SaveStockInf_She();
            //    savesheldata.ID = treeDataArea.ID;//�ÿ�λID��ΪĬ�ϻ���ID
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

        //������������
        if (tree[0].Nodes == null || tree[0].Nodes.Count==0)
        {
            Debug.LogError("�����ڿ�λ���ݣ�");
            return;
        }

        StoredTreeViewItem facTreeData = tree[0].Item as StoredTreeViewItem;
        string id = facTreeData.ID;
        int areaCount = tree[0].Nodes.Count;//��λ����
        countText.text = areaCount.ToString();


        //��¼
        //��¼�ֿ�����ID
        saveData = new SaveStockInf_Fac();
        saveData.ID = id;
        //saveData.modelData = defaultFac;
        saveData.areaList = new List<SaveStockInf_Area>();


       // if (IsRecoredDataExist(id))//���ڼ�¼��
        {
            //��ȡ��¼������
            string path = PathExtensions.StreamingAssetsPath() + "/RecoredDir/" + id + ".json";
            StartCoroutine(CommonHelper.GetText(path, (textStr) =>
            {
                SaveStockInf_Fac rs = JsonMapper.ToObject<SaveStockInf_Fac>(textStr);

                //�ֿ��¼��Ϣ��ԭ
                //defaultFac = rs.modelData;
                //���ɲֿ�
                CreateFactory(50, 50 / areaCount, tree[0].Item.Name,id);

                //���ɿ�λ
                CreateAreaList(areaCount, autoAreaListParent1);

                //����������Ϣ������
                for (int i = 0; i < areaCount; i++)
                {
                    TempAutoAreaInf inf = tempAutoInf[i];

                    //�Ҽ�¼�� ֱ�Ӹ�ֵ
                    StoredTreeViewItem treeDataArea = tree[0].Nodes[i].Item as StoredTreeViewItem;
                    SaveStockInf_Area finddata = rs.areaList.Find(p => p.ID == treeDataArea.ID);
                    if (finddata != null)
                    {
                        inf.shelvesData = finddata.modelData;
                        inf.x = float.Parse(finddata.x);
                        inf.y = float.Parse(finddata.y);
                        inf.level = finddata.level;
                    }

                    //��¼��������
                    inf.shelist = new List<string>();
                    inf.idlist = new List<string>();

                    int sheCount;
                    //�����������
                    if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
                    {
                        sheCount = 1;
                        inf.shelist.Add(tree[0].Nodes[i].Item.Name+"_Ĭ�ϻ���");
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
                        
                    //Ĭ�Ͽ�λֻ��һ������
                    if (sheCount == 0)
                        inf.count = 1;
                    else
                        inf.count = sheCount;


                    //��¼��λ��Ϣ��
                    SaveStockInf_Area saveAreadata = new SaveStockInf_Area();
                    saveAreadata.ID = treeDataArea.ID;
                    //��¼��λ�»�����Ϣ��
                    saveAreadata.sheList = new List<SaveStockInf_She>();
                    if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
                    {
                        SaveStockInf_She savesheldata = new SaveStockInf_She();
                        savesheldata.ID = treeDataArea.ID;//�ÿ�λID��ΪĬ�ϻ���ID
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



                    //չʾ��Ϣ
                    string name = tree[0].Nodes[i].Item.Name;
                    //�༭��ť
                    inf.itemObj.transform.GetChild(0).GetComponent<Text>().text = name;
                    inf.itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        nowSelectInf = inf;

                        //�򿪱༭���
                        OpenAreaEditPanel(name, nowSelectInf.count, nowSelectInf.level, nowSelectInf.x, nowSelectInf.y);
                    });

                    //������Ϣ
                    areaList[i].GetComponent<StoredItemArea>().SetName(name);
                    areaList[i].GetComponent<StoredItemArea>().SetID(treeDataArea.ID);
                }

                if (isOpenEdit)
                {
                    p0.SetActive(false);
                    p1.SetActive(true);
                    OpenUIPanel();
                }

                //��������ˢ�´�С
                StartCoroutine(CreateAllShe());
            }));


        }
        //else//�����ڼ�¼�� ֱ�Ӱ�����������
        {
            //���ɲֿ�
            CreateFactory(50, 50 / areaCount, tree[0].Item.Name,id);

            //���������б�Ϳ�λ
            CreateAreaList(areaCount, autoAreaListParent1);

            //����������Ϣ������
            for (int i = 0; i < areaCount; i++)
            {
                TempAutoAreaInf inf = tempAutoInf[i];
                StoredTreeViewItem treeDataArea = tree[0].Nodes[i].Item as StoredTreeViewItem;

                //��¼��������
                inf.shelist = new List<string>();
                inf.idlist = new List<string>();
                int sheCount;

                //�����������
                if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
                {
                    sheCount = 1;
                    inf.shelist.Add(tree[0].Nodes[i].Item.Name + "_Ĭ�ϻ���");

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
                //Ĭ�Ͽ�λֻ��һ������
                if (sheCount == 0)
                    inf.count = 1;
                else
                    inf.count = sheCount;


                //��¼��λ��Ϣ��
                SaveStockInf_Area saveAreadata = new SaveStockInf_Area();
            
                saveAreadata.ID = treeDataArea.ID;
                //��¼��λ�»�����Ϣ��
                saveAreadata.sheList = new List<SaveStockInf_She>();
                if (tree[0].Nodes[i].Nodes == null || tree[0].Nodes[i].Nodes.Count == 0)
                {
                    SaveStockInf_She savesheldata = new SaveStockInf_She();
                    savesheldata.ID = treeDataArea.ID;//�ÿ�λID��ΪĬ�ϻ���ID
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



                //չʾ��Ϣ
                string name = tree[0].Nodes[i].Item.Name;
                //�༭��ť
                inf.itemObj.transform.GetChild(0).GetComponent<Text>().text = name;
                inf.itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    nowSelectInf = inf;

                    //�򿪱༭���
                    OpenAreaEditPanel(name, nowSelectInf.count, nowSelectInf.level, nowSelectInf.x, nowSelectInf.y);

                    //nowItemName = goodsSelectBtn.GetComponentInChildren<Text>();
                    //Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelOpen, "��������-����"));

                });

                //������Ϣ
                areaList[i].GetComponent<StoredItemArea>().SetName(name);
                areaList[i].GetComponent<StoredItemArea>().SetID(treeDataArea.ID);
            }
            if (isOpenEdit)
            {
                p0.SetActive(false);
                p1.SetActive(true);
                OpenUIPanel();
            }

            //��������ˢ�´�С
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
            //���
            ClearScene();

            p1.SetActive(false);
            p0.SetActive(true);
            CloseUIPanel();
        }
    }

    void CreateAreaList(int areaCount,Transform parentList)
    {
        //UIѡ��������ݣ���λ����

        //����б�ѡ��
        for (int i = 0; i < parentList.childCount;)
        {
            DestroyImmediate(parentList.GetChild(i).gameObject);
        }
        //�����������
        tempAutoInf.Clear();
        //�������
        for (int i = 0; i < areaList.Count; i++)
        {
            DestroyImmediate(areaList[i]);
        }
        areaList.Clear();



        Transform areaP = CommonHelper.GetFactoryAreaParent(newFac);
        //�ֿ�ߴ�
        float height = newFac.GetComponent<StoredItemFactory>().GetRelativeHeightValue();
        Bounds facBounds = newFac.GetComponent<StoredItemFactory>().GetGroundBounds();
        Vector3 size = facBounds.size;
        //��һ����λ��
        //�˴��൱�ڽӴ�����ã����ϸ����߶ȣ�����������һ�£�
        Vector3 posOrigin = facBounds.center + new Vector3(-size.x / 2, height + CommonHelper.AreaLineFloatHeight, size.z / 2);//�ֿ����Ͻ�����

        //�б�����
        for (int i = 0; i < areaCount; i++)
        {
            TempAutoAreaInf inf = new TempAutoAreaInf();

            GameObject tempItem = Instantiate(_autoAreaItemEditPrefab, parentList);

            inf.itemObj = tempItem;

            inf.shelvesData = defaultShelves;
           


            tempAutoInf.Add(inf);


            //�����߿�
            LineRenderer tempLine = Instantiate(_linePrefab, areaP).GetComponent<LineRenderer>();
            //Ĭ��
            tempLine.positionCount = 5;
            tempLine.SetPosition(0, posOrigin);
            tempLine.SetPosition(1, posOrigin + new Vector3(size.x / areaCount, 0, 0));
            tempLine.SetPosition(2, posOrigin + new Vector3(size.x / areaCount, 0, -size.z));
            tempLine.SetPosition(3, posOrigin + new Vector3(0, 0, -size.z));
            tempLine.SetPosition(4, posOrigin);
            posOrigin = posOrigin + new Vector3(size.x / areaCount, 0, 0);

            Area Areadata = new Area();
            Areadata.Usage = "����";
            Areadata.AreaShape = "����";
            StoredItemArea areaItem = CommonHelper.BindArea_(tempLine.gameObject, Areadata);
            areaItem.InitOp();
            tempLine.name += i;
            areaList.Add(tempLine.gameObject);
        }
    }




    void GetSaveData()
    {
        //��¼��λ�༭��Ϣ��
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
        defaultFac.abRes.LabelName = "Ĭ�ϲֿ�";
        defaultFac.abRes.ABPath = "Ĭ��";
        defaultFac.abRes.AssetName = "Ĭ�ϲֿ�";


        defaultShelves = new Shelves();
        defaultShelves.abRes.LabelName = "Ĭ�ϻ���";
        defaultShelves.abRes.ABPath = "she";
        defaultShelves.abRes.AssetName = "shelves_";
        defaultShelves.Usage = "����";



        if (_defaultFacObj == null)
        {
            //������ɺ�ж��ab��������ʵ��
            Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Factory/", defaultFac.abRes.ABPath, defaultFac.abRes.AssetName, (go) =>
            {
                if (CommonHelper.IsSuitFactory(go, defaultFac))
                {
                    _defaultFacObj = go;
                }
                else
                {
                    Debug.LogError("�ⲿĬ�ϲֿ������쳣��");
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

        //���泡��
        saveBtn.onClick.AddListener(() => {
            GetSaveData();
        });



        createFac_Btn.onClick.AddListener(()=> {
            CreateFactory(15,15,"Ĭ�ϲֿ�","");
        });

        //close_Btn.onClick.AddListener(() => {
        //    //ɾ��

        //    //�ر����

        //    CloseUIPanel();

        //});

        //��������
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

        //��������
        areaCount_Input.onEndEdit.AddListener((str) => {
            if(int.TryParse(str,out areaCount))
            {

                CreateAreaList(areaCount,autoAreaListParent);

                //����������Ϣ������
                for (int i = 0; i < areaCount; i++)
                {
                    TempAutoAreaInf inf = tempAutoInf[i];

                    //չʾ��Ϣ
                    inf.itemObj.transform.GetChild(0).GetComponent<Text>().text = "Ĭ�Ͽ�λ"+i;
                    inf.itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        nowSelectInf = inf;
                        //nowItemName = goodsSelectBtn.GetComponentInChildren<Text>();
                        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelOpen, "��������-����"));

                    });
                }


                //ClearArea();

                ////�б�����
                //for(int i = 0; i < areaCount; i++)
                //{
                //    TempAutoAreaInf inf = new TempAutoAreaInf();
                //    tempAutoInf.Add(inf);
                //    GameObject tempItem = Instantiate(_autoAreaItemPrefab, autoAreaListParent);

                //    //���ɲ���
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

                //    //ѡ������¼�
                //    Button goodsSelectBtn = tempItem.GetComponentInChildren<Button>();
                //    goodsSelectBtn.onClick.AddListener(() => {

                //        nowSelectInf = inf;
                //        nowItemName = goodsSelectBtn.GetComponentInChildren<Text>();
                //        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelOpen, "��������-����"));
                //        //CreateViewPanel.CreateViewEvent cve = new CreateViewPanel.CreateViewEvent();
                //        //cve.title = "����";
                //        //cve.ac = (objs) => {
                //        //    ////for
                //        //};


                //        //�򿪻���ѡ�����

                //        //ѡ�к����Ϣ��¼��inf��
                //        //inf.shelvesData
                //    });


                //}

                ////ƽ����������
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
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelOpen, "��������-����"));
            else
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelClose, "��������-����"));
           
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
        //���
        panelTempObj.name = "QuickShelvesListPanel";
        GameObject panelTemp = Instantiate(panelTempObj, transform.parent);
        panelTemp.name = "QuickShelvesListPanel";
        CreateViewPanel cvPtemp = panelTemp.GetComponent<CreateViewPanel>();
        List<Shelves> tagData = Manager3DStored.Instance._shelves.FindAll(p => p.Usage == "����");
        cvPtemp.CreateViewByDate("��������-����", Resources.Load("UIItemPrefab/facItem") as GameObject, tagData.Count, (objs) =>
        {
            for (int i = 0; i < objs.Count; i++)
            {
                //չʾ����
                objs[i].GetComponentInChildren<Text>().text = tagData[i].abRes.LabelName;
                objs[i].name = tagData[i].abRes.LabelName;
                //����¼�
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

                        //Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelClose, "��������-����"));
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
            CommonHelper.CreateDefaultFactorySize(newFac,sizeX,sizeY);//ָ���ֿ�ߴ�
            defaultFac.Scale = CommonHelper.Vector3toString(new Vector3(sizeX,1,sizeY));
            StoredItemFactory facItem = CommonHelper.BindFactory_(newFac, defaultFac);
            facItem.InitOp();
            facItem.SetName(name);
            facItem.SetID(id);
        }
        else
        {
            Debug.LogError("�Ѿ����ڣ�");
        }
    }

    void CreateAreaObject(int count)
    {
       
    }

    IEnumerator CreateAllShe()
    {

        int waitNumber = 0;
        int maxNumber = areaList.Count;
        //����ÿ�������Ӧ�Ļ���
        for (int i = 0; i < areaList.Count; i++)
        {
            //ɾ�����л���
            DestroyImmediate(CommonHelper.GetFactoryShelvesParent(areaList[i]).gameObject);



            TempAutoAreaInf inf = tempAutoInf[i];

            int index = i;
            //���ض�Ӧab��
            //������ɺ�ж��ab��������ʵ��
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
                        Debug.LogError("�����쳣" + index);
                    }
                    finally
                    {
                        waitNumber++;
                    }
                }
                else
                {
                    Debug.LogError("���ɻ���ʧ�ܣ�");
                    waitNumber++;
                }

              
            });
        }

        while (waitNumber < maxNumber)
        {
            yield return null;
        }


        //ˢ�²ֿ��С
        RefreshFac();
        //��λ��������
        ReSetAreaPos();


        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent( CoreEventId.CameraLookAt, newFac));

    }








    void RefreshFac()
    {
        //��� x=��������x  y=�������y
        float newX = 0, newY = 0;
        for (int i = 0; i < areaList.Count; i++)
        {
            Bounds bound = CommonHelper.GetObjBounds(areaList[i]);
            newX += bound.size.x;
            if (newY < bound.size.z)
                newY = bound.size.z;
        }

        //���ô�С
        CommonHelper.CreateDefaultFactorySize(newFac,newX,newY);
        //���ݸı�
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

        //�ֿ����
        Bounds facBounds = newFac.GetComponent<BoxCollider>().bounds;

        Vector3 originPos = facBounds.center + new Vector3(-(facBounds.size.x) / 2, 0, (facBounds.size.z) / 2);//����߽����Ͻ�
        Debug.Log(originPos);
        float x = 0, y = 0;
        for (int i = 0; i < areaList.Count; i++)
        {
            //��ȡ����ߴ�
            Bounds bound = areaList[i].GetComponent<StoredItemArea>().GetBoxBounds();

            //Bounds bound = CommonHelper.GetObjBounds(areaList[i]);
            if (i > 0)
                x += areaList[i - 1].GetComponent<StoredItemArea>().GetBoxBounds().size.x;


            //�������� = ����λ��+box.center;
            //�����ȥbounds.center��Ϊ�˽�box�ƶ���ԭ��λ��(����ʼ��ƫ����)
            Vector3 newPos = originPos + new Vector3(x + bound.size.x / 2, 0, -bound.size.z / 2) - bound.center;
            newPos.y = areaList[i].transform.position.y;
            areaList[i].transform.position = newPos;

            //TODO:���ƶ���ֻ�ĵ�λ

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

    //177 499 1993//����

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

        //����ת��//������������ ����
        //if (count > 6 && getlineC.Count == 2)
        //{
        //    GetSuitRowColumn(count+1,out row,out column);
        //}
        //else
        {
            if (getlineC.Count % 2 == 0)//ż����
            {
                int index = Mathf.FloorToInt(getlineC.Count / 2.0f);
                row = getlineC[index - 1];
                column = getlineC[index];
            }
            else//������
            {
                int index = Mathf.FloorToInt(getlineC.Count / 2.0f);
                row = column = getlineC[index];
            }
        }

        //�������в����� 
        if (Mathf.Abs(row - column) > 5)
        {
            GetSuitRowColumn(count + 1, out row, out column);
        }
        //TODO������ʾЧ���Ļ��������㳤��

    }



    void CreateShelves(TempAutoAreaInf inf, LineRenderer area, GameObject she, float x, float y)
    {
        int count = inf.count;
        if (inf.shelist != null)
            count = inf.shelist.Count;

        Shelves data = inf.shelvesData;
        int level = inf.level;
            
        //����ʵ�����
        int lineCount;
        int maxLineCount;

        GetSuitRowColumn(count, out lineCount, out maxLineCount);

        //maxLineCount = Mathf.CeilToInt(count*1.0f / lineCount);//ÿ�и���

        //Debug.Log(maxLineCount);
        int splineCount = maxLineCount + 1;//�������


        //��������ߴ�
        Bounds shelvesBounds = CommonHelper.GetObjBounds(she);
        float xLength = shelvesBounds.size.x * maxLineCount + x * splineCount;
        float yLength = shelvesBounds.size.z * lineCount + y * (lineCount + 1);

        //ָ����λ����������������
        Vector3 lineViewCenter = Vector3.up * area.GetComponent<StoredItemArea>().GetViewHeight();
        //�������ɳߴ�
        CommonHelper.CreateAreaBoundsByLength(area, xLength, yLength, lineViewCenter);
        //ˢ��boxcollider
        area.GetComponent<StoredItemArea>().RefreshBoxCollider();

        Bounds areaBounds = area.GetComponent<StoredItemArea>().GetBoxBounds();
        //if(areaBounds.size.x<xLength || areaBounds.size.z < yLength)//������������


        Vector3 originPos = areaBounds.center + new Vector3(-(areaBounds.size.x ) / 2, 0, (areaBounds.size.z ) / 2);//����߽����Ͻ�
        Debug.Log(originPos);
        Vector3 objoriginPos = originPos + new Vector3(x + shelvesBounds.size.x / 2, 0, -y - shelvesBounds.size.z / 2);//����λ�����Ͻ�
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

                //���ڷֲ�
                if (level > 1)
                {
                    ShelvesLevel levelComponent = objtemp.AddComponent<ShelvesLevel>();
                    levelComponent.SaveOrigin(objtemp);
                    levelComponent.SetLevel_new(level);

                    //���ﲿ��--
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
                    //����
                    //objtemp.transform.Find("GD").AddComponent<GDGoods>().InitSelf();
                }


                //objtemp.transform.position = CommonHelper.GetObjPositionByGroundHeight(objtemp.transform.position, objtemp, areaBounds.center.y);




                allCount++;
            }
        }

    }























    //���
    void ClearScene()
    {
        //�ֿ�ɾ��
        Destroy(newFac);
        newFac = null;

        //

    }










    void ClearArea()
    {
        //����б�
        for(int i =0;i< autoAreaListParent.childCount;)
        {
            DestroyImmediate(autoAreaListParent.GetChild(i).gameObject);
        }
        tempAutoInf.Clear();
        
        //ɾ������
        for (int i = 0; i < areaList.Count;i++)
        {
            DestroyImmediate(areaList[i]);
        }
        areaList.Clear();
    }


}

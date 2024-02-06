using FrameWork;
using Stored3D;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//��ʼ״̬��findģʽ
//����selectObj��pickUPģʽ(��һ��)

//��Ҫ��ק

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

    //��ǰ��������
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
                //ȡ������
                _dragManager.UnsetFindLightObj(nowSetObj);
                nowSetObj.transform.SetParent(objParent);

               
                ChangePanelState(0);
            }
        });



        exBtn.onClick.AddListener(() =>
        {
            if (isExPanelOpen)
            {
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ShelvesEdit_Close, null));//�رձ༭���
            }
            else
            {
                //�ռ�ͨ�ò��������򿪶๦�����
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
            //��ɾ������
            if (nowSetObj != null)
            {
                if(isInCache)
                    Destroy(nowSetObj);
                else
                    _dragManager.UnsetFindLightObj(nowSetObj);
            }
            //�˳�
            ClosePanel();
        });
    }

    bool startSelect = false;
    private void Update()
    {
        if (startSelect)
        {
            //�ȴ�ѡ��
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
        AreaRemoveComponent();//�Ƴ������box

        //��ʼ��ʽ
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

        //���box����ק
        //area.GetComponent<StoredItemArea>().
        AreaComponentChange();

        //��ʼ��ʽ
        ChangePanelState(0);

        //����ģʽ
        //_dragManager.ChangeDrayType(DragManager.DragType.Find_Light, CommonHelper.TempShelves, nowArea.name);
        //_dragManager.SetDragManager(true, CommonHelper.TempShelves, CommonHelper.TempArea, DragManager.DragType.FindOnce_Light);
        //startSelect = true;

        OpenUIPanel();//��
    }

    #region ��ؼ���

    
    void AreaComponentChange()
    {
        StoredItemArea areaItem = nowArea.GetComponent<StoredItemArea>();
        //��λbox�ı�
        areaItem.ChangeAreaBoxCollider(true);
        //��Ϊ��ק����
        areaItem.SetAreaFloor(true);
        //�����ק
        //_dragManager.GetOrAddDragItem(nowArea).InitDragFunc(CommonHelper.TempArea, CommonHelper.TempArea, true);
    }
    void AreaRemoveComponent()
    {
        StoredItemArea areaItem = nowArea.GetComponent<StoredItemArea>();
        //��λbox�ı�
        areaItem.ChangeAreaBoxCollider(false);
        //��Ϊ��ק����
        areaItem.SetAreaFloor(false);
    }


    #endregion


    UnityAction quitCallBack;
    //��ʼ�����ɻ������
    public void InitShelvesSetPanel(List<Shelves> shelves,Transform shelvesParent,DragManager dg,UnityAction ac = null)
    {
        _dragManager = dg;
        createParent = shelvesParent;
        quitCallBack = ac;

        //����
        typeMap.Clear();
        for(int i = 0; i < shelves.Count; i++)
        {
            if (!typeMap.ContainsKey(shelves[i].Usage))
                typeMap.Add(shelves[i].Usage, new List<Shelves>());
            typeMap[shelves[i].Usage].Add(shelves[i]);   
        }

        //������𴴽�����ǩ�Ͷ�Ӧ�������
        GameObject typetogglePrefab = Resources.Load("UIItemPrefab/ShelvesToggle") as GameObject;
        GameObject panelTempObj = Resources.Load("Panel/SelectView") as GameObject;
        foreach(var tag in typeMap.Keys)
        {
            //��ǩ
            GameObject typeTG = Instantiate(typetogglePrefab, shelvesTypeParent);
            typeTG.GetComponentInChildren<Text>().text = tag;
            typeTG.name = tag;
            Toggle tg = typeTG.GetComponent<Toggle>();
            tg.group = shelvesTypeParent.GetComponent<ToggleGroup>();

            //���
            panelTempObj.name = tag + "SelectPanel";//�޸�Ԥ������������
            GameObject panelTemp = Instantiate(panelTempObj, transform.parent);
            panelTemp.name = tag + "SelectPanel";//ȥ��clone
            CreateViewPanel cvPtemp = panelTemp.GetComponent<CreateViewPanel>();
            List<Shelves> tagData = typeMap[tag];
            cvPtemp.CreateViewByDate(tag, Resources.Load("UIItemPrefab/facItem") as GameObject, tagData.Count, (objs) => {
                for (int i = 0; i < objs.Count; i++)
                {
                    //չʾ����
                    objs[i].GetComponentInChildren<Text>().text = tagData[i].abRes.LabelName;
                    objs[i].name = tagData[i].abRes.LabelName;
                    objs[i].GetComponentInChildren<RawImage>().texture = tagData[i].GetThumbnail();
                    //����¼�
                    string abName = tagData[i].abRes.ABPath;
                    string assetName = tagData[i].abRes.AssetName;
                    Shelves data = tagData[i];
                    objs[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    objs[i].GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (nowSetObj != null)
                            return;

                        //���ض�Ӧab��
                        //������ɺ�ж��ab��������ʵ��
                        Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Shelves/", abName, assetName, (go) =>
                        {
                            if(CommonHelper.IsSuitShelves(go, data))
                            {
                                GameObject insObj = Instantiate(go, createParent);

                                //��ʼλ�ã���������ģ��ذ�ĸ߶�
                                Vector3 center = nowArea.GetComponent<BoxCollider>().bounds.center;
                                float height = nowFactory.GetComponent<BoxCollider>().bounds.center.y;

                                StoredItemShelves sheItem = CommonHelper.BindShelves_(insObj, data);
                                sheItem.InitOp(center, height);

                                //λ�ã���������xz����ϵ��xzΪԭ����
                                //insObj.transform.position = GetAreaOriginPosition(nowArea.GetComponent<BoxCollider>(), nowFactory);
                                //insObj.name = data.LabelName + "_" + DateTime.Now.ToShortTimeString();


                                //_dragManager.GetOrAddDragItem(insObj).InitDragFunc(CommonHelper.TempShelves,CommonHelper.TempArea);
                                //if (insObj.GetComponent<JDataHelper>() == null)
                                //    insObj.AddComponent<JDataHelper>().SetData<Shelves>(data);
                                //else
                                //    insObj.GetComponent<JDataHelper>().SetData<Shelves>(data);

                                //���ɺ��Զ�ѡ��
                                DoSelectObj(insObj, data, true);
                                //DoLookCreateObj(insObj);
                            }
                        });
                    });
                }
            });

            //���¼�
            tg.onValueChanged.AddListener(cvPtemp.TogglePanel);
        }


        Resources.UnloadUnusedAssets();
    }

    bool isInCache = false;
    void DoSelectObj(GameObject obj,Shelves data,bool incache)
    {
        isInCache = incache;
        //ѡ������
        nowSetObj = obj;
        //����
        _dragManager.SetDragManager(false);
        _dragManager.SetFindLightObj(nowSetObj);
        //_dragManager.ChangeDrayType(DragManager.DragType.PickUpOnlyTarget, CommonHelper.TempShelves, nowArea.name);
        //�������
        GetObjInf(data);
        ChangePanelState(1);

    }

    bool isExPanelOpen = false;
    void ChangePanelState(int state)
    {
        if(state == 0)//��ʼ���,��ʾ����
        {
            nowSetObj = null;
            _dragManager.SetDragManager(true, CommonHelper.TempShelves, CommonHelper.TempArea, DragManager.DragType.FindOnce_Light);
            startSelect = true;
            if(isExPanelOpen)
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ShelvesEdit_Close, null));//�رձ༭���
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

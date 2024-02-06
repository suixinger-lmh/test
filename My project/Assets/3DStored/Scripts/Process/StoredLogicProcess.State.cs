using FrameWork;
using LitJson;
using Stored3D;
using Sxer.Camera;
using UnityEngine;

public partial class StoredLogicProcess
{
    public enum Stored3DState
    {
        None,//��״̬=��������ƶ�
        AddFactory,//��Ӳֿ�״̬=��������ƶ�+��ȡ���ù���
        AddArea_select,//�������״̬=ѡ��ֿ�
        AddArea_edit,//�������״̬=��������
        AddShelves_select,//��ӻ���=ѡ��ֿ�
        AddShelves_select_area,//��ӻ���=ѡ������
        AddShelves_add,//���

        SelectFactory,//ѡ��ֿ�

        View,//�۲�ģʽ
        Edit,//�༭
        Create,//xx
        GetData,//ƽ̨����    �����������λ(����ģ�ͣ����ܲ��������)��
    }

    [Header("ģʽ")]
    public Stored3DState _mainState = Stored3DState.None;


    //�л�״̬����������䶯
    void ChangeStoredState(Stored3DState nextState)
    {
        //��ǰ״̬����
        switch (_mainState)
        {
            case Stored3DState.None:
                break;
            case Stored3DState.AddFactory:
                //���ù���-ѡȡ
                tempFunc_DragManager.SetDragManager(false);

                //�ر������б����
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("�ر�", null)));


                //Manager3DStored.Instance.GetStoredComponent<DragManager>().ChangeDrayType(DragManager.DragType.None);
                break;

            case Stored3DState.AddArea_select:
                if (nextState == Stored3DState.AddArea_edit)
                {
                   

                }
                else
                {
                    //����
                    IsClickLock(false);
                    //Manager3DStored.Instance.GetStoredComponent<DragManager>().ChangeDrayType(DragManager.DragType.None);
                }
                break;
            case Stored3DState.AddArea_edit:
                nowFindFactory = null;
                //�ر������б����
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("�ر�", null)));


                //����factory
                for (int i = 0; i < _FactorRoot.childCount; i++)
                    _FactorRoot.GetChild(i).gameObject.SetActive(true);

                //����
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
                    //����
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
                    //����factory
                    for (int i = 0; i < _FactorRoot.childCount; i++)
                        _FactorRoot.GetChild(i).gameObject.SetActive(true);
                    //����
                    IsClickLock(false);
                }
               
                break;

            case Stored3DState.AddShelves_add:
                //�����
                Transform tempareaP = CommonHelper.GetFactoryAreaParent(nowFindFactory);
                for (int i = 0; i < tempareaP.childCount; i++)
                        tempareaP.GetChild(i).gameObject.SetActive(true);

                nowFindFactory = null;
                //����factory
                for (int i = 0; i < _FactorRoot.childCount; i++)
                    _FactorRoot.GetChild(i).gameObject.SetActive(true);
                //����
                IsClickLock(false);

                break;

            case Stored3DState.View:

                IsClickLock(false);
                tempUIPanel_ShowInfPanel.CloseUIPanel();
                MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
                //��ǩɾ��
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ToolLabel_Delete,null));
                break;

            case Stored3DState.Edit:

                IsClickLock(false);
                tempUIPanel_ShowInfPanel.CloseUIPanel();
                MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
                //��ǩɾ��
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ToolLabel_Delete, null));
                break;

            case Stored3DState.Create:
                IsClickLock(false);
                tempUIPanel_AutoCreatePanel.CloseUIPanel();

                MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
                //��ǩɾ��
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ToolLabel_Delete, null));
                break;
            case Stored3DState.GetData:
                IsClickLock(false);
                tempUIPanel_ShowInfPanel.CloseUIPanel();
                MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
                //�ر����
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.OpenStockInf, null));
                break;
        }

        //��״̬��ʼ
        switch (nextState)
        {
            case Stored3DState.None:
                break;

            case Stored3DState.AddFactory://��Ӳֿ�״̬
                //���

                //�����б�ѡ�����
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("�ֿ�ɾ��", _FactorRoot)));
                

                //���ù���-ѡȡ
                tempFunc_DragManager.SetDragManager(true, CommonHelper.Factory, CommonHelper.Floor, DragManager.DragType.PickPut);
                //Manager3DStored.Instance.GetStoredComponent<DragManager>().ChangeDrayType(DragManager.DragType.PickUp,CommonHelper.Factory, "Floor");
                break;
            case Stored3DState.AddArea_select:  //��������༭״̬��ѡ��ֿ�

                //��ѡ�����
                tempUIPanel_SelectGetPanel.OpenSelectPanelType(new OpPanelparam("�ֿ�", null), () => {
                    //ȡ��
                    area_Add.isOn = false;
                }, 
                (selectObj) => {
                    //ѡ��
                    nowFindFactory = selectObj;
                    ChangeStoredState(Stored3DState.AddArea_edit);
                });

                IsClickLock(true);
              
                break;
            case Stored3DState.AddArea_edit:

                //nowFindFactory = Manager3DStored.Instance.GetStoredComponent<DragManager>().findObj;
                //�������򿪶���
                tempUIPanel_AreaSetPanel.ShowAreaSetStyle(nowFindFactory);


                //�����б�ѡ�����
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("����༭", nowFindFactory)));
                

                //��������factory
                for (int i = 0; i < _FactorRoot.childCount; i++)
                    if (_FactorRoot.GetChild(i).gameObject != nowFindFactory)
                        _FactorRoot.GetChild(i).gameObject.SetActive(false);
                //��һ״̬

                //MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraState.LookTarget);

                //CameraOpLookTarget cameralookat = (CameraOpLookTarget)MainCamera.GetComponent<CameraOperateSet>().ChangeCameraOperate(CameraState.LookTarget);
                //cameralookat.target = nowFindFactory.transform;

                break;

            case Stored3DState.AddShelves_select:
               
                //��ѡ�����
                tempUIPanel_SelectGetPanel.OpenSelectPanelType(new OpPanelparam("�ֿ�", null), () =>
                {
                    //ȡ��
                    shelves_Add.isOn = false;
                },
                (selectObj) =>
                {
                    //ѡ��
                    nowFindFactory = selectObj;
                    ChangeStoredState(Stored3DState.AddShelves_select_area);
                });

                IsClickLock(true);
                break;

            case Stored3DState.AddShelves_select_area://ѡ������

                //��������factory
                for (int i = 0; i < _FactorRoot.childCount; i++)
                    if (_FactorRoot.GetChild(i).gameObject != nowFindFactory)
                        _FactorRoot.GetChild(i).gameObject.SetActive(false);

                //��ѡ�����
                tempUIPanel_SelectGetPanel.OpenSelectPanelType(new OpPanelparam("����", nowFindFactory), () =>
                {
                    //ȡ��
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

                //�򿪻����������
                tempUIPanel_ShelvesSetPanel.OpenPanel(nowFindFactory,nowArea);
                break;

            case Stored3DState.View:
                if(_mainState == Stored3DState.None)
                {
                    IsClickLock(true);
                    //��չʾģʽ
                    //�����б�����
                    //���㵱ǰ�����������ṹ���ṹ��TreeViewItemֻ��Name��Ϣ,�ñ���洢��Ϣ��
                    tempUIPanel_ShowInfPanel.InitViewData(CreateViewListByScene_new(), (node) => {//�б�ѡ���¼�
                        //��ȡ���ݶ�Ӧ����
                        GameObject obj = FindNodeObj(node);

                        //����洢��Ϣ
                        StoredTreeViewItem storedTVI = node.Item as StoredTreeViewItem;
                        // Debug.Log(storedTVI.ID + storedTVI.Name);

                       // Debug.Log(node.Path.Count);


                        if (obj == null) return;

                        //���ݼ���
                        Bounds box = CommonHelper.GetObjBounds(obj);//��ȡ�߽�
                        float scaleValue = CommonHelper.GetObjSuitDistance(obj);//�뾶

                        Vector3 labelOffset = new Vector3(0, box.size.y/2 + Mathf.Clamp(box.size.y,0,1), 0);
                         //���ɱ�ǩ����ǩ��Ӧ�¼���
                        CreateLabel(node.Item.Name,node.Path.Count.ToString(), obj,  box.center, labelOffset, scaleValue, ToolLabelManager.ToolLabelType.UI, () => {
                            //��ǩ����¼���
                            //����Ϣ��ʾ���
                            Debug.Log(storedTVI.ID);

                            Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewData, node));
                        });
                        //��ͷ�ƶ����������������㣩
                        CameraLookAtObj(box.center+ labelOffset, 0, 30, scaleValue, 0);
                    }, 
                    (node) => { 

                        //δѡ��

                    });
                    //CreateViewListByScene();
                    //tempUIPanel_ShowInfPanel.InitViewData(shelvesList);
                }
                break;


            case Stored3DState.Edit:
                if (_mainState == Stored3DState.None)
                {
                    IsClickLock(true);
                    //��չʾģʽ
                    //�����б�����
                    //���㵱ǰ�����������ṹ���ṹ��ֻ��Name��Ϣ��
                    tempUIPanel_ShowInfPanel.InitViewData(CreateViewListByScene_new(), (node) =>
                    {
                        nowSelectNode = node;

                        GameObject obj = FindNodeObj(node);
                        StoredItemBase dabase = obj.GetComponent<StoredItemBase>();
                        
                        Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.EditorPanel, dabase));
                        ////�б�ѡ���¼�
                        ////��ȡ���ݶ�Ӧ����
                        //GameObject obj = FindNodeObj(node);

                        ////����洢��Ϣ
                        //StoredTreeViewItem storedTVI = node.Item as StoredTreeViewItem;
                        ////Debug.Log(storedTVI.ID + storedTVI.Name);

                        //if (obj == null) return;

                        ////���ݼ���
                        //Bounds box = CommonHelper.GetObjBounds(obj);//��ȡ�߽�
                        //float scaleValue = box.size.magnitude / 2 + 1;//�뾶

                        //Vector3 labelOffset = new Vector3(0, box.size.y / 2 + Mathf.Clamp(box.size.y, 0, 1), 0);
                        ////���ɱ�ǩ����ǩ��Ӧ�¼���
                        //CreateLabel(node.Item.Name, node.Path.Count.ToString(), obj, box.center, labelOffset, scaleValue, ToolLabelManager.ToolLabelType.UI, () =>
                        //{
                        //    //��ǩ����¼���
                        //    //����Ϣ��ʾ���
                        //    Debug.Log(storedTVI.ID);
                        //});
                        ////��ͷ�ƶ����������������㣩
                        //CameraLookAtObj(box.center + labelOffset, 0, 30, scaleValue, 0);
                    },
                    (node) =>
                    {

                        //δѡ��

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
                    //��ȡ���ݣ����ɲֿ��
                    string path = PathExtensions.StreamingAssetsPath() + "/SceneData/stock.json";
                    StartCoroutine(CommonHelper.GetText(path, (textStr) =>
                    {
                        //ƽ̨���ݽṹ
                        StockInf rs = JsonMapper.ToObject<StockInf>(textStr);
                        //DoReadCreate(rs);
                        Debug.Log(rs);
                        Debug.Log("�ֿ�����:" + rs.WAREHOUSES.Count);

                        //ƽ̨���ݽ���Ϊ���ṹ�������Ϣ�洢�����ṹ��
                        //��������ͼ
                        tempUIPanel_ShowInfPanel.InitViewData(CreateStockList(rs), (node) =>
                        {
                            //ѡ���Ӧ�Ĳֿ⣬����idȥ�ֵ��л�ȡ �òֿ�����ṹ
                            StoredTreeViewItem storedTVI = node.Item as StoredTreeViewItem;
                            if (facListTemp.ContainsKey(storedTVI.ID))
                                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.OpenStockInf, facListTemp[storedTVI.ID]));

                        },
                        (node) =>
                        {
                            //δѡ��
                        });

                    }, (errormsg) => {
                        Debug.LogError("���ݻ�ȡʧ��");
                    }));
                }
                break;
        }

        _mainState = nextState;
    }






}

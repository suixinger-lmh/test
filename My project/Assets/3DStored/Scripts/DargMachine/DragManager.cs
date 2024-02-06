using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Stored3D;
using UnityEngine.EventSystems;

//���ܵ�����
//1.���ã�����Ŀ�����壬������͵���y=0����������ƶ������������ã������������Ҽ����������廹ԭ
//2.��ȡ����Ŀ�����壬�ж��Ƿ�Ϊ�ֿ����壬������Ǻ��Ƴ�����������ȡ���л�����״̬
//3.��ȡ����Ŀ�����壬�ж��Ƿ�Ϊ�ɻ�ȡ���壬������Ǻ��Ƴ�����������ȡ����


//���Ż���̫�����ˣ���ͬ���ܿ��Գ�����̶����ܣ���������仯
//������//��������//��������//

//��קĿ��
//����ק����
//��ק��������

public class DragManager : MonoBehaviour, IBaseComponent
{

    public void InitComponent()
    {
        dragItemsCache = new List<DragItem>();
    }


    #region ����ӿ�



    public void SetViewCamera(Camera maincamera)
    {
        viewCamera = maincamera;
    }

    public DragItem GetOrAddDragItem(GameObject obj)
    {
        DragItem tempDrag = obj.GetComponent<DragItem>();
        if (tempDrag == null)
            tempDrag = obj.AddComponent<DragItem>();
        return tempDrag;
    }


    public void AddDragItemInCache(DragItem item)
    {
        if (!dragItemsCache.Contains(item))
        {
            dragItemsCache.Add(item);
        }
    }



    #endregion


    public enum DragType
    {
        None,
        PutDown,//���ù���
        PickUp,//��ȡ����
        PickUpOnlyTarget,//����Թ̶������ȡ
        Find,//��ȡ����
        Find_Light,//��ȡ��״̬����

        //�·�ʽ
        PickPut,//��ȡ����
        FindOnce,//ѡ��һ�Σ��˳�
        PutOnce,//����һ��
        FindOnce_Light,//ѡ��һ�θ������˳�
    }

    public DragType _dragType = DragType.None;


    public Camera viewCamera;
    Vector2 mousePos;

    Ray screenRay;
    RaycastHit raycastHit;
    public GameObject targetObj;


    Vector3 floorNormal;



    List<DragItem> dragItemsCache;



    #region ���ù���

    GameObject _putTarget;
    Vector3 startEuler;
    Vector3 startPos;
    Vector3 endPos;
    float objHeight;
    float floorHeight = 0;//����߶ȣ�Ĭ��0
    Vector3 objLowPointWorldPos;//ģ����͵�
    float scrollHeight = 0;//����ƫ�Ƹ߶�
    //Ŀ�������ʼ��
    void Put_InitTargetObj(GameObject obj)
    {
        //��ʼλ�ü�¼
        startPos = obj.transform.position;
        startEuler = obj.transform.localEulerAngles;
        endPos = startPos;

        if(obj.GetComponent<StoredItemBase>()!= null)
        {
            floorHeight = obj.GetComponent<StoredItemBase>().GetRelativeHeightValue();
        }
        //if(obj.GetComponent<DragItem>().draglayer == CommonHelper.TempShelves)
        //{
        //    floorHeight = 0;
        //}
        //��ȡһ���������λ�õ�
        //:::::��Ϊʹ�õ����������꣬������͵��Ƕ�̬�仯�ģ������ƶ���͵�Ҳ�ƶ�
        //:::::�˴���������͵㣬ֻ���˱����mesh����������ģ�ͣ���Ҫ������ȡ���������ϵ����λ��
        //objLowPointWorldPos = CommonHelper.FindLowPositionLoop(obj.transform);
        ////�������ĵ���Ҫ���ƾ���
        //objHeight = obj.transform.position.y - objLowPointWorldPos.y;
        //objHeight = CommonHelper.GetObjGroundHeightValue(obj.transform.position,obj);
        //�������屾��Ӱ�����λ��
        obj.GetComponent<Collider>().enabled = false;
    }

    void Put_EndDO()
    {
        _putTarget.GetComponent<Collider>().enabled = true;
        //_putTarget.transform.position = endPos;
         targetObj = null;
        _putTarget = null;
    }

    //����������Ա���ȡ
    string floorLayer = string.Empty;
    public bool IsOnFloor(RaycastHit hit)
    {
        //��ǰ�汾�༭���޷�����tag��Ϣ����������ʽ�ж�
        //if (raycastHit.transform.tag == "target")
        //    return true;

        if (hit.transform.name == floorLayer)
            return true;
        return false;
    }
    //���ù�������
    //void PutUpdate(Ray needRay)
    //{
    //    if (targetObj != null)
    //    {
    //        //��δ��ʼ��
    //        if (_putTarget != targetObj)
    //        {
    //            _putTarget = targetObj;
    //            Put_InitTargetObj(_putTarget);
    //        }


    //        //�����ڵذ���
    //        if (Physics.Raycast(needRay, out raycastHit) && IsOnFloor(raycastHit))
    //        {
    //            endPos = raycastHit.point;
    //            endPos.y = floorHeight + objHeight;//���λ�÷��ڵ���߶���

    //            //��ת
    //            if (Input.GetKey(KeyCode.R))
    //            {
    //                float rotateValue = Input.mouseScrollDelta.y;
    //                _putTarget.transform.Rotate(0, rotateValue, 0);
    //            }
    //            //else//���ֿ��Ƹ߶�
    //            //{
    //            //    float yFloat = Input.mouseScrollDelta.y;
    //            //    scrollHeight += yFloat;
    //            //}
    //            //if (Input.GetKey(KeyCode.F))
    //            //{
    //            //    endPos.y = floorHeight + objHeight;
    //            //}
    //            _putTarget.transform.position = endPos;

    //            //�������
    //            if (Input.GetMouseButtonDown(0))
    //            {
    //                //Debug.LogError("����");
    //                Put_EndDO();

    //                ChangeDrayType(DragType.PickUp, pickLayer, floorLayer);
    //            }
    //            //�Ҽ���ԭ
    //            if (Input.GetMouseButtonDown(1))
    //            {
    //                _putTarget.transform.position = startPos;
    //                _putTarget.transform.localEulerAngles = startEuler;
    //                Put_EndDO();

    //                ChangeDrayType(DragType.PickUp,pickLayer,floorLayer);
    //            }
    //        }
    //        else
    //        {

    //        }

    //    }
    //}

    void Put_End()
    {
        if (_putTarget != null)
        {
            _putTarget.transform.position = startPos;
            _putTarget.transform.localEulerAngles = startEuler;
            Put_EndDO();
        }
    }

    #endregion







    #region ��ȡ

    //����������Ա���ȡ
    string pickLayer = string.Empty;
    bool CanPickUp(GameObject hitObj)
    {
        if (hitObj.transform.parent.name == pickLayer)
            return true;
        return false;
    }
    bool CanPickUp(RaycastHit hit)
    {
        if (hit.transform.parent.name == pickLayer)
            return true;
        return false;
    }
    void Pick_InitTargetObj(GameObject obj)
    {
        //��ѡȡ
        if (CanPickUp(obj))
        {
            //����
        }
        else
        {

        }
    }
    void PickUpUpdate(Ray needRay)
    {
        if (Physics.Raycast(needRay, out raycastHit))
        {
            //��δ��ʼ��
            if (targetObj != raycastHit.transform.gameObject)
            {
                targetObj = raycastHit.transform.gameObject;
                Pick_InitTargetObj(targetObj);
            }
            else
            {
                //�������
                if (CanPickUp(targetObj))
                {
                    targetObj.GetComponent<DragItem>().HighOn();
                    //���ѡȡ
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Debug.LogError("��ȡ");
                        ChangeDrayType(DragType.PutDown,pickLayer,floorLayer);
                    }
                }

            }


               
            //if (IsFactory(raycastHit))//��ѡȡ
            //{
            //    //���ѡȡ
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        //Debug.LogError("��ȡ");
            //        ChangeDrayType( DragType.PutDown);
            //    }
            //}
        }
    }

    void Pick_End()
    {

    }

    #endregion


    #region ��ȡ(���ֿ�)

    public GameObject findObj;
    //void Find_InitTargetObj(GameObject obj)
    //{
    //    //��ѡȡ
    //    if (CommonHelper.IsFactory(obj))
    //    {
    //        //����

    //    }
    //    else
    //    {

    //    }
    //}
    //void FindUpdate(Ray needRay)
    //{
    //    if (Physics.Raycast(needRay, out raycastHit))
    //    {
    //        //��δ��ʼ��
    //        if (targetObj != raycastHit.transform.gameObject)
    //        {
    //            targetObj = raycastHit.transform.gameObject;
    //            Find_InitTargetObj(targetObj);
    //        }
    //        else
    //        {
    //            if (CommonHelper.IsFactory(targetObj))//��ѡȡ
    //            {
    //                targetObj.GetComponent<DragItem>().HighOn();
    //                //���ѡȡ
    //                if (Input.GetMouseButtonDown(0))
    //                {
    //                    findObj = targetObj;
    //                    //findObj.GetComponent<DragItem>().FlashOn();
    //                    ChangeDrayType(DragType.None);
    //                }
    //            }
    //        }

          
    //    }
    //}




    #endregion

    #region ��ȡ(����������)

    public void SetFindLightObj(GameObject obj)
    {
        selectObj = obj;
        selectObj.GetComponent<DragItem>().FlashOn();
    }
    public void UnsetFindLightObj(GameObject obj)
    {
        selectObj = null;
        obj.GetComponent<DragItem>().FlashOff();
    }
    //public GameObject findObj;
    //void Find_InitTargetObj1(GameObject obj)
    //{
    //    //��ѡȡ
    //    if (CommonHelper.IsFactory(obj))
    //    {
    //        //����

    //    }
    //    else
    //    {

    //    }
    //}
    //void FindLightUpdate(Ray needRay)
    //{
    //    if (findObj == null)
    //    {
    //        if (Physics.Raycast(needRay, out raycastHit))
    //        {
    //            //��δ��ʼ��
    //            if (targetObj != raycastHit.transform.gameObject)
    //            {
    //                targetObj = raycastHit.transform.gameObject;
    //                Find_InitTargetObj(targetObj);
    //            }
    //            else
    //            {
    //                if (CanPickUp(targetObj))//��ѡȡ
    //                {
    //                    targetObj.GetComponent<DragItem>().HighOn();
    //                    //���ѡȡ
    //                    if (Input.GetMouseButtonDown(0))
    //                    {
    //                        findObj = targetObj;
    //                        findObj.GetComponent<DragItem>().FlashOn();
    //                        //ChangeDrayType(DragType.None);
    //                    }
    //                }
    //            }


    //        }
    //    }
    //    else
    //    {

    //    }

    //}
    #endregion




    public void ChangeDrayType(DragType type,string layerPick = "", string layerFloor = "")
    {
        if (_dragType == type) return;

        //��һ״̬����
        switch (_dragType)
        {
            case DragType.PickUp:
                Pick_End();
                break;
            case DragType.PutDown:
                Put_End();
                break;
            case DragType.Find:
                break;
            case DragType.None:
                break;
        }

        //Ŀ��״̬��ʼ
        switch (type)
        {
            case DragType.PickUp:
                pickLayer = layerPick;
                floorLayer = layerFloor;
                break;
            case DragType.PutDown:
                pickLayer = layerPick;
                floorLayer = layerFloor;
                break;
            case DragType.Find:
                findObj = null;
                break;
            case DragType.None:
                break;
            case DragType.Find_Light:
                findObj = null;
                pickLayer = layerPick;
                floorLayer = layerFloor;
                break;
            case DragType.PickUpOnlyTarget:

                break;
        }
        _dragType = type;
    }






    /// <summary>
    /// ������ִ�в���
    /// </summary>
    /// <param name="isOpen">�Ƿ�����</param>
    /// <param name="dragLayer">������㼶</param>
    /// <param name="floorLayer">������㼶</param>
    /// <param name="dg">ִ���߼�����</param>
    public void SetDragManager(bool isOpen,string dragLayer = "",string floorLayer = "",DragType dg = DragType.None)
    {
        ResetObjs();
        nowDragLayer = dragLayer;
        nowFloorLayer = floorLayer;
        _dragType = dg;
        enable = isOpen;
    }
    public GameObject GetSelectObj()
    {
        return selectObj;
    }

    public void SetPutObj(GameObject obj)
    {
        putObj = obj;
        Put_InitTargetObj(putObj);
    }

    void ResetObjs()
    {
        hitObj = null;
        putObj = null;
        selectObj = null;
    }


    public GameObject hitObj;//ɨ�赽����
    public GameObject putObj;//��������
    public GameObject selectObj;//ѡ������
    string nowDragLayer;
    string nowFloorLayer;
    bool enable = false;
    //�����˳�
    void DoRaycastExit()
    {

    }

    void DoRaycastEnter()
    {

    }
    void DoRaycastStay(GameObject obj)
    {
        obj.GetComponent<DragItem>().HighOn();
    }

    void DoSelect(GameObject obj)
    {
        switch (_dragType)
        {
            case DragType.PickPut:
                putObj = obj;
                Put_InitTargetObj(putObj);
                break;
            case DragType.FindOnce:
                enable = false;
                break;
            case DragType.FindOnce_Light:
                selectObj.GetComponent<DragItem>().FlashOn();
                enable = false;
                break;
        }
    }

    

    void DoStayOnFloor(RaycastHit hit)
    {
        if(_dragType == DragType.PickPut || _dragType == DragType.PutOnce)
        {
            endPos = hit.point;

            //��ȡ���淴���߶ȡ�����=��0   ��λ=������߶� ��
            
            endPos.y += floorHeight;//����߶���Ӧ

            //��ת
            if (Input.GetKey(KeyCode.R))
            {
                float rotateValue = Input.mouseScrollDelta.y;
                putObj.transform.Rotate(0, rotateValue, 0);
            }
            //else//���ֿ��Ƹ߶�
            //{
            //    float yFloat = Input.mouseScrollDelta.y;
            //    scrollHeight += yFloat;
            //}
            //if (Input.GetKey(KeyCode.F))
            //{
            //    endPos.y = floorHeight + objHeight;
            //}
            putObj.transform.position = endPos;
        }

        //switch (_dragType)
        //{
        //    case DragType.PickPut:

        //        endPos = hit.point;
        //        endPos.y = floorHeight + objHeight;//���λ�÷��ڵ���߶���

        //        //��ת
        //        if (Input.GetKey(KeyCode.R))
        //        {
        //            float rotateValue = Input.mouseScrollDelta.y;
        //            putObj.transform.Rotate(0, rotateValue, 0);
        //        }
        //        //else//���ֿ��Ƹ߶�
        //        //{
        //        //    float yFloat = Input.mouseScrollDelta.y;
        //        //    scrollHeight += yFloat;
        //        //}
        //        //if (Input.GetKey(KeyCode.F))
        //        //{
        //        //    endPos.y = floorHeight + objHeight;
        //        //}
        //        putObj.transform.position = endPos;
        //        break;
        //}

       
    }
    void DoPut()
    {
        switch (_dragType)
        {
            case DragType.PickPut:
                putObj.GetComponent<Collider>().enabled = true;
                putObj = null;
                break;
            case DragType.PutOnce:
                putObj.GetComponent<Collider>().enabled = true;
                putObj = null;
                enable = false;
                break;
        }
    }

    void DoPutCancel()
    {
        switch (_dragType)
        {
            case DragType.PickPut:
                putObj.transform.position = startPos;
                putObj.transform.localEulerAngles = startEuler;
                putObj.GetComponent<Collider>().enabled = true;
                putObj = null;
                break;
            case DragType.PutOnce:
                putObj.transform.position = startPos;
                putObj.transform.localEulerAngles = startEuler;
                putObj.GetComponent<Collider>().enabled = true;
                putObj = null;
                enable = false;
                break;
        }
    }

    bool IsInDragLayer(Transform trans)
    {
        DragItem tempDrag = trans.GetComponent<DragItem>();
        if (tempDrag == null)
            return false;
        //��������
        if (tempDrag.IsFloor)
            return false;
        else
        {
            //�㼶����
            if (tempDrag.Draglayer == nowDragLayer)
                return true;
            else
                return false;
        }
    }

    bool IsOnFloor(Transform trans)
    {
        DragItem tempDrag = trans.GetComponent<DragItem>();
        if (tempDrag == null)
            return false;
        if(tempDrag.Floorlayer == nowFloorLayer)//�㼶����
            return tempDrag.IsFloor;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enable)
        {
            //����UI
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            //�����Ļλ������
            screenRay = viewCamera.ScreenPointToRay(Input.mousePosition);
            //���߼��
            if (Physics.Raycast(screenRay, out raycastHit))
            {
                //��ȡ����(��������Ϊ�վ��Ǽ�ȡ����)
                if (putObj == null)
                {
                    //ɨ��㼶����
                    if (IsInDragLayer(raycastHit.transform))
                    {
                        //��δ��ʼ��
                        if (hitObj != raycastHit.transform.gameObject)
                        {
                            if (hitObj != null)
                            {
                                //�����˳�ִ��
                            }
                            hitObj = raycastHit.transform.gameObject;
                            //���߽���ִ��

                        }
                        else//update
                        {
                            //���߱���ִ��
                            DoRaycastStay(hitObj);

                            //���ѡ��ִ��
                            if (Input.GetMouseButtonDown(0))
                            {
                                selectObj = hitObj;
                                //��ȡִ��
                                DoSelect(selectObj);
                            }
                        }
                    }
                }
                else //���ò���
                {
                    //������
                    if (IsOnFloor(raycastHit.transform))
                    {
                        //�����ڵ�����ִ��
                        DoStayOnFloor(raycastHit);                      

                        //�������ִ��
                        if (Input.GetMouseButtonDown(0))
                        {
                            DoPut();
                            //Debug.LogError("����");
                            //Put_EndDO();

                            //ChangeDrayType(DragType.PickUp, pickLayer, floorLayer);
                        }
                        //�Ҽ���ԭִ��
                        if (Input.GetMouseButtonDown(1))
                        {
                            DoPutCancel();
                            //_putTarget.transform.position = startPos;
                            //_putTarget.transform.localEulerAngles = startEuler;
                            //Put_EndDO();

                            //ChangeDrayType(DragType.PickUp, pickLayer, floorLayer);
                        }
                    }
                }

            }

        }


    
        //Ray screenRay = viewCamera.ScreenPointToRay(Input.mousePosition);
      
        //if (Physics.Raycast(screenRay,out raycastHit))
        //{
        //    DrawLine(screenRay.origin, screenRay.direction, 10, Color.green);

        //    DrawLine(raycastHit.point, raycastHit.normal, 10, Color.red);


        //    if (targetObj != null)
        //    {
        //        //if (floorNormal != raycastHit.normal)
        //        //{
        //        //    floorNormal = raycastHit.normal;

                  
        //        //    //����͵�
        //        //}

        //        FindLowPosition(targetObj.transform);


        //        //targetObj.transform.position = 
        //    }


        //}




    }



  


   



    void DrawLine(Vector3 startPos,Vector3 dir,float length,Color color)
    {
        Debug.DrawRay(startPos, dir, color, 0.1f);
    }

}

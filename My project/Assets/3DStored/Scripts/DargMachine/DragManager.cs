using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Stored3D;
using UnityEngine.EventSystems;

//功能点梳理：
//1.放置：存在目标物体，物体最低点在y=0，跟随鼠标移动，点击左键放置，滚轮升降。右键放弃，物体还原
//2.捡取：无目标物体，判断是否为仓库物体，高亮标记和移除，点击左键获取并切换放置状态
//3.获取：无目标物体，判断是否为可获取物体，高亮标记和移除，点击左键获取长亮


//待优化，太杂乱了，不同功能可以抽离出固定功能，添加条件变化
//找物体//拿起物体//放下物体//

//拖拽目标
//被拖拽物体
//拖拽放置区域

public class DragManager : MonoBehaviour, IBaseComponent
{

    public void InitComponent()
    {
        dragItemsCache = new List<DragItem>();
    }


    #region 对外接口



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
        PutDown,//放置功能
        PickUp,//捡取功能
        PickUpOnlyTarget,//仅针对固定物体捡取
        Find,//获取功能
        Find_Light,//获取，状态高亮

        //新方式
        PickPut,//捡取放置
        FindOnce,//选中一次，退出
        PutOnce,//放置一次
        FindOnce_Light,//选中一次高亮，退出
    }

    public DragType _dragType = DragType.None;


    public Camera viewCamera;
    Vector2 mousePos;

    Ray screenRay;
    RaycastHit raycastHit;
    public GameObject targetObj;


    Vector3 floorNormal;



    List<DragItem> dragItemsCache;



    #region 放置功能

    GameObject _putTarget;
    Vector3 startEuler;
    Vector3 startPos;
    Vector3 endPos;
    float objHeight;
    float floorHeight = 0;//地面高度，默认0
    Vector3 objLowPointWorldPos;//模型最低点
    float scrollHeight = 0;//滚轮偏移高度
    //目标物体初始化
    void Put_InitTargetObj(GameObject obj)
    {
        //初始位置记录
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
        //获取一次物体最低位置点
        //:::::因为使用的是世界坐标，所以最低点是动态变化的，物体移动最低点也移动
        //:::::此处找物体最低点，只用了本身的mesh，如果是组合模型，需要迭代获取整个物体上的最低位置
        //objLowPointWorldPos = CommonHelper.FindLowPositionLoop(obj.transform);
        ////物体中心点需要上移距离
        //objHeight = obj.transform.position.y - objLowPointWorldPos.y;
        //objHeight = CommonHelper.GetObjGroundHeightValue(obj.transform.position,obj);
        //避免物体本身影响放置位置
        obj.GetComponent<Collider>().enabled = false;
    }

    void Put_EndDO()
    {
        _putTarget.GetComponent<Collider>().enabled = true;
        //_putTarget.transform.position = endPos;
         targetObj = null;
        _putTarget = null;
    }

    //哪类物体可以被捡取
    string floorLayer = string.Empty;
    public bool IsOnFloor(RaycastHit hit)
    {
        //当前版本编辑器无法保存tag信息，换其他方式判定
        //if (raycastHit.transform.tag == "target")
        //    return true;

        if (hit.transform.name == floorLayer)
            return true;
        return false;
    }
    //放置功能流程
    //void PutUpdate(Ray needRay)
    //{
    //    if (targetObj != null)
    //    {
    //        //尚未初始化
    //        if (_putTarget != targetObj)
    //        {
    //            _putTarget = targetObj;
    //            Put_InitTargetObj(_putTarget);
    //        }


    //        //射线在地板上
    //        if (Physics.Raycast(needRay, out raycastHit) && IsOnFloor(raycastHit))
    //        {
    //            endPos = raycastHit.point;
    //            endPos.y = floorHeight + objHeight;//最低位置放在地面高度上

    //            //旋转
    //            if (Input.GetKey(KeyCode.R))
    //            {
    //                float rotateValue = Input.mouseScrollDelta.y;
    //                _putTarget.transform.Rotate(0, rotateValue, 0);
    //            }
    //            //else//滚轮控制高度
    //            //{
    //            //    float yFloat = Input.mouseScrollDelta.y;
    //            //    scrollHeight += yFloat;
    //            //}
    //            //if (Input.GetKey(KeyCode.F))
    //            //{
    //            //    endPos.y = floorHeight + objHeight;
    //            //}
    //            _putTarget.transform.position = endPos;

    //            //左键放置
    //            if (Input.GetMouseButtonDown(0))
    //            {
    //                //Debug.LogError("放下");
    //                Put_EndDO();

    //                ChangeDrayType(DragType.PickUp, pickLayer, floorLayer);
    //            }
    //            //右键还原
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







    #region 捡取

    //哪类物体可以被捡取
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
        //可选取
        if (CanPickUp(obj))
        {
            //高亮
        }
        else
        {

        }
    }
    void PickUpUpdate(Ray needRay)
    {
        if (Physics.Raycast(needRay, out raycastHit))
        {
            //尚未初始化
            if (targetObj != raycastHit.transform.gameObject)
            {
                targetObj = raycastHit.transform.gameObject;
                Pick_InitTargetObj(targetObj);
            }
            else
            {
                //物体高亮
                if (CanPickUp(targetObj))
                {
                    targetObj.GetComponent<DragItem>().HighOn();
                    //左键选取
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Debug.LogError("捡取");
                        ChangeDrayType(DragType.PutDown,pickLayer,floorLayer);
                    }
                }

            }


               
            //if (IsFactory(raycastHit))//可选取
            //{
            //    //左键选取
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        //Debug.LogError("捡取");
            //        ChangeDrayType( DragType.PutDown);
            //    }
            //}
        }
    }

    void Pick_End()
    {

    }

    #endregion


    #region 获取(仅仓库)

    public GameObject findObj;
    //void Find_InitTargetObj(GameObject obj)
    //{
    //    //可选取
    //    if (CommonHelper.IsFactory(obj))
    //    {
    //        //高亮

    //    }
    //    else
    //    {

    //    }
    //}
    //void FindUpdate(Ray needRay)
    //{
    //    if (Physics.Raycast(needRay, out raycastHit))
    //    {
    //        //尚未初始化
    //        if (targetObj != raycastHit.transform.gameObject)
    //        {
    //            targetObj = raycastHit.transform.gameObject;
    //            Find_InitTargetObj(targetObj);
    //        }
    //        else
    //        {
    //            if (CommonHelper.IsFactory(targetObj))//可选取
    //            {
    //                targetObj.GetComponent<DragItem>().HighOn();
    //                //左键选取
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

    #region 获取(高亮，物体)

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
    //    //可选取
    //    if (CommonHelper.IsFactory(obj))
    //    {
    //        //高亮

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
    //            //尚未初始化
    //            if (targetObj != raycastHit.transform.gameObject)
    //            {
    //                targetObj = raycastHit.transform.gameObject;
    //                Find_InitTargetObj(targetObj);
    //            }
    //            else
    //            {
    //                if (CanPickUp(targetObj))//可选取
    //                {
    //                    targetObj.GetComponent<DragItem>().HighOn();
    //                    //左键选取
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

        //上一状态结束
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

        //目标状态初始
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
    /// 管理器执行参数
    /// </summary>
    /// <param name="isOpen">是否运行</param>
    /// <param name="dragLayer">物体检测层级</param>
    /// <param name="floorLayer">地面检测层级</param>
    /// <param name="dg">执行逻辑类型</param>
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


    public GameObject hitObj;//扫描到物体
    public GameObject putObj;//放置物体
    public GameObject selectObj;//选中物体
    string nowDragLayer;
    string nowFloorLayer;
    bool enable = false;
    //射线退出
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

            //获取地面反馈高度【网格=》0   库位=》地面高度 】
            
            endPos.y += floorHeight;//物体高度适应

            //旋转
            if (Input.GetKey(KeyCode.R))
            {
                float rotateValue = Input.mouseScrollDelta.y;
                putObj.transform.Rotate(0, rotateValue, 0);
            }
            //else//滚轮控制高度
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
        //        endPos.y = floorHeight + objHeight;//最低位置放在地面高度上

        //        //旋转
        //        if (Input.GetKey(KeyCode.R))
        //        {
        //            float rotateValue = Input.mouseScrollDelta.y;
        //            putObj.transform.Rotate(0, rotateValue, 0);
        //        }
        //        //else//滚轮控制高度
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
        //地面区分
        if (tempDrag.IsFloor)
            return false;
        else
        {
            //层级区分
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
        if(tempDrag.Floorlayer == nowFloorLayer)//层级区分
            return tempDrag.IsFloor;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enable)
        {
            //屏蔽UI
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            //鼠标屏幕位置射线
            screenRay = viewCamera.ScreenPointToRay(Input.mousePosition);
            //射线检测
            if (Physics.Raycast(screenRay, out raycastHit))
            {
                //捡取操作(放置物体为空就是捡取操作)
                if (putObj == null)
                {
                    //扫描层级过滤
                    if (IsInDragLayer(raycastHit.transform))
                    {
                        //尚未初始化
                        if (hitObj != raycastHit.transform.gameObject)
                        {
                            if (hitObj != null)
                            {
                                //射线退出执行
                            }
                            hitObj = raycastHit.transform.gameObject;
                            //射线进入执行

                        }
                        else//update
                        {
                            //射线保持执行
                            DoRaycastStay(hitObj);

                            //左键选中执行
                            if (Input.GetMouseButtonDown(0))
                            {
                                selectObj = hitObj;
                                //捡取执行
                                DoSelect(selectObj);
                            }
                        }
                    }
                }
                else //放置操作
                {
                    //地面检测
                    if (IsOnFloor(raycastHit.transform))
                    {
                        //保持在地面上执行
                        DoStayOnFloor(raycastHit);                      

                        //左键放置执行
                        if (Input.GetMouseButtonDown(0))
                        {
                            DoPut();
                            //Debug.LogError("放下");
                            //Put_EndDO();

                            //ChangeDrayType(DragType.PickUp, pickLayer, floorLayer);
                        }
                        //右键还原执行
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

                  
        //        //    //找最低点
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

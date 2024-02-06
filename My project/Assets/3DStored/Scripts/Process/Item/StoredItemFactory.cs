using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StoredItemFactory : StoredItemBase
{
    public static int SceneFacCount = 0;

    //JDataHelper _jDataHelper;

    BoxCollider groundBox;
    float relativeHeight = 0;


   

    public override float GetRelativeHeightValue()
    {
        return relativeHeight;
    }

    public override void BindComponent<T>(T data)
    {
        try
        {
            //仓库地面参照物
            groundBox = GetComponent<BoxCollider>();
            /////////////////////作为被摆放对象

            //////////////////摆放对象
            //仓库摆放相对高度  【实际高度 - 地面射线对象高度】
            relativeHeight = CommonHelper.GroundFLOORHEIGHT - CommonHelper.GRIDHEIGHT;
            relativeHeight += CommonHelper.GetObjSelfHeightValue(transform.position,groundBox);

            //拖拽组件初始化
            Manager3DStored.Instance.GetStoredComponent<DragManager>().GetOrAddDragItem(gameObject).InitDragFunc(CommonHelper.Factory, CommonHelper.Floor);
            //数据绑定
            SetData(data);
            //_jDataHelper = gameObject.AddComponent<JDataHelper>();
            //_jDataHelper.SetData<T>(data);

            _type = StoredItemType.Factory;
            canUse = true;
        }
        catch(Exception ex)
        {
            canUse = false;
            Debug.LogError("仓储组件绑定失败！原因:" + ex.Message);
        }
       
    }
    public void InitOp()
    {
        if (canUse)
        {
            //仓库初始位置：
            transform.position = CommonHelper.GetObjPositionByGroundHeight(Vector3.zero, groundBox, CommonHelper.GroundFLOORHEIGHT);
            //仓库初始名称：(数据显示名称+时间+个数)
            SetName(GetData<Factory>().abRes.LabelName + "_" + DateTime.Now.ToShortTimeString() + "_" + SceneFacCount);

            SceneFacCount++;
        }
    }
    public void InitOp(string name,Vector3 pos)
    {
        if (!canUse) return;

        transform.position = pos;
        SetName(name);
    }
    
    public Bounds GetGroundBounds()
    {
        return groundBox.bounds;
    }

}

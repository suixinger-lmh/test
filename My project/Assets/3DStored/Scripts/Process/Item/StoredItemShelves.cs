using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StoredItemShelves : StoredItemBase
{
    public static int SceneShelvesCount = 0;
    //JDataHelper _jDataHelper;
    float relativeHeight = 0;
    public override void BindComponent<T>(T data)
    {
        try
        {
            relativeHeight += 0;//在接触面上放置
            relativeHeight += CommonHelper.GetObjSelfHeightValue(transform.position, gameObject);//物体自身偏移

            //Debug.Log("长度偏移:"+relativeHeight);

            //组件添加
            Manager3DStored.Instance.GetStoredComponent<DragManager>().GetOrAddDragItem(gameObject).InitDragFunc(CommonHelper.TempShelves, CommonHelper.TempArea);

            //数据绑定
            SetData(data);
            //_jDataHelper = gameObject.AddComponent<JDataHelper>();
            //_jDataHelper.SetData<T>(data);

            _type = StoredItemType.Shelves;
            canUse = true;
        }
        catch (Exception ex)
        {
            canUse = false;
            Debug.LogError("仓储组件绑定失败！原因:" + ex.Message);
        }
    }

    public void InitOp(string name,Vector3 pos)
    {
        if (!canUse) return;

        transform.position = pos;
        SetName(name);
    }

    public void InitOp(Vector3 center,float height)
    {
        if (canUse)
        {
            //货架初始位置：（库位的中心，仓库地面高度）
            transform.position = CommonHelper.GetObjPositionByGroundHeight(center, gameObject, height);
            //初始名称：
            SetName(GetData<Shelves>().abRes.LabelName + "_" + DateTime.Now.ToShortTimeString() + "_" + SceneShelvesCount);
            //gameObject.name = _jDataHelper.GetData<Shelves>().LabelName + "_" + DateTime.Now.ToShortTimeString() + "_" + SceneShelvesCount;

            SceneShelvesCount++;
        }
    }


    public GameObject GetCloneObj()
    {
        //获取一个克隆物体，并初始化
        GameObject tempobj = Instantiate(gameObject);

        //名称
        tempobj.name = GetName() + "-" + SceneShelvesCount;
        SceneShelvesCount++;
        //组件初始化
        tempobj.GetComponent<DragItem>().InitDragFunc(GetComponent<DragItem>());

        return tempobj;
    }


    public override float GetRelativeHeightValue()
    {
        return relativeHeight;
    }
}

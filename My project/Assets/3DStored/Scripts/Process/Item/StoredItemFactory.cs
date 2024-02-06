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
            //�ֿ���������
            groundBox = GetComponent<BoxCollider>();
            /////////////////////��Ϊ���ڷŶ���

            //////////////////�ڷŶ���
            //�ֿ�ڷ���Ը߶�  ��ʵ�ʸ߶� - �������߶���߶ȡ�
            relativeHeight = CommonHelper.GroundFLOORHEIGHT - CommonHelper.GRIDHEIGHT;
            relativeHeight += CommonHelper.GetObjSelfHeightValue(transform.position,groundBox);

            //��ק�����ʼ��
            Manager3DStored.Instance.GetStoredComponent<DragManager>().GetOrAddDragItem(gameObject).InitDragFunc(CommonHelper.Factory, CommonHelper.Floor);
            //���ݰ�
            SetData(data);
            //_jDataHelper = gameObject.AddComponent<JDataHelper>();
            //_jDataHelper.SetData<T>(data);

            _type = StoredItemType.Factory;
            canUse = true;
        }
        catch(Exception ex)
        {
            canUse = false;
            Debug.LogError("�ִ������ʧ�ܣ�ԭ��:" + ex.Message);
        }
       
    }
    public void InitOp()
    {
        if (canUse)
        {
            //�ֿ��ʼλ�ã�
            transform.position = CommonHelper.GetObjPositionByGroundHeight(Vector3.zero, groundBox, CommonHelper.GroundFLOORHEIGHT);
            //�ֿ��ʼ���ƣ�(������ʾ����+ʱ��+����)
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

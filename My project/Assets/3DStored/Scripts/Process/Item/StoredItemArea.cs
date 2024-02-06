using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredItemArea : StoredItemBase
{

    BoxCollider box;

    
    //float relativeHeight = 0;
    public override void BindComponent<T>(T data)
    {
        try
        {
          

            //relativeHeight += 0;//�Ӵ������
            //relativeHeight += CommonHelper.

            //���box������
            RefreshBoxCollider();
            //box = gameObject.AddComponent<BoxCollider>();
            //box.size -= Vector3.one;//�Զ���ӵ�box��size��+1
            ////�߶�ȷ��(���е�λƽ���߶� - ��ʾ�����߶� = ʵ�ʵ���box�߶�)
            //LineRenderer tempLine = GetComponent<LineRenderer>();
            //float averageHeight = 0;
            //for(int i = 0; i < tempLine.positionCount; i++)
            //    averageHeight += tempLine.GetPosition(i).y;
            //averageHeight /= tempLine.positionCount;
            //float groundHeight = averageHeight - CommonHelper.AreaLineFloatHeight;
            //box.center = new Vector3(box.center.x, groundHeight, box.center.z);
            //box.enabled = false;

            //��ק�����ʼ��
            Manager3DStored.Instance.GetStoredComponent<DragManager>().GetOrAddDragItem(gameObject).InitDragFunc(CommonHelper.TempArea, CommonHelper.TempArea);
            ////�����ק
            //_dragManager.GetOrAddDragItem(nowArea).InitDragFunc(CommonHelper.TempArea, CommonHelper.TempArea, true);




            //���ݰ�
            SetData(data);
            //gameObject.AddComponent<JDataHelper>().SetData(data);

            _type = StoredItemType.Area;
            canUse = true;
        }
        catch (Exception ex)
        {
            canUse = false;
            Debug.LogError("�ִ������ʧ�ܣ�ԭ��:" + ex.Message);
        }
      
    }

    public void InitOp()
    {
        GetComponent<LineRenderer>().startColor = Color.yellow;
        GetComponent<LineRenderer>().endColor = Color.yellow;
    }


    public override float GetRelativeHeightValue()
    {
        return 0;
    }



    public void ChangeAreaBoxCollider(bool isShow)
    {
        box.enabled = isShow;
    }

    public void SetAreaFloor(bool isfloor)
    {
        //��Ϊ����
        Manager3DStored.Instance.GetStoredComponent<DragManager>().GetOrAddDragItem(gameObject).InitDragFunc(CommonHelper.TempArea, CommonHelper.TempArea, isfloor);
    }


    public float GetViewHeight()
    {

        //return CommonHelper.GetObjBounds(gameObject).center;
        return GetBoxBounds().center.y + CommonHelper.AreaLineFloatHeight;
    }
    public Bounds GetBoxBounds()
    {
        Bounds boxBound;
        bool isopen = box.enabled;
        box.enabled = true;
        boxBound = box.bounds;
        box.enabled = isopen;
        return boxBound;
    }

    public void RefreshBoxCollider()
    {
        if (box != null)
            DestroyImmediate(box);

        //���box������
        box = gameObject.AddComponent<BoxCollider>();
        box.size -= Vector3.one;//�Զ���ӵ�box��size��+1
        //�߶�ȷ��(���е�λƽ���߶� - ��ʾ�����߶� = ʵ�ʵ���box�߶�)
        LineRenderer tempLine = GetComponent<LineRenderer>();
        float averageHeight = 0;
        for (int i = 0; i < tempLine.positionCount; i++)
            averageHeight += tempLine.GetPosition(i).y;
        averageHeight /= tempLine.positionCount;
        float groundHeight = averageHeight - CommonHelper.AreaLineFloatHeight;
        box.center = new Vector3(box.center.x, groundHeight, box.center.z);
        box.enabled = false;

        //Debug.LogError("111"+ box.center+ box.size);
    }
    
}

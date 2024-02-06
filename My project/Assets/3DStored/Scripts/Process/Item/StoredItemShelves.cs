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
            relativeHeight += 0;//�ڽӴ����Ϸ���
            relativeHeight += CommonHelper.GetObjSelfHeightValue(transform.position, gameObject);//��������ƫ��

            //Debug.Log("����ƫ��:"+relativeHeight);

            //������
            Manager3DStored.Instance.GetStoredComponent<DragManager>().GetOrAddDragItem(gameObject).InitDragFunc(CommonHelper.TempShelves, CommonHelper.TempArea);

            //���ݰ�
            SetData(data);
            //_jDataHelper = gameObject.AddComponent<JDataHelper>();
            //_jDataHelper.SetData<T>(data);

            _type = StoredItemType.Shelves;
            canUse = true;
        }
        catch (Exception ex)
        {
            canUse = false;
            Debug.LogError("�ִ������ʧ�ܣ�ԭ��:" + ex.Message);
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
            //���ܳ�ʼλ�ã�����λ�����ģ��ֿ����߶ȣ�
            transform.position = CommonHelper.GetObjPositionByGroundHeight(center, gameObject, height);
            //��ʼ���ƣ�
            SetName(GetData<Shelves>().abRes.LabelName + "_" + DateTime.Now.ToShortTimeString() + "_" + SceneShelvesCount);
            //gameObject.name = _jDataHelper.GetData<Shelves>().LabelName + "_" + DateTime.Now.ToShortTimeString() + "_" + SceneShelvesCount;

            SceneShelvesCount++;
        }
    }


    public GameObject GetCloneObj()
    {
        //��ȡһ����¡���壬����ʼ��
        GameObject tempobj = Instantiate(gameObject);

        //����
        tempobj.name = GetName() + "-" + SceneShelvesCount;
        SceneShelvesCount++;
        //�����ʼ��
        tempobj.GetComponent<DragItem>().InitDragFunc(GetComponent<DragItem>());

        return tempobj;
    }


    public override float GetRelativeHeightValue()
    {
        return relativeHeight;
    }
}

using FrameWork;
using HighlightingSystem;
using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragItem : MonoBehaviour
{
    Highlighter _highLighter;
    public Color testc;

    public string draglayer;
    public string floorlayer;
    public bool isFloor = false;

    public bool IsFloor { get => isFloor; }
    public string Draglayer { get => draglayer; }
    public string Floorlayer { get => floorlayer; }

    public void InitDragFunc(string dragLay,string floorLay = "", bool isF = false)
    {
        isFloor = isF;//�Ƿ�Ϊ�ذ�
        draglayer = dragLay;//��ק�㼶
        floorlayer = floorLay;//��ק����㼶

        //�������
        _highLighter = GetComponent<Highlighter>();
        if (_highLighter == null)
            _highLighter = gameObject.AddComponent<Highlighter>();


        //�������manager����
        //Manager3DStored.Instance.GetStoredComponent<DragManager>().AddDragItemInCache(this);


        //�����¼�
        //Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener( CoreEventId.DragItemCall,)

    }

    public void InitDragFunc(DragItem item)
    {
        InitDragFunc(item.draglayer, item.floorlayer, item.isFloor);
    }




    //����ִ�д���ŷ��⣩
    public void HighOn()
    {
        _highLighter.On(Color.green);
    }

    //һֱ��˸
    public void FlashOn()
    {
        _highLighter.FlashingOn();
    }

    public void FlashOff()
    {
        _highLighter.FlashingOff();
    }

    public void ReInitObj()
    {
        _highLighter.ReinitMaterials();
    }

}

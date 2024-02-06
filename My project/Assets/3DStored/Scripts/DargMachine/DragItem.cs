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
        isFloor = isF;//是否为地板
        draglayer = dragLay;//拖拽层级
        floorlayer = floorLay;//拖拽区域层级

        //高亮组件
        _highLighter = GetComponent<Highlighter>();
        if (_highLighter == null)
            _highLighter = gameObject.AddComponent<Highlighter>();


        //自身加入manager缓存
        //Manager3DStored.Instance.GetStoredComponent<DragManager>().AddDragItemInCache(this);


        //监听事件
        //Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener( CoreEventId.DragItemCall,)

    }

    public void InitDragFunc(DragItem item)
    {
        InitDragFunc(item.draglayer, item.floorlayer, item.isFloor);
    }




    //亮（执行代码才发光）
    public void HighOn()
    {
        _highLighter.On(Color.green);
    }

    //一直闪烁
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stored3D;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DrawTool : MonoBehaviour,IBaseComponent
{
    //功能需要初始化对象
    Camera viewCamera;
    Transform lineParent;

    public enum DrawStyle
    {
        rect,//矩形
        cicle,//圆形
        line,//线段
    }


    public enum DrawState
    {
        None,//空,准备状态
        Begin,//确定起始点
        Drawing,//等待确定终止点
        End,//绘制结束
    }
    public DrawState _state = DrawState.None;

    public bool isDraw = false;
    public bool isMove = false;

    public DrawStyle _nowDrawStyle = DrawStyle.rect;


    GameObject lineRenderPrefab;
    LineRenderer _tempLineRender; 

    static int nameCount = 0;
    public void InitComponent()
    {
        if (lineRenderPrefab == null)
        {
            lineRenderPrefab = Resources.Load("AreaLine") as GameObject;
        }
        //nameCount = 0;
    }

    //该组件功能依赖物体
    public void SetNeeded(Camera camera,Transform lp)
    {
        viewCamera = camera;
        lineParent = lp;
    }


    public void ChangeDrawStyle(DrawStyle ds)
    {
        _nowDrawStyle = ds;
    }

    public void EnabelDraw() { isDraw = true; }
    public void DisableDraw() {
        //重新绘制
        if (_tempLineRender != null)
        {
            _tempLineRender.positionCount = 0;
            _state = DrawState.None;
        }
        isDraw = false;
        isMove = false;
    }

    public void SaveLineRender(Transform tempArea,Area area)
    {
        //保存库位时，将绘制高度改变为显示高度
        for(int i = 0; i < _tempLineRender.positionCount; i++)
        {
            _tempLineRender.SetPosition(i, _tempLineRender.GetPosition(i) + Vector3.up * (CommonHelper.AreaLineFloatHeight - CommonHelper.AreaLineFloatHeight_Drawing));
        }


        _tempLineRender.transform.SetParent(tempArea);
        //_tempLineRender.startColor = Color.yellow;
        //_tempLineRender.endColor = Color.yellow;

        StoredItemArea areaItem = CommonHelper.BindArea_(_tempLineRender.gameObject, area);
        areaItem.InitOp();
        //JDataHelper jd = _tempLineRender.AddComponent<JDataHelper>();
        //jd.SetData<Area>(area);


        _tempLineRender = null;
        _state = DrawState.None;
    }

    //给出绘制信息
    public void GetDrawInf(out Vector3 vStart,out Vector3 vEnd)
    {
        vStart = startPos;
        vEnd = endPos;
    }
    public void GetDrawInf_line(out Vector3 vStart, out Vector3 vEnd)
    {
        if (_tempLineRender.positionCount < 2)
        {
            vStart = vEnd = Vector3.zero;
        }
        else
        {
            vStart = _tempLineRender.GetPosition(_tempLineRender.positionCount -2);
            vEnd = _tempLineRender.GetPosition(_tempLineRender.positionCount - 1);
        }
        
    }


    Vector3 startPos;//绘制起始点
    Vector3 endPos;//绘制终止点


    //绘制时，离地近距离
    //绘制完成时，按高度显示

    // Update is called once per frame
    void Update()
    {
        if (isDraw)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;


            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            switch (_state)
            {
                case DrawState.None://进入绘制准备，但是尚未开始画
                    //新对象资源
                    if (_tempLineRender == null)
                    {
                        nameCount++;
                        //初始化绘制对象
                        _tempLineRender = Instantiate(lineRenderPrefab, lineParent).GetComponent<LineRenderer>();
                        _tempLineRender.gameObject.name = "库位_" + nameCount;
                    }

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            //点击在仓库上
                            if (!CommonHelper.IsFactory(hit))
                                return;

                        
                            startPos = hit.point + Vector3.up * CommonHelper.AreaLineFloatHeight_Drawing;
                            _state = DrawState.Begin;
                        }
                    }
                    break;
                case DrawState.Begin://点出了起始位置后
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (!CommonHelper.IsFactory(hit))
                            return;


                        endPos = hit.point + Vector3.up * CommonHelper.AreaLineFloatHeight_Drawing;
                        switch (_nowDrawStyle)
                        {
                            case DrawStyle.rect:
                                DrawRect(endPos, startPos);
                                break;
                            case DrawStyle.cicle:
                                DrawCircle(startPos, endPos);
                                break;

                            case DrawStyle.line:
                                if (_tempLineRender.positionCount < 2)
                                    DrawLine(startPos, endPos);
                                else
                                    DrawLine(endPos);
                                break;
                        }


                        //点击右键取消
                        if (Input.GetMouseButtonDown(1))
                        {
                            _tempLineRender.positionCount = 0;
                            _state = DrawState.None;
                        }

                        if (Input.GetMouseButtonDown(0))
                        {
                            if(_nowDrawStyle == DrawStyle.line)
                            {
                                if(Vector3.Distance(endPos,_tempLineRender.GetPosition(0)) < 0.5f)
                                {
                                    DrawLine(startPos);
                                    _state = DrawState.Drawing;
                                }
                                else
                                    _tempLineRender.positionCount++;
                              
                            }
                            else
                            {
                                _state = DrawState.Drawing;
                            }

                        }

                    }



                    break;

                case DrawState.Drawing://绘制完成

                    //移动
                    if (isMove)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (!CommonHelper.IsFactory(hit))
                                return;

                            Vector3 moveAdd = hit.point + Vector3.up * CommonHelper.AreaLineFloatHeight_Drawing;
                            Vector3 centerP = (startPos + endPos) / 2;
                            moveAdd -= centerP;
                            startPos += moveAdd;
                            endPos += moveAdd;

                            switch (_nowDrawStyle)
                            {
                                case DrawStyle.rect:
                                    DrawRect(endPos, startPos);
                                    break;
                                case DrawStyle.cicle:
                                    DrawCircle(startPos, endPos);
                                    break;

                                case DrawStyle.line:

                                    ReDrawLine(moveAdd);
                                    break;
                            }
                          

                            if (Input.GetMouseButtonDown(0))
                            {
                                isMove = false;
                            }


                        }
                    }




                    break;

            }




        }
    }


    void DrawMovePoint(Vector3 moveAdd)
    {
        for(int i = 0;i< _tempLineRender.positionCount; i++)
        {
            _tempLineRender.SetPosition(i,_tempLineRender.GetPosition(i)+ moveAdd);
        }

        startPos = _tempLineRender.GetPosition(0);

    }
    //void DrawUpdate()
    //{
    //    switch (_nowDrawStyle)
    //    {
    //        case DrawStyle.rect:
    //            DrawRect();
    //            break;
    //        case DrawStyle.cicle:
    //            break;

    //        case DrawStyle.line:
    //            break;
    //    }
    //}
    public void DrawRect(Vector3 endP, Vector3 startP)
    {
        startPos = startP;
        endPos = endP;

        //endP += Vector3.up * CommonHelper.AreaLineFloatHeight;
        //startP += Vector3.up * CommonHelper.AreaLineFloatHeight;

        _tempLineRender.positionCount = 5;
        _tempLineRender.SetPosition(0, startP);
        _tempLineRender.SetPosition(1, new Vector3(endP.x, startP.y, startP.z));
        _tempLineRender.SetPosition(2, endP);
        _tempLineRender.SetPosition(3, new Vector3(startP.x, startP.y, endP.z));
        _tempLineRender.SetPosition(4, startP);
    }

    void DrawLine(Vector3 startP, Vector3 endP)
    {
        startPos = startP;
        endPos = endP;

        //endP += Vector3.up * CommonHelper.AreaLineFloatHeight;
        //startP += Vector3.up * CommonHelper.AreaLineFloatHeight;

        _tempLineRender.positionCount = 2;

        _tempLineRender.SetPosition(0, startP);
        _tempLineRender.SetPosition(1, endP);
    }
    void DrawLine(Vector3 endP)
    {
      //  startPos = startP;
        endPos = endP;

        //endP += Vector3.up * CommonHelper.AreaLineFloatHeight;
        //startP += Vector3.up * 0.1f;
        _tempLineRender.SetPosition(_tempLineRender.positionCount- 1, endP);
    }
    void ReDrawLine(Vector3 moveAdd)
    {
        moveAdd.y = 0;
        for (int i = 0; i < _tempLineRender.positionCount; i++)
        {
            _tempLineRender.SetPosition(i, _tempLineRender.GetPosition(i) + moveAdd);
        }
        startPos = _tempLineRender.GetPosition(0);
        endPos = _tempLineRender.GetPosition(_tempLineRender.positionCount - 1);
    }
    public void DrawCircle(Vector3 startP, Vector3 endP, int pointCount = 360)
    {
        startPos = startP;
        endPos = endP;

        //endP += Vector3.up * CommonHelper.AreaLineFloatHeight;
        //startP += Vector3.up * CommonHelper.AreaLineFloatHeight;

        //起点和中点连线中心为圆心
        Vector3 center = startP + endP;
        center /= 2;

        float radius = Vector3.Distance(startP, endP) / 2;

        //以y值固定的平面上画圆

        //点量算出角度分辨率
        float angleScale = 360 / pointCount;



        //绘制
        _tempLineRender.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            //以0°在右侧计算

            float posx = radius * Mathf.Cos(angleScale * i * Mathf.Deg2Rad);
            float posz = radius * Mathf.Sin(angleScale * i * Mathf.Deg2Rad);

            _tempLineRender.SetPosition(i, center + new Vector3(posx, 0, posz));
        }

    }





}

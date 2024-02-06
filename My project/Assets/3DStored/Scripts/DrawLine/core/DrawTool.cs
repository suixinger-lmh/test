using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stored3D;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DrawTool : MonoBehaviour,IBaseComponent
{
    //������Ҫ��ʼ������
    Camera viewCamera;
    Transform lineParent;

    public enum DrawStyle
    {
        rect,//����
        cicle,//Բ��
        line,//�߶�
    }


    public enum DrawState
    {
        None,//��,׼��״̬
        Begin,//ȷ����ʼ��
        Drawing,//�ȴ�ȷ����ֹ��
        End,//���ƽ���
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

    //�����������������
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
        //���»���
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
        //�����λʱ�������Ƹ߶ȸı�Ϊ��ʾ�߶�
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

    //����������Ϣ
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


    Vector3 startPos;//������ʼ��
    Vector3 endPos;//������ֹ��


    //����ʱ����ؽ�����
    //�������ʱ�����߶���ʾ

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
                case DrawState.None://�������׼����������δ��ʼ��
                    //�¶�����Դ
                    if (_tempLineRender == null)
                    {
                        nameCount++;
                        //��ʼ�����ƶ���
                        _tempLineRender = Instantiate(lineRenderPrefab, lineParent).GetComponent<LineRenderer>();
                        _tempLineRender.gameObject.name = "��λ_" + nameCount;
                    }

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            //����ڲֿ���
                            if (!CommonHelper.IsFactory(hit))
                                return;

                        
                            startPos = hit.point + Vector3.up * CommonHelper.AreaLineFloatHeight_Drawing;
                            _state = DrawState.Begin;
                        }
                    }
                    break;
                case DrawState.Begin://�������ʼλ�ú�
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


                        //����Ҽ�ȡ��
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

                case DrawState.Drawing://�������

                    //�ƶ�
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

        //�����е���������ΪԲ��
        Vector3 center = startP + endP;
        center /= 2;

        float radius = Vector3.Distance(startP, endP) / 2;

        //��yֵ�̶���ƽ���ϻ�Բ

        //��������Ƕȷֱ���
        float angleScale = 360 / pointCount;



        //����
        _tempLineRender.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            //��0�����Ҳ����

            float posx = radius * Mathf.Cos(angleScale * i * Mathf.Deg2Rad);
            float posz = radius * Mathf.Sin(angleScale * i * Mathf.Deg2Rad);

            _tempLineRender.SetPosition(i, center + new Vector3(posx, 0, posz));
        }

    }





}

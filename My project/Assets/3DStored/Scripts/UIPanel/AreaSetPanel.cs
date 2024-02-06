using DG.Tweening;
using FrameWork;
using Stored3D;
using Sxer.Camera;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//绘制区域的帮助面板
public class AreaSetPanel : UIPanel
{
    //形态1：提示选取一个仓库【TODO:当前所有仓库选择列表】；退出编辑
    //形态2：
    //绘制面板：1.视角切换（转动和固定俯视角）2.绘制形状（矩形，圆形，连线）3.绘制信息[todo:可编辑]


    //绘制信息：当在绘制过程中时，动态显示

    //none状态：toggle可切换，视角移动可点击，保存不可点击，取消可点击


    DrawTool _drawTool;
    CameraOperateSet _cameraSet;
    GameObject _factoryObj;

    public Text title;
    public Transform p2;
    public GameObject inf_DRAW,inf_rect, inf_circle, inf_line;
    public Button draw_Save, draw_Cancel;
    [Header("Input")]
    public Button draw_Move;
    public InputField input_RectX, input_RectY, input_circleR,input_line;
    [Header("绘制物体")]
    public Toggle tg_cameraFly;
    public Toggle tg_cameraRot;
    public Toggle tg_cameraTop;
    public Toggle tg_DrawRect;
    public Toggle tg_DrawCircle;
    public Toggle tg_DrawLine;

    public Button exitBtn;


    DrawTool.DrawStyle _nowdrawSy;

    public void InitAreaSetPanel(DrawTool tool, CameraOperateSet cameraset, UnityAction closeBtnAc)
    {
        _drawTool = tool;
        _cameraSet = cameraset;
        //退出绘制
        exitBtn.onClick.AddListener(() => {

            ResetPanel();

            CloseUIPanel();

            closeBtnAc();
        });

        //视角响应事件：
        tg_cameraFly.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                //if (_drawTool.isDraw)//绘制开始后禁止切换(这里只禁用方法，toggle状态还会改变)
                //    return;
                //_cameraSet.ChangeCameraOperate(CameraState.LookTarget);
                _cameraSet.ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
            }
        });
        tg_cameraRot.onValueChanged.AddListener((ison) => {
            if (ison)
            {
                //if (_drawTool.isDraw)//绘制开始后禁止切换(这里只禁用方法，toggle状态还会改变)
                //    return;
                //_cameraSet.ChangeCameraOperate(CameraState.LookTarget);

                _cameraSet.ChangeCameraOperate(CameraOpState.LookTarget);
                _cameraSet.SetLookAtObj(_factoryObj);
                //CameraOpLookTarget cameralookat = (CameraOpLookTarget)_cameraSet.ChangeCameraOperate(CameraOpState.LookTarget);
                //cameralookat.target = _factoryObj.transform;
            }
        });
        tg_cameraTop.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                //if (_drawTool.isDraw)//绘制开始后禁止切换(这里只禁用方法，toggle状态还会改变)
                //    return;
                //_cameraSet.ChangeCameraOperate(CameraState.LookTarget);
                _cameraSet.ChangeCameraOperate(CameraOpState.TopView);
                _cameraSet.LookAtTargetAtTop(_factoryObj);
            }
        });

        //绘制图形选择
        tg_DrawRect.onValueChanged.AddListener((ison) =>
        {
            ChangeDrawTool(ison, DrawTool.DrawStyle.rect, inf_rect);
        });
        tg_DrawCircle.onValueChanged.AddListener((ison) =>
        {
            ChangeDrawTool(ison, DrawTool.DrawStyle.cicle, inf_circle);
        });
        tg_DrawLine.onValueChanged.AddListener((ison) =>
        {
            ChangeDrawTool(ison, DrawTool.DrawStyle.line, inf_line);
        });

        //输入框修改
        input_RectX.onEndEdit.AddListener((str) =>
        {
            if (CanChangeInput())
            {
                float x;
                //获取当前中心点
                Vector3 stp, endp;
                _drawTool.GetDrawInf(out stp, out endp);

                if (float.TryParse(str, out x))
                {
                    if (x > 0)//
                    {
                        //左延长还是右延长
                        float addX = endp.x > stp.x ? x : -x;
                        Vector3 newEndP = new Vector3(stp.x + addX, stp.y, endp.z);
                        _drawTool.DrawRect(newEndP, stp);
                        return;
                    }

                }
                //非法值
                ShowRectInf(stp, endp);
            }
        });
        input_RectY.onEndEdit.AddListener((str) =>
        {
            if (CanChangeInput())
            {
                float x;
                //获取当前中心点
                Vector3 stp, endp;
                _drawTool.GetDrawInf(out stp, out endp);

                if (float.TryParse(str, out x))
                {
                    if (x > 0)//
                    {
                        //左延长还是右延长
                        float addX = endp.z > stp.z ? x : -x;
                        Vector3 newEndP = new Vector3(endp.x, stp.y, stp.z + addX);
                        _drawTool.DrawRect(newEndP, stp);
                        return;
                    }

                }
                //非法值
                ShowRectInf(stp, endp);
            }
        });
        input_circleR.onEndEdit.AddListener((str) => {
            if (CanChangeInput())
            {
                float x;
                
                Vector3 stp, endp;
                _drawTool.GetDrawInf(out stp, out endp);

                if (float.TryParse(str, out x))
                {
                    if (x > 0)//
                    {
                        //中心
                        Vector3 centerPos = (stp + endp) / 2;
                        //圆心不变，扩展半径
                        Vector3 newStartPos = (stp - centerPos).normalized * x + new Vector3(centerPos.x,0,centerPos.z);
                        Vector3 newEndPos = (endp - centerPos).normalized * x + new Vector3(centerPos.x, 0, centerPos.z);
                        _drawTool.DrawCircle(newStartPos, newEndPos);
                        return;
                    }
                }
                //非法值
                ShowCircleInf(stp, endp);
            }
        });

        //移动
        draw_Move.onClick.AddListener(() => {
            if (CanChangeInput())
            {
                _drawTool.isMove = true;
            }
        });


        //保存当前区域
        draw_Save.onClick.AddListener(() => {
            if (CanChangeInput())
            {
                //保存到仓库物体上
                SaveAreaIntoFactory();
                //刷新区域列表面板
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("区域编辑_刷新", _factoryObj)));

                tg_DrawRect.isOn = false;
                tg_DrawCircle.isOn = false;
                tg_DrawLine.isOn = false;
            }
        });

    }



    void SaveAreaIntoFactory()
    {
        if (_factoryObj != null)
        {
            Transform areaTemp = CommonHelper.GetFactoryAreaParent(_factoryObj);

            Area areadata = new Area();
            areadata.SetShape(_nowdrawSy);
            //areadata.SetUsage();
            _drawTool.SaveLineRender(areaTemp,areadata);
            
        }
    }


    bool CanChangeInput()
    {
        if (_drawTool != null)
        {
            if (_drawTool.isDraw)
            {
                if (_drawTool._state == DrawTool.DrawState.Drawing)//存在图形后才能修改参数
                {
                    return true;
                }
            }
        }
        return false;
    }



    void ChangeDrawTool(bool ison,DrawTool.DrawStyle ds,GameObject inf_p)
    {
        if (ison)
        {
            if(_nowdrawSy != ds)
            {
                ClearInputData();
                _nowdrawSy = ds;
            }
          
            tg_cameraRot.interactable = false;
            tg_cameraTop.interactable = false;
            tg_cameraFly.interactable = false;

            _cameraSet.StopCamera();
            _drawTool.ChangeDrawStyle(ds);
            _drawTool.EnabelDraw();
            inf_DRAW.SetActive(true);
            inf_p.SetActive(true);
        }
        else
        {
            tg_cameraRot.interactable = true;
            tg_cameraTop.interactable = true;
            tg_cameraFly.interactable = true;

            _cameraSet.StartCamera();

            if(_drawTool._state == DrawTool.DrawState.Drawing)//绘制完成的状态,只隐藏，不清空
            {
                inf_DRAW.SetActive(false);
                inf_p.SetActive(false);
            }
            else//清空当前绘制数据
            {
               
                inf_DRAW.SetActive(false);
                inf_p.SetActive(false);

                ClearInputData();
            }
        }
    }

    void ClearInputData()
    {
        _drawTool.DisableDraw();//清空图形
        //clear
        input_RectX.text = string.Empty;
        input_RectY.text = string.Empty;
        input_circleR.text = string.Empty;
        input_line.text = string.Empty;
    }



    public override void TogglePanel(bool isOn)
    {
        if (isOn)
            OpenUIPanel();
        else
            CloseUIPanel();
    }

    Tween openTw;
    public override void OpenUIPanel()
    {
        if (openTw != null)
            openTw.Kill(true);
        base.OpenUIPanel();
        openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(120, 0), 1f);
    }

    public override void CloseUIPanel()
    {
        if (openTw != null)
            openTw.Kill(false);//关闭的时候不需要上个完成，当前位置关闭
        openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-120, 0), 1f).OnComplete(() =>
        {
            base.CloseUIPanel();
        });
    }



    public void ShowAreaSetStyle(GameObject factorObj)
    {
         _factoryObj = factorObj;
        title.text = "["+_factoryObj.name+"]";

        //绘制状态设置
        tg_cameraFly.isOn = false;
        tg_cameraFly.isOn = true;
        //tg_DrawRect.isOn = true;

        OpenUIPanel();
    }

    void ResetPanel()
    {
        tg_cameraFly.isOn = true;
        _factoryObj = null;
        title.text = "仓库名称";

        tg_DrawRect.isOn = false;
        tg_DrawCircle.isOn = false;
        tg_DrawLine.isOn = false;



        _drawTool.DisableDraw();
    }



    void ShowRectInf(Vector3 sp,Vector3 ep)
    {
        Vector2 rectinf = CommonHelper.DrawInfGet_RectInf(sp, ep);
        input_RectX.text = rectinf.x.ToString("f2");
        input_RectY.text = rectinf.y.ToString("f2");
    }

    void ShowCircleInf(Vector3 sp, Vector3 ep)
    {
        input_circleR.text = CommonHelper.DrawInfGet_CircleInf(sp, ep).ToString("f2");
    }
    void ShowLineInf(Vector3 sp, Vector3 ep)
    {
        input_line.text = Vector3.Distance(sp, ep).ToString("f2");
    }


    private void Update()
    {
        if (_drawTool != null)
        {
            if (_drawTool.isDraw)
            {
                if(_drawTool._state == DrawTool.DrawState.Begin)//不断显示信息
                {
                    Vector3 stp, endp;
                    _drawTool.GetDrawInf(out stp,out endp);

                    if (tg_DrawRect.isOn)//矩形
                    {
                        ShowRectInf(stp,endp);
                    }

                    if (tg_DrawCircle.isOn)//
                    {
                        ShowCircleInf(stp,endp);
                    }

                    if (tg_DrawLine.isOn)
                    {
                        _drawTool.GetDrawInf_line(out stp, out endp);
                        ShowLineInf(stp,endp);
                    }

                }


            }
        }





        
    }










}

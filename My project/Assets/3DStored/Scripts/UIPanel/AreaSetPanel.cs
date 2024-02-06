using DG.Tweening;
using FrameWork;
using Stored3D;
using Sxer.Camera;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//��������İ������
public class AreaSetPanel : UIPanel
{
    //��̬1����ʾѡȡһ���ֿ⡾TODO:��ǰ���вֿ�ѡ���б����˳��༭
    //��̬2��
    //������壺1.�ӽ��л���ת���͹̶����ӽǣ�2.������״�����Σ�Բ�Σ����ߣ�3.������Ϣ[todo:�ɱ༭]


    //������Ϣ�����ڻ��ƹ�����ʱ����̬��ʾ

    //none״̬��toggle���л����ӽ��ƶ��ɵ�������治�ɵ����ȡ���ɵ��


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
    [Header("��������")]
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
        //�˳�����
        exitBtn.onClick.AddListener(() => {

            ResetPanel();

            CloseUIPanel();

            closeBtnAc();
        });

        //�ӽ���Ӧ�¼���
        tg_cameraFly.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                //if (_drawTool.isDraw)//���ƿ�ʼ���ֹ�л�(����ֻ���÷�����toggle״̬����ı�)
                //    return;
                //_cameraSet.ChangeCameraOperate(CameraState.LookTarget);
                _cameraSet.ChangeCameraOperate(CameraOpState.SimpleFly_UNITY);
            }
        });
        tg_cameraRot.onValueChanged.AddListener((ison) => {
            if (ison)
            {
                //if (_drawTool.isDraw)//���ƿ�ʼ���ֹ�л�(����ֻ���÷�����toggle״̬����ı�)
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
                //if (_drawTool.isDraw)//���ƿ�ʼ���ֹ�л�(����ֻ���÷�����toggle״̬����ı�)
                //    return;
                //_cameraSet.ChangeCameraOperate(CameraState.LookTarget);
                _cameraSet.ChangeCameraOperate(CameraOpState.TopView);
                _cameraSet.LookAtTargetAtTop(_factoryObj);
            }
        });

        //����ͼ��ѡ��
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

        //������޸�
        input_RectX.onEndEdit.AddListener((str) =>
        {
            if (CanChangeInput())
            {
                float x;
                //��ȡ��ǰ���ĵ�
                Vector3 stp, endp;
                _drawTool.GetDrawInf(out stp, out endp);

                if (float.TryParse(str, out x))
                {
                    if (x > 0)//
                    {
                        //���ӳ��������ӳ�
                        float addX = endp.x > stp.x ? x : -x;
                        Vector3 newEndP = new Vector3(stp.x + addX, stp.y, endp.z);
                        _drawTool.DrawRect(newEndP, stp);
                        return;
                    }

                }
                //�Ƿ�ֵ
                ShowRectInf(stp, endp);
            }
        });
        input_RectY.onEndEdit.AddListener((str) =>
        {
            if (CanChangeInput())
            {
                float x;
                //��ȡ��ǰ���ĵ�
                Vector3 stp, endp;
                _drawTool.GetDrawInf(out stp, out endp);

                if (float.TryParse(str, out x))
                {
                    if (x > 0)//
                    {
                        //���ӳ��������ӳ�
                        float addX = endp.z > stp.z ? x : -x;
                        Vector3 newEndP = new Vector3(endp.x, stp.y, stp.z + addX);
                        _drawTool.DrawRect(newEndP, stp);
                        return;
                    }

                }
                //�Ƿ�ֵ
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
                        //����
                        Vector3 centerPos = (stp + endp) / 2;
                        //Բ�Ĳ��䣬��չ�뾶
                        Vector3 newStartPos = (stp - centerPos).normalized * x + new Vector3(centerPos.x,0,centerPos.z);
                        Vector3 newEndPos = (endp - centerPos).normalized * x + new Vector3(centerPos.x, 0, centerPos.z);
                        _drawTool.DrawCircle(newStartPos, newEndPos);
                        return;
                    }
                }
                //�Ƿ�ֵ
                ShowCircleInf(stp, endp);
            }
        });

        //�ƶ�
        draw_Move.onClick.AddListener(() => {
            if (CanChangeInput())
            {
                _drawTool.isMove = true;
            }
        });


        //���浱ǰ����
        draw_Save.onClick.AddListener(() => {
            if (CanChangeInput())
            {
                //���浽�ֿ�������
                SaveAreaIntoFactory();
                //ˢ�������б����
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.ViewListCall, new OpPanelparam("����༭_ˢ��", _factoryObj)));

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
                if (_drawTool._state == DrawTool.DrawState.Drawing)//����ͼ�κ�����޸Ĳ���
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

            if(_drawTool._state == DrawTool.DrawState.Drawing)//������ɵ�״̬,ֻ���أ������
            {
                inf_DRAW.SetActive(false);
                inf_p.SetActive(false);
            }
            else//��յ�ǰ��������
            {
               
                inf_DRAW.SetActive(false);
                inf_p.SetActive(false);

                ClearInputData();
            }
        }
    }

    void ClearInputData()
    {
        _drawTool.DisableDraw();//���ͼ��
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
            openTw.Kill(false);//�رյ�ʱ����Ҫ�ϸ���ɣ���ǰλ�ùر�
        openTw = transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(new Vector2(-120, 0), 1f).OnComplete(() =>
        {
            base.CloseUIPanel();
        });
    }



    public void ShowAreaSetStyle(GameObject factorObj)
    {
         _factoryObj = factorObj;
        title.text = "["+_factoryObj.name+"]";

        //����״̬����
        tg_cameraFly.isOn = false;
        tg_cameraFly.isOn = true;
        //tg_DrawRect.isOn = true;

        OpenUIPanel();
    }

    void ResetPanel()
    {
        tg_cameraFly.isOn = true;
        _factoryObj = null;
        title.text = "�ֿ�����";

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
                if(_drawTool._state == DrawTool.DrawState.Begin)//������ʾ��Ϣ
                {
                    Vector3 stp, endp;
                    _drawTool.GetDrawInf(out stp,out endp);

                    if (tg_DrawRect.isOn)//����
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

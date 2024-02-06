using DG.Tweening;
using FrameWork;
using Stored3D;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class ToolLabel : MonoBehaviour
{
    
    public string labelName; //道具名称

    public Transform labelParent_World;
    public Transform labelParent_UI;


    public bool isShow = false;
    private ToolLabelManager.ToolLabelType type;
    private Vector3 scaleBack;

    float scaleDis;

    Vector3 worldPos;
    Vector2 uiPos;
    public Camera uiCamera;
    public Camera worldCamera;

    UnityAction clickDO;

    public void InitLabel(ToolLabelManager.LabelCreateParam labelCreate)
    {

        labelName = transform.name = labelCreate.name;
        worldPos = labelCreate.pos + labelCreate.offset;
        scaleDis = labelCreate.scaleRef;
        scaleBack = Vector3.one * CommonHelper.Remap(labelCreate.scaleRef, 0, 100, 0.1f, 1);//label三维world尺寸

        clickDO = labelCreate.labelClickDo;

        //事件添加
        EventTriggerListener.Get(transform.gameObject).onClick = (go) => {
            if (clickDO != null)
                clickDO();
            Debug.Log(go.name);
            HideLabel();
        };
        EventTriggerListener.Get(transform.gameObject).onEnter = (go) =>
        {
           // tween.Pause();
            transform.localScale *= 1.2f;
        };
        EventTriggerListener.Get(transform.gameObject).onExit = (go) =>
        {
            transform.localScale /= 1.2f;
           // tween.Restart();
        };

      

        ChangeLabelType(labelCreate.labelType);


        isShow = true;
        //动效
        //if (tween != null)
        //    tween.Kill(true);
        ////tween = transform.DOScale(transform.localScale * 0.7f, 2f).SetLoops(-1);
        //tween = transform.DOMoveY(transform.position.y+0.5f, 2f).SetEase(Ease.Linear).SetLoops(-1);

    }

    //刷新
    public void RefreshLabel(ToolLabelManager.LabelCreateParam labelCreate)
    {
        //InitLabel()
    }


    public void ChangeLabelType(ToolLabelManager.ToolLabelType newType)
    {
        if (type == newType)
            ShowLabel();
        else
        {
            type = newType;
            switch (newType)
            {
                case ToolLabelManager.ToolLabelType.World:
                    //if (tween != null)
                    //    tween.Kill(true);

                    //层级
                    gameObject.layer = 0;
                    transform.SetParent(labelParent_World);
                    transform.position = worldPos;
                    transform.localScale = scaleBack;
                    break;
                case ToolLabelManager.ToolLabelType.UI:
                    //if (tween != null)
                    //    tween.Kill(true);

                    uiCamera = labelParent_UI.parent.GetComponent<Canvas>().worldCamera;
                    //父物体

                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform.SetParent(labelParent_UI);

                    //层级
                    gameObject.layer = 5;
                    break;
            }
        }
       
    }

    Tween tween;
  

    public void HideLabel()
    {
        isShow = false;
        if (tween!=null)
            tween.Pause();
        transform.localScale *= 0;
    }
    public void ShowLabel()
    {
        isShow = true;
        transform.localScale = scaleBack;
        if (tween != null)
            tween.Restart();
    }




    private void Update()
    {
        if (isShow)
        {
            if (type == ToolLabelManager.ToolLabelType.World)
            {
                transform.forward = transform.position - Camera.main.transform.position;
            }

            if (type == ToolLabelManager.ToolLabelType.UI)
            {
                float angle = Vector3.Angle(Camera.main.transform.forward, worldPos - Camera.main.transform.position);

                if (angle < 55)
                {
                    //世界坐标转屏幕坐标，转ui位置
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                    //ui坐标
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(labelParent_UI.GetComponent<RectTransform>(), screenPos, uiCamera, out uiPos);

                    transform.GetComponent<RectTransform>().anchoredPosition3D = uiPos;


                    //尺寸  距离越远越小
                    float dis = Vector3.Distance(Camera.main.transform.position, worldPos);
                    transform.localScale = Vector3.one * 0.2f * (1 - Mathf.Clamp01(CommonHelper.Remap(dis, 0, scaleDis, 0.1f, 0.5f)));
                }
                else
                {
                    transform.localScale = Vector3.zero;
                }
            }

        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
    //private void OnMouseEnter()
    //{
    //    if (type != ToolLabelType.Mouse)
    //    {
    //        return;
    //    }
    //    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())//屏蔽UI操作事件
    //    {
    //        return;
    //    }

    //    //myLabel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.1f);
    //    isShow = true;
    //}
    //private void OnMouseExit()
    //{
    //    if (type != ToolLabelType.Mouse)
    //    {
    //        return;
    //    }
    //    if (isShowStay)
    //        return;
    //    //myLabel.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.1f);
    //    //isShowStay = false;
    //    isShow = false;
    //}

    //private void Awake()
    //{
    //    isShowStay = false;
    //    isShow = false;
    //    type = ToolLabelType.Mouse;
    //    hudOffset = new Vector2(0, 50);
    //    RegisterUI();
    //}


    /// <summary>
    /// 事件响应，toollabel是否持续展示
    /// </summary>
    /// <param name="coreEvent"></param>
    //private void ShowStay(CoreEvent coreEvent)
    //{
    //    isShowStay = (bool)coreEvent.EventParam;
    //    //Vector3 v = isShowStay ? Vector3.one : Vector3.zero;
    //    //myLabel.GetComponent<RectTransform>().DOScale(v, 0.1f);

    //    isShow = (bool)coreEvent.EventParam;
    //}


    /// <summary>
    /// 事件响应，toollabel响应模式改变
    /// </summary>
    /// <param name="coreEvent"></param>
    //private void PlaneControlToTarget(CoreEvent coreEvent)
    //{
    //    if (type != ToolLabelType.PlaneControl)
    //    {
    //        return;
    //    }
    //    if (isShowStay)
    //        return;
    //    Transform target = (Transform)coreEvent.EventParam;
    //    isShow = transform == target;
    //}
   
   
    /// <summary>
    /// 生成Label
    /// </summary>
    //public void RegisterUI()
    //{
    //    if (labelTemp != null && myLabel == null && labelParent != null)
    //    {
    //        myLabel = Instantiate(labelTemp.gameObject, labelParent).GetComponent<RectTransform>();
    //        myLabel.localScale = Vector3.zero;
    //        //myLabel.Find("Text").GetComponent<Text>().text = ToolName;
    //        //myLabel.name = ToolName;
    //        myLabel.gameObject.SetActive(true);
    //    }
    //}

}


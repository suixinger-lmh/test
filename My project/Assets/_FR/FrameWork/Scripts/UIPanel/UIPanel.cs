using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stored3D;
namespace FrameWork
{
    /// <summary>
    /// UI面板基类
    /// </summary>
    public class UIPanel : MonoBehaviour
    {
        public string PanelType = string.Empty;
        public bool isActive = true;
        protected virtual void Awake()
        {
            PanelType = this.gameObject.name;
            //Debug.Log(PanelType);
            Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().RegisetPanel(PanelType,this,false);
            //FrameEntrance.Instance.UIPanel.RegisetPanel(gameObject.name, this, isActive);
        }

        protected virtual void Start()
        {
            OnTransformObject();
            OnAddListener();
        }


        /// <summary>
        /// 物体的查找
        /// </summary>
        protected virtual void OnTransformObject()
        {

        }

        /// <summary>
        /// 事件的绑定
        /// </summary>
        protected virtual void OnAddListener()
        {

        }
        protected virtual void OnDestroy()
        {
            if(Manager3DStored.Instance!=null)
                Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().UnRegisetPanel(PanelType);
            //if (FrameEntrance.Instance != null)
            //    FrameEntrance.Instance.UIPanel.UnRegisetPanel(gameObject.name);
        }
        public virtual void OpenUIPanel()
        {
            //if (isActive) return;
            transform.GetChild(0).gameObject.SetActive(true);
            isActive = true;
        }
        public virtual void CloseUIPanel()
        {
            //if (!isActive) return;

            transform.GetChild(0).gameObject.SetActive(false);
            isActive = false;
        }


        public virtual void TogglePanel(bool isOn)
        {
            if (isOn)
                OpenUIPanel();
            else
                CloseUIPanel();
        }


    }


    public class OpPanelparam
    {
        public string tag;

        /// <summary>
        /// 自定义参数数组
        /// </summary>
        public object param;


        public OpPanelparam(string tag, object param)
        {
            this.tag = tag;
            this.param = param;
        }
    }

}
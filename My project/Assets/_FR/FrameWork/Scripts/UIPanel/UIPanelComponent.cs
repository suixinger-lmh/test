using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrameWork
{
    public class UIPanelComponent:MonoBehaviour, IBaseComponent
    {

        public void InitComponent()
        {
            
        }
        //public override void Init()
        //{
        //    base.Init();
        //}
        private Dictionary<string, UIPanel> uipanels = new Dictionary<string, UIPanel>();

        public Dictionary<string, UIPanel> UIPanelDic
        {
            get
            {
                return uipanels;
            }
        }
        /// <summary>
        /// 注册面板到组件中
        /// </summary>
        /// <param name="name">面板名，唯一</param>
        /// <param name="active">默认隐藏</param>
        public void RegisetPanel(string name, UIPanel panel, bool active = false)
        {
            if (uipanels.ContainsKey(name))
            {
                Debug.LogError("存在同名面板");
                return;
            }
            else
            {
                uipanels.Add(name, panel);
            }

            if (!active)
            {
                panel.CloseUIPanel();
            }
        }
        /// <summary>
        /// 移除面板
        /// </summary>
        /// <param name="name"></param>
        public void UnRegisetPanel(string name)
        {
            if (uipanels.ContainsKey(name))
            {
                uipanels.Remove(name);
            }
            else
            {
                Debug.LogError("不存在的面板"+name);
            }
        }
        /// <summary>
        /// 获取UIPanel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">面板名</param>
        /// <returns></returns>
        public T GetUIPanel<T>(string name) where T : UIPanel
        {
            if (uipanels.ContainsKey(name))
            {
                return (T)uipanels[name];
            }
            else
            {
                Debug.LogError("不存在的面板"+ name);
                return null;
            }

        }

        /// <summary>
        /// 直接获取指定类型的面板(注意多个相同类型的面板请用带字符串的获取方式获取)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUIPanel<T>() where T:UIPanel
        {
            foreach(var panel in uipanels.Values)
            {
                if (typeof(T) == panel.GetType())
                {
                    return (T)panel;
                }
            }
            return null;
        }

        public UIPanel GetUIPanel(string name)
        {
            if (uipanels.ContainsKey(name))
            {
                return uipanels[name];
            }
            Debug.LogError("不存在的面板" + name);
            return null;
        }

    }
}
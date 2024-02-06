using UnityEngine;
using System.Collections;
using Stored3D;

namespace FrameWork
{
    /// <summary>
    /// 自定义消息事件组件
    /// </summary>
    public sealed class EventComponent : MonoBehaviour,IBaseComponent
    {

        public void InitComponent()
        {
            
        }
        //public override void Init()
        //{
        //    base.Init();
        //}

        private CoreEventDispatcher m_EventDispatcher = new CoreEventDispatcher();
        private System.Collections.Generic.Queue<CoreEvent> m_eventQueue = new System.Collections.Generic.Queue<CoreEvent>();
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventId">事件类型编号</param>
        /// <param name="handler">事件回调函数</param>
        public void AddEventListener(CoreEventId eventId, CoreEventDispatcher.EventHandler handler)
        {
            m_EventDispatcher.AddEventListener(eventId, handler);
        }
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件类型编号</param>
        /// <param name="handler">事件回调函数</param>
        public void RemoveEventListener(CoreEventId eventId, CoreEventDispatcher.EventHandler handler)
        {
            m_EventDispatcher.RemoveEventListener(eventId, handler);
        }
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId">事件类型编号</param>
        public void RemoveEventListener(CoreEventId eventId)
        {
            m_EventDispatcher.RemoveEventListener(eventId);
        }
        /// <summary>
        /// 事件分发,立即抛出非线程安全
        /// </summary>
        /// <param name="evt">数据</param>
        public void DispatchCoreEventImmediately(CoreEvent evt)
        {
            m_EventDispatcher.DispatchCoreEvent(evt);
        }
        /// <summary>
        /// 事件分发,下一帧抛出，线程安全
        /// </summary>
        /// <param name="evt">数据</param>
        public void DispatchCoreEvent(CoreEvent evt)
        {
            m_eventQueue.Enqueue(evt);
        }

        private void Update()
        {
            while (m_eventQueue.Count > 0)
            {
                m_EventDispatcher.DispatchCoreEvent(m_eventQueue.Dequeue());
            }
        }

    }
}
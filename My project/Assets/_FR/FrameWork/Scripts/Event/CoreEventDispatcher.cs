using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace FrameWork
{
    public class CoreEventDispatcher
    {
        /*消息处理委托,每一个事件id，对应一个或多个消息处理函数*/
        public delegate void EventHandler(CoreEvent evt);

        private Dictionary<CoreEventId, EventHandler> mEventHandlerPool = new Dictionary<CoreEventId, EventHandler>();
        /*消息注册函数*/
        public void AddEventListener(CoreEventId eventId, EventHandler handler)
        {
            EventHandler evtHandler = null;
            if (mEventHandlerPool.ContainsKey(eventId))
            {
                evtHandler = mEventHandlerPool[eventId];
                evtHandler += handler;
                mEventHandlerPool[eventId] = evtHandler;
            }
            else mEventHandlerPool.Add(eventId, handler);

            evtHandler = null;
        }
        /*消息移除函数*/
        public void RemoveEventListener(CoreEventId eventId, EventHandler handler)
        {
            EventHandler evtHandler = null;
            if (mEventHandlerPool.ContainsKey(eventId))
            {
                evtHandler = mEventHandlerPool[eventId];
                evtHandler -= handler;
                mEventHandlerPool[eventId] = evtHandler;
                if (evtHandler == null) mEventHandlerPool.Remove(eventId);
            }

            evtHandler = null;
        }
        /*消息移除函数*/
        public void RemoveEventListener(CoreEventId eventId)
        {
            EventHandler evtHandler = null;
            if (mEventHandlerPool.ContainsKey(eventId))
            {
                evtHandler = mEventHandlerPool[eventId];
                mEventHandlerPool.Remove(eventId);
            }
            evtHandler = null;
        }
        /*消息派发函数*/
        public void DispatchCoreEvent(CoreEvent evt)
        {
            if (evt != null && mEventHandlerPool.ContainsKey(evt.EventID))
            {
                EventHandler evtHandler = mEventHandlerPool[evt.EventID];
                if (evtHandler != null)
                {
                    evtHandler(evt);
                }
            }
        }
    }
}
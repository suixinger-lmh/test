using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace FrameWork
{
    public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
        public VoidDelegate onDrag;

        public VoidDelegate onDoublelick;
        public VoidDelegate onPointStay;



        private bool isEnter;
        private float t1, t2;//点击时间

        private void Update()
        {
            if (isEnter)
            {
                if (onPointStay != null)
                    onPointStay(gameObject);
            }
        }
        static public EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>();
            return listener;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].eventID == EventTriggerType.PointerClick)
                {
                    triggers[i].callback.Invoke(eventData);
                }
            }
            if (onClick != null) onClick(gameObject);



        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].eventID == EventTriggerType.PointerDown)
                {
                    triggers[i].callback.Invoke(eventData);
                }
            }
            if (onDown != null) onDown(gameObject);
            if (Input.GetMouseButtonDown(0))
            {
                t2 = Time.time;

                if (t2 - t1 < 0.4)
                {
                    if (onDoublelick != null)
                        onDoublelick(gameObject);
                }
                t1 = t2;
            }
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].eventID == EventTriggerType.PointerEnter)
                {
                    triggers[i].callback.Invoke(eventData);
                }
            }
            if (onEnter != null) onEnter(gameObject);

            isEnter = true;
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].eventID == EventTriggerType.PointerExit)
                {
                    triggers[i].callback.Invoke(eventData);
                }
            }
            if (onExit != null) onExit(gameObject);

            isEnter = false;
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].eventID == EventTriggerType.PointerUp)
                {
                    triggers[i].callback.Invoke(eventData);
                }
            }
            if (onUp != null) onUp(gameObject);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].eventID == EventTriggerType.Select)
                {
                    triggers[i].callback.Invoke(eventData);
                }
            }
            if (onSelect != null) onSelect(gameObject);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].eventID == EventTriggerType.UpdateSelected)
                {
                    triggers[i].callback.Invoke(eventData);
                }
            }
            if (onUpdateSelect != null) onUpdateSelect(gameObject);
        }
        public override void OnDrag(PointerEventData eventData)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].eventID == EventTriggerType.Drag)
                {
                    triggers[i].callback.Invoke(eventData);
                }
            }
            if (onDrag != null) onDrag(gameObject);
        }



    }
}
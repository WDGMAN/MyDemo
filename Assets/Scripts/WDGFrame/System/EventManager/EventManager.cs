using System;
using System.Collections.Generic;
using UnityEngine;

namespace WDGFrame
{
    public enum EventEnum
    {
        BeHurt,//受伤
    }
    public class EventManager:Singleton<EventManager>
    {
        
        private EventManager(){}
        
        private Dictionary<int, List<Delegate>> eventDic = new Dictionary<int, List<Delegate>>();

        private void AddListenerBase(EventEnum eventEnum, Delegate callback)
        {
            if (eventDic.TryGetValue((int)eventEnum ,out List<Delegate> val))
            {
                eventDic[(int)eventEnum].Add(callback);
            }
            else
            {
                val = new List<Delegate>() { callback };
                eventDic.Add((int)eventEnum,val);
            }
        }

        /// <summary>
        /// 无参事件
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="callback"></param>
        public void AddListener(EventEnum eventEnum, Action callback)
        {
            AddListenerBase(eventEnum, callback);
        }
        /// <summary>
        /// 一个参数
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public void AddListener<T>(EventEnum eventEnum, Action<T> callback)
        {
            AddListenerBase(eventEnum, callback);
        }
        
        /// <summary>
        /// 2个参数
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        public void AddListener<T1,T2>(EventEnum eventEnum, Action<T1,T2> callback)
        {
            AddListenerBase(eventEnum, callback);
        }
        
        /// <summary>
        /// 3个参数
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        public void AddListener<T1,T2,T3>(EventEnum eventEnum, Action<T1,T2,T3> callback)
        {
            AddListenerBase(eventEnum, callback);
        }
        
        /// <summary>
        /// 4个参数
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        public void AddListener<T1,T2,T3,T4>(EventEnum eventEnum, Action<T1,T2,T3,T4> callback)
        {
            AddListenerBase(eventEnum, callback);
        }

        private void RemoveListenerBase(EventEnum eventEnum,Delegate callback)
        {
            if (eventDic.TryGetValue((int)eventEnum, out List<Delegate> eventList))
            {
                eventList.Remove(callback);
                //事件列表没有事件时,从字典中移除
                if (eventList.Count == 0)
                {
                    eventDic.Remove((int)eventEnum);
                }
            }
        }

        /// <summary>
        /// 删除整个对应的事件列表
        /// </summary>
        /// <param name="eventEnum"></param>
        public void RemoveListener(EventEnum eventEnum)
        {
            if (eventDic. ContainsKey((int)eventEnum))
            {
                eventDic.Remove((int)eventEnum);
            }
            else
            {
                Debug.Log("删除空事件列表");
            }
        }
        
        public void RemoveListener(EventEnum eventEnum, Action callback)
        {
            RemoveListenerBase(eventEnum,callback);
        }
        
        /// <summary>
        /// 一个参数
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public void RemoveListener<T>(EventEnum eventEnum, Action<T> callback)
        {
            RemoveListenerBase(eventEnum,callback);
        }
        
        /// <summary>
        /// 2个参数
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="callback"></param>
        public void RemoveListener<T1,T2>(EventEnum eventEnum, Action<T1,T2> callback)
        {
            RemoveListenerBase(eventEnum,callback);
        }
        
        /// <summary>
        /// 3个参数
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        public void RemoveListener<T1,T2,T3>(EventEnum eventEnum, Action<T1,T2,T3> callback)
        {
            RemoveListenerBase(eventEnum,callback);
        }
        
        /// <summary>
        /// 4个参数
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        public void RemoveListener<T1,T2,T3,T4>(EventEnum eventEnum, Action<T1,T2,T3,T4> callback)
        {
            RemoveListenerBase(eventEnum,callback);
        }

        /// <summary>
        /// 事件触发 无参
        /// </summary>
        /// <param name="eventEnum"></param>
        public void TriggerEvent(EventEnum eventEnum)
        {
            if (eventDic.ContainsKey((int)eventEnum))
            {
                foreach (Delegate callback in eventDic[(int)eventEnum])
                {
                    (callback as Action)?.Invoke();
                }
            }
        }
        
        /// <summary>
        /// 事件触发(1个参数)
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="info"></param>
        /// <typeparam name="T"></typeparam>
        public void TriggerEvent<T>(EventEnum eventEnum,T info)
        {
            if (eventDic.ContainsKey((int)eventEnum))
            {
                foreach (Delegate callback in eventDic[(int)eventEnum])
                {
                    (callback as Action<T>)?.Invoke(info);
                }
            }
        }
        
        /// <summary>
        /// 事件触发(2个参数)
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="info1"></param>
        /// <param name="info2"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        public void TriggerEvent<T1,T2>(EventEnum eventEnum,T1 info1,T2 info2)
        {
            if (eventDic.ContainsKey((int)eventEnum))
            {
                foreach (Delegate callback in eventDic[(int)eventEnum])
                {
                    (callback as Action<T1,T2>)?.Invoke(info1,info2);
                }
            }
        }
        
        /// <summary>
        /// 事件触发(3个参数)
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="info1"></param>
        /// <param name="info2"></param>
        /// <param name="info3"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        public void TriggerEvent<T1,T2,T3>(EventEnum eventEnum,T1 info1,T2 info2,T3 info3)
        {
            if (eventDic.ContainsKey((int)eventEnum))
            {
                foreach (Delegate callback in eventDic[(int)eventEnum])
                {
                    (callback as Action<T1,T2,T3>)?.Invoke(info1,info2,info3);
                }
            }
        }
        
        /// <summary>
        /// 事件触发(4个参数)
        /// </summary>
        /// <param name="eventEnum"></param>
        /// <param name="info1"></param>
        /// <param name="info2"></param>
        /// <param name="info3"></param>
        /// <param name="info4"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        public void TriggerEvent<T1,T2,T3,T4>(EventEnum eventEnum,T1 info1,T2 info2,T3 info3,T4 info4)
        {
            if (eventDic.ContainsKey((int)eventEnum))
            {
                foreach (Delegate callback in eventDic[(int)eventEnum])
                {
                    (callback as Action<T1,T2,T3,T4>)?.Invoke(info1,info2,info3,info4);
                }
            }
        }

        /// <summary>
        /// 清空事件（慎用！！！）
        /// </summary>
        public void Clear()
        {
            eventDic.Clear();
        }
        
        
        
    }
}
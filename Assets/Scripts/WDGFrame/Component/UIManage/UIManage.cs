using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WDGFrame
{
    public enum UILayer
    {
        Bot,
        Mid,
        Top,
        System,
    }

    public class UIManage:Singleton<UIManage>
    {
        private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

      

        //记录我们UI的Canvas父对象 方便以后外部可能会使用它
        public RectTransform canvas;

        private  UIManage()
        {
            //创建canvas
            GameObject obj = Resources.Load<GameObject>("UI/Canvas");
           obj= GameObject.Instantiate(obj);
            canvas = obj.transform as RectTransform;
            GameObject.DontDestroyOnLoad(obj);

         

            // obj = Resources.Load<GameObject>("UI/EventSystem"); //load资源未定义
            // GameObject.DontDestroyOnLoad(obj);
        }

        /// <summary>
        /// 通过层级枚举 得到对应层级的父对象
        /// </summary>
        /// <returns></returns>
        public Transform GetLayerFather(UILayer layer)
        {
            switch (layer)
            {
            }

            return null;
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        /// <param name="callBack">当面板预设体创建成功后 你想做的事</param>
        public void ShowPanel<T>( UnityAction<T> callBack = null)
            where T : BasePanel
        {
            string panelName = typeof(T).ToString();
            if (panelDic.TryGetValue(panelName, out BasePanel basePanel))
            {
                panelDic[panelName].Show();
                if (callBack != null)
                {
                    callBack((panelDic[panelName]) as T);
                }
            }
            else
            {
                //创建UI
                GameObject panelObj = Resources.Load<GameObject>("UI/" + panelName);
                panelObj=   GameObject.Instantiate(panelObj);
              
                //设置父对象 设置相对位置和大小
                panelObj.transform.SetParent(canvas);
                panelObj.transform.localPosition=Vector3.zero;
                panelObj.transform.localScale=Vector3.one;
                (panelObj.transform as RectTransform).offsetMax = Vector2.zero;
                (panelObj.transform as RectTransform).offsetMin = Vector2.zero;
                //添加脚本
             T panel=  panelObj.AddComponent<T>();
                if (callBack != null)
                {
                    callBack(panel);
                }
                panel.Show();
                //最后存起来
                panelDic.Add(panelName,panel);
            }
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        public void HidePanel<T>()
        {
            string panelName = typeof(T).ToString();
            if (panelDic.TryGetValue(panelName, out BasePanel basePanel))
            {
                panelDic[panelName].Hide();
                GameObject.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }
        }

        /// <summary>
        /// 得到一个已经显示的面板 方便外部使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPanel<T>() where T : BasePanel
        {
            string panelName = typeof(T).ToString();
            if (panelDic.TryGetValue(panelName, out BasePanel basePanel))
            {
                return basePanel as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 给控件添加自定义事件监听
        /// </summary>
        /// <param name="control"></param>
        /// <param name="type"></param>
        /// <param name="callBack"></param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type,
            UnityAction<BaseEventData> callBack)
        {
            EventTrigger trigger = control.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = control.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(callBack);
            trigger.triggers.Add(entry);
        }
        
        
        
    }
}
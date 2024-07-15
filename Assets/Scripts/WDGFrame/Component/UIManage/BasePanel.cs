
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WDGFrame
{
    public class BasePanel : MonoBehaviour
    {
        private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

        protected virtual void Awake()
        {
            FindChildrenControl<Button>();
            FindChildrenControl<Image>();
            FindChildrenControl<Text>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<Slider>();
            FindChildrenControl<ScrollRect>();
            FindChildrenControl<InputField>();
             
                
        }
        

        /// <summary>
        /// 显示
        /// </summary>
        public virtual void Show()
        {
            
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void Hide()
        {
            
        }

        /// <summary>
        /// 点击按钮
        /// </summary>
        /// <param name="btnName"></param>
        protected virtual void OnClick(string btnName)
        {
            
        }

        /// <summary>
        /// value改变
        /// </summary>
        protected virtual void OnValueChanged(string toggleName, bool value)
        {
            
        }

        /// <summary>
        /// 得到对应名字的对应控件脚本
        /// </summary>
        /// <param name="controlName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            
            if (controlDic.ContainsKey(controlName))
            {
                for (int i = 0; i < controlDic[controlName].Count; i++)
                {
                    if (controlDic[controlName][i] is T)
                    {
                        return controlDic[controlName][i] as T;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 找到子对象的对应控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void FindChildrenControl<T>() where T : UIBehaviour
        {
            T[] controls = this.GetComponentsInChildren<T>();

            for (int i = 0; i < controls.Length; i++)
            {
                string objName = controls[i].gameObject.name;
                if (controlDic.ContainsKey(objName))
                {
                    controlDic[objName].Add(controls[i]);
                }
                else
                {
                    controlDic.Add(objName,new List<UIBehaviour>(){controls[i]});
                }
                //如果是按钮控件
                if (controls[i] is Button)
                {
                    (controls[i] as Button).onClick.AddListener(() =>
                    {
                        OnClick(objName);
                    });
                }
                else if (controls[i] is Toggle)
                {
                    (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                    {
                        OnValueChanged(objName,value);
                    });
                }
                
                
            }
        }
        

    }

}


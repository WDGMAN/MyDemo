using System;
using UnityEngine;

namespace WDGFrame
{
    public class SingletonAutoMono<T>:MonoBehaviour where T:MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
}
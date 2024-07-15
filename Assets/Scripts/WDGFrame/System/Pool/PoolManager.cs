using System.Collections.Generic;
using UnityEngine;

namespace WDGFrame
{
    
    public class PoolManager:Singleton<PoolManager>
    {
        private Dictionary<string, PoolGameObject> poolObjectsDic = new Dictionary<string, PoolGameObject>();

        private PoolManager()
        {
            
        }

        public void PushGameObject(GameObject obj,string keyName)
        {
            //查找是否有
            if (poolObjectsDic.TryGetValue(keyName, out PoolGameObject poolGameObject))
            {
                //有的话就加进去
                poolGameObject.Push(obj);
            }
            else
            {
                PoolGameObject poolGameObjectTemp = new PoolGameObject();
                poolGameObjectTemp.Push(obj);
                //没有的话创建
                poolObjectsDic.Add(keyName,poolGameObjectTemp);
            }
            
        }

        public GameObject GetGameObject(string keyName)
        {
            if (poolObjectsDic.TryGetValue(keyName, out PoolGameObject poolGameObject))
            {
                return poolGameObject.Get();
            }
            else
            {
                return null;
            }
        }
        
        
    }
}
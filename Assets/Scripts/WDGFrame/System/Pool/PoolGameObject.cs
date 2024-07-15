
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolGameObject
{
    private Queue<GameObject> gameObjectsQueue = new Queue<GameObject>();

    private int maxCount=100;//最大数量
    public void Push(GameObject gameObject)
    {
        if(gameObjectsQueue.Count>=maxCount)return;
        gameObject.SetActive(false);
        gameObjectsQueue.Enqueue(gameObject);
    }

    public GameObject Get()
    {
        if (gameObjectsQueue.Count == 0) return null;
        GameObject gameObject=gameObjectsQueue.Dequeue();
        gameObject.SetActive(true);
        return gameObject;
    }

    public void Clear()
    {
        //先销毁
        foreach (GameObject item in gameObjectsQueue)
        {
            GameObject.Destroy(item);
        }
        //再清空
        gameObjectsQueue.Clear();
    }
    
}
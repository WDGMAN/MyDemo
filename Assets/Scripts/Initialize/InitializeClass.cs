
using Cinemachine;
using UnityEditor;
using UnityEngine;
using WDGFrame;


    public static class InitializeClass
    {
        static InitializeClass()
        {
           
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
        
            CameraManage.Instance.Init();
            
            //初始化场景
            SceneManage.Instance.LoadScene("MainMenu", () =>
            {
                UIManage.Instance.ShowPanel<MainMenu>();
            });
        }
        
        
    }



    using System;
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using WDGFrame;

    public class SceneManage:SingletonAutoMono<SceneManage>
    {
        
        /// <summary>
        /// 加载场景
        /// </summary>
        public void LoadScene(string name,Action callBack)
        {
            StartCoroutine(AwaitLoading(name,callBack));
        }

        IEnumerator AwaitLoadScene(string name,Action callBack)
        {

            GameObject obj = Resources.Load<GameObject>("UI/Loading");
           obj= Instantiate(obj);
            Slider loadingSlider = obj.transform.Find("BackGround/Slider").GetComponent<Slider>();
            TextMeshProUGUI loadingText =
                obj.transform.Find("BackGround/Slider/loadingText").GetComponent<TextMeshProUGUI>();
            AsyncOperation op = SceneManager.LoadSceneAsync(name);
            op.allowSceneActivation = false;

            bool flag = false;
            while (!op.isDone)
            {
                loadingSlider.value= op.progress;
                loadingText.text = "进度加载中 " +  op.progress*100+"%";
                if (op.progress >= 0.9f)
                {
                    flag = true;
                    op.allowSceneActivation = true;
                }
                yield return null;
                if (flag)
                {
                    callBack();
                }
            }

        }

        IEnumerator AwaitLoading(string name,Action callBack)
        {

            SceneManager.LoadScene("Loading");

            yield return null;
            StartCoroutine(AwaitLoadScene(name,callBack));
        }
    }

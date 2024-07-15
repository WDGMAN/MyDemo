
    using System.Collections;
    using Animancer;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using WDGFrame;

    public class MainMenu:BasePanel
    {

        private GameObject startClick;
        
        protected override void Awake()
        {
            base.Awake();
            startClick = transform.Find("BackGround/Start").gameObject;
            startClick.AddComponent<Button>().onClick.AddListener(() =>
            {
                UIManage.Instance.HidePanel<MainMenu>();
              SceneManage.Instance.LoadScene("Game", () =>
              {
                  //加载角色
                  GameObject player = Resources.Load<GameObject>("GameObject/Player/Player");
                  player=  Instantiate(player);
            
                  player.AddComponent<PlayerController>();
                 
    
                 
                 //加载血条
                 UIManage.Instance.ShowPanel<HealthBar>();

              });
              
            });
        }

    
        private void Init()
        {
            
        }
    }

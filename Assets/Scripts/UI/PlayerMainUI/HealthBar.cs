
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using WDGFrame;

    public class HealthBar:BasePanel
    {

        private GameObject backGround;
        private GameObject healthBar;
        private TextMeshProUGUI text;


        private Action<float> eventAction;

        protected override void Awake()
        {
            backGround = transform.Find("BackGround").gameObject;
            backGround.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Png/Health");
            backGround.GetComponent<Image>().color=Color.black;//临时
            backGround.GetComponent<Image>().type = Image.Type.Filled;
            backGround.GetComponent<Image>().fillMethod=Image.FillMethod.Horizontal;
            backGround.GetComponent<Image>().fillOrigin=(int)Image.OriginHorizontal.Left;
            healthBar = transform.Find("Health").gameObject;
            healthBar.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Png/Health");
            healthBar.GetComponent<Image>().type=Image.Type.Filled;
            healthBar.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
            healthBar.GetComponent<Image>().fillOrigin=(int)Image.OriginHorizontal.Left;

            
            text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            
            //更改left top位置
            RectTransform healthBarTransform = GetComponent<RectTransform>();
            healthBarTransform.offsetMax = new Vector2(959, -960);
            //更改right bottom
            healthBarTransform.offsetMin = new Vector2(-962, 118);

            backGround.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 400);
            healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 400);

            eventAction = (float number) =>
            {
                updateHealthBar(number);
            };
            //添加事件
          EventManager.Instance.AddListener(EventEnum.BeHurt, eventAction);
        }

        private void updateHealthBar(float number)
        {
            number = Mathf.Round(number);
            // text.text = PlayerController.Instance.Attribute.HealthMax+"/" + number;
            // healthBar.GetComponent<Image>().fillAmount = number/PlayerController.Instance.Attribute.HealthMax  ;
        }

        public override void Hide()
        {
            base.Hide();
            EventManager.Instance.RemoveListener(EventEnum.BeHurt,eventAction);
        }

      
    }

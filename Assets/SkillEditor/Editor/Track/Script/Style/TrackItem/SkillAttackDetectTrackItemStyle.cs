using UnityEngine.UIElements;
using UnityEditor;


public class SkillAttackDetectTrackItemStyle:SkillTrackItemStyleBase
    {
        private const string trackItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/TrackItem/AnimationTrackItem.uxml";
        private Label titleLabel;
        public VisualElement mainDragArea { get; private set; }
        public VisualElement animationOverLine{ get; private set; }

        public void Init(SkillTrackStyleBase trackStyle)
        {
            titleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate()
                .Query<Label>();
            root = titleLabel;
            mainDragArea = root.Q<VisualElement>("Main");
            animationOverLine = root.Q<VisualElement>("OverLine");
            trackStyle.AddItem(root);
         
        }

   

      


        public virtual void SetTitle(string title)
        {
            titleLabel.text = title;
        }

      
       
       
       
    }

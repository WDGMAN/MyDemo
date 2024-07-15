using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;


public class SkillEditorWindow : EditorWindow
{
    public static SkillEditorWindow Instance;
    [MenuItem("SkillEditor/SkillEditorWindow")]
    public static void ShowExample()
    {
        SkillEditorWindow wnd = GetWindow<SkillEditorWindow>();
        wnd.titleContent = new GUIContent("技能编辑器");
    }

    private VisualElement root;
    public VisualElement Root => root;
    public void CreateGUI()
    {
        if(Application.isPlaying)return;
        SkillConfig.SetValidateAction(RestView);
        Instance = this;
        // Each editor window contains a root VisualElement object
         root = rootVisualElement;

     

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SkillEditor/Editor/EditorWindow/SkillEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        InitTopMenu();
        InitTimerShaft();
        InitConsole();
        InitContent();
        if (skillConfig != null)
        {
            SkillConfigObjectField.value = skillConfig;
            CurrentFrameCount = skillConfig.FrameCount;
        }
        else
        {
            CurrentFrameCount = 100;
        }

        if (currentPreviewCharacterObj != null)
        {
            PreviewCharacterPrefabObjectField.value = currentPreviewCharacterPrefab;
        }

        if (currentPreviewCharacterObj != null)
        {
            PreviewCharacterObjectField.value = currentPreviewCharacterObj;
        }
        CurrentSelectFrameIndex = 0;

    }

    private void RestView()
    {
     
        SkillConfig tempSkillConfig = skillConfig;
        SkillConfigObjectField.value = null;
        SkillConfigObjectField.value = tempSkillConfig;
    }
    // //窗口销毁会调用 但是直接关闭Unity不会调用
    // private void OnDestroy()
    // {
    // }

    private void OnDisable()
    {
        if(skillConfig!=null)SaveConfig();

    }

    #region TopMenu

    private const string skillEditorScenePath="Assets/SkillEditor/SkillEditorScene.unity";
    private const string PreviewCharacterRootPath = "PreviewCharacterRoot";
    private string oldScenePath;
    private Button LoadEditorSceneButton;
    private Button LoadOldSceneButton;
    private Button SkillBasicButton;

    private ObjectField PreviewCharacterPrefabObjectField;
    private ObjectField PreviewCharacterObjectField;
    
    private ObjectField SkillConfigObjectField;
    private GameObject currentPreviewCharacterPrefab;
    private GameObject currentPreviewCharacterObj;
    public GameObject PreviewCharacterObj
    {
        get => currentPreviewCharacterObj;
    }

    private void InitTopMenu()
    {
        LoadEditorSceneButton = root.Q<Button>(nameof(LoadEditorSceneButton));
        LoadEditorSceneButton.clicked += LoadEditorSceneButtonClick;
        LoadOldSceneButton = root.Q<Button>(nameof(LoadOldSceneButton));
        LoadOldSceneButton.clicked += LoadOldSceneButtonClick;
        SkillBasicButton = root.Q<Button>(nameof(SkillBasicButton));
        SkillBasicButton.clicked += SkillBasicButtonClick;

        PreviewCharacterPrefabObjectField = root.Q<ObjectField>(nameof(PreviewCharacterPrefabObjectField));
        PreviewCharacterPrefabObjectField.RegisterValueChangedCallback(PreviewCharacterPrefabObjectFieldChanged);
      
        PreviewCharacterObjectField = root.Q<ObjectField>(nameof(PreviewCharacterObjectField));
        PreviewCharacterObjectField.RegisterValueChangedCallback(PreviewCharacterObjectFieldChanged);

        
        SkillConfigObjectField = root.Q<ObjectField>(nameof(SkillConfigObjectField));
        SkillConfigObjectField.RegisterValueChangedCallback(SkillConfigObjectFieldChanged);

    }

    //角色预览对象修改
    private void PreviewCharacterObjectFieldChanged(ChangeEvent<Object> evt)
    {
        currentPreviewCharacterObj = (GameObject)evt.newValue;
    }

    //角色预制体修改
    private void PreviewCharacterPrefabObjectFieldChanged(ChangeEvent<Object> evt)
    {
        
        //避免在其他场景实例化
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        if (currentScenePath != skillEditorScenePath)
        {
            PreviewCharacterPrefabObjectField.value = null;
            return;
        }
        
        //值相等,设置无效
        if (evt.newValue == currentPreviewCharacterPrefab) return;
        
        currentPreviewCharacterPrefab = (GameObject)evt.newValue ;
        
        //销毁旧的
        if(currentPreviewCharacterObj!=null)DestroyImmediate(currentPreviewCharacterObj);

        Transform parent = GameObject.Find(PreviewCharacterRootPath).transform;
        if (parent != null && parent.childCount > 0)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }
        //实例化新的
        if (evt.newValue != null)
        {
            currentPreviewCharacterObj = Instantiate(evt.newValue as GameObject,Vector3.zero, Quaternion.identity,parent);
            PreviewCharacterObjectField.value = currentPreviewCharacterObj;
        }
        
    }
    //技能配置修改

    private void SkillConfigObjectFieldChanged(ChangeEvent<Object> evt)
    {
        skillConfig=evt.newValue as SkillConfig;
        //刷新轨道
        ResetTrack();

        CurrentSelectFrameIndex = 0;
        if (skillConfig == null)
        {
            CurrentFrameCount = 100;
            return;
        }
        CurrentFrameCount = skillConfig.FrameCount;
    }
  

    //加载编辑器场景
    private void LoadEditorSceneButtonClick()
    {
       string  currentPath = EditorSceneManager.GetActiveScene().path;
       if (currentPath != skillEditorScenePath)  //相同场景无意义
       {
           oldScenePath = currentPath;
           EditorSceneManager.OpenScene(skillEditorScenePath);
           
       }
    }

    //回归旧场景
    private void LoadOldSceneButtonClick()
    {
        if (!string.IsNullOrEmpty(oldScenePath))
        {
            string  currentPath = EditorSceneManager.GetActiveScene().path;
            if (currentPath != oldScenePath)
            {
                EditorSceneManager.OpenScene(oldScenePath);
            }

        }
    }
    //查看技能基本信息
    private void SkillBasicButtonClick()
    {
        if (skillConfig != null)
        {
            Selection.activeObject = skillConfig; //当前选择对象等于XX
        }
    }

    #endregion

    #region TimerShaft

    private IMGUIContainer timerShaft;
    private IMGUIContainer selectLine;
    private VisualElement contentContainer;
    private VisualElement contentViewPort;
    private int currentFrameCount;

    public int CurrentFrameCount
    {
        get => currentFrameCount;
        set
        {
            currentFrameCount = value;
            //同步给SkillConfig
            FrameCountField.value = currentFrameCount;
            if (skillConfig != null)
            {
                skillConfig.FrameCount = currentFrameCount;
                SaveConfig();
            }
            //Content区域尺寸的变化
            UpdateContentSize();
        }
    }

    private int currentSelectFrameIndex=-1;

    public int CurrentSelectFrameIndex
    {
        get => currentSelectFrameIndex;
       private set
        {
            int old = currentSelectFrameIndex;
            //如果超出范围 更新最大帧率
            if (value > CurrentFrameCount) CurrentFrameCount = value;
            currentSelectFrameIndex = Mathf.Clamp(value,0,CurrentFrameCount) ;
            CurrentFrameField.value = currentSelectFrameIndex;
            if (old != currentSelectFrameIndex)
            {
                UpdateTimerShaftView();
                TickSkill();
            }
            
          
            
     
        }
    }
    //当前内容区域的偏移坐标
    private float contentOffsetPos
    {
        get => Mathf.Abs(contentContainer.transform.position.x) ;
    }  
    //当前选中
    private float currentSelectFramePos
    {
        get => CurrentSelectFrameIndex*skillEditorConfig.framUnitWidth ;
    }

    private bool timerShaftMouseIsMouseEnter = false;
    private void InitTimerShaft()
    {
        timerShaft = root.Q<IMGUIContainer>("TimerShaft");
        ScrollView  MainContentView = root.Q<ScrollView>("MainContentView");
        contentContainer = MainContentView.Q<VisualElement>("unity-content-container");
        contentViewPort = MainContentView.Q<VisualElement>("unity-content-viewport");
        timerShaft.onGUIHandler = DrawTimerShaft;
        
        selectLine = root.Q<IMGUIContainer>("SelectLine");
        selectLine.onGUIHandler = DrawSelectLine;

        timerShaft.RegisterCallback<WheelEvent>(TimerShaftWheel);
        timerShaft.RegisterCallback<MouseDownEvent>(TimerShaftMouseDown);
        timerShaft.RegisterCallback<MouseMoveEvent>(TimerShaftMouseMove);
        timerShaft.RegisterCallback<MouseUpEvent>(TimerShaftMouseUp);
        timerShaft.RegisterCallback<MouseOutEvent>(TimerShaftMouseOut);
        
    }

    private void TimerShaftMouseOut(MouseOutEvent evt)
    {
        timerShaftMouseIsMouseEnter = false;

    }

  

    private void TimerShaftMouseUp(MouseUpEvent evt)
    {
        timerShaftMouseIsMouseEnter = false;
    }

    private void TimerShaftMouseMove(MouseMoveEvent evt)
    {
        if (timerShaftMouseIsMouseEnter)
        {
            int newValue= GetFrameIndexByMousePos(evt.localMousePosition.x);
            if (newValue != CurrentSelectFrameIndex)
            {
                CurrentSelectFrameIndex = newValue;
            }
        }
    }

    private void TimerShaftMouseDown(MouseDownEvent evt)
    {
        //让选中线的位置卡在帧的位置上
        timerShaftMouseIsMouseEnter = true;
        IsPlaying = false;
        int newValue= GetFrameIndexByMousePos(evt.localMousePosition.x);
        if (newValue != CurrentSelectFrameIndex)
        {
            CurrentSelectFrameIndex = newValue;
        }
    }

    /// <summary>
    /// 根据鼠标坐标获取帧坐标
    /// </summary>
    /// <returns></returns>
    private int GetFrameIndexByMousePos(float x)
    {
        return GetFrameIndexByPos(x + contentOffsetPos);
    }

    public int GetFrameIndexByPos(float x)
    {
        return Mathf.RoundToInt(x / skillEditorConfig.framUnitWidth);
    }
    private void DrawSelectLine()
    {
        if (currentSelectFramePos>=contentOffsetPos)
        {
            //判断当前选中帧是否在视图范围内
            Handles.BeginGUI();
            Handles.color = Color.white;
            float x = currentSelectFramePos - contentOffsetPos;
            Handles.DrawLine(new Vector3(x ,0),new Vector3(x,contentViewPort.contentRect.height+timerShaft.contentRect.height));
            Handles.EndGUI();
        }
        
    
    }

    private void TimerShaftWheel(WheelEvent evt)
    {
        int delta = (int)evt.delta.y;
        skillEditorConfig.framUnitWidth = Mathf.Clamp(skillEditorConfig.framUnitWidth - delta,
            SkillEditorConfig.standFramUnitWidth,
            SkillEditorConfig.maxFramUnitWidthLV * SkillEditorConfig.standFramUnitWidth);
        UpdateTimerShaftView();
        UpdateContentSize();
        ResetTrack();

    }

    private void UpdateTimerShaftView()
    {
   
        timerShaft.MarkDirtyLayout();
        selectLine.MarkDirtyLayout();
    }
    private void DrawTimerShaft()
    {

        Handles.BeginGUI();
        Handles.color=Color.white;
        Rect rect = timerShaft.contentRect;
        //起始索引
        int index = Mathf.CeilToInt(contentOffsetPos/skillEditorConfig.framUnitWidth);
        //计算绘制起点的偏移
        float startOffset = 0;
        if (index > 0)
        {
            startOffset = skillEditorConfig.framUnitWidth - (contentOffsetPos % skillEditorConfig.framUnitWidth);

        }
        int tickStep = SkillEditorConfig.maxFramUnitWidthLV+1-(skillEditorConfig.framUnitWidth/SkillEditorConfig.standFramUnitWidth);//刻度步长
        tickStep = tickStep / 2;
        if (tickStep == 0) tickStep = 1;
        for (float i = startOffset; i < rect.width; i+=skillEditorConfig.framUnitWidth)
        {
            //绘制长线条、文本
            if (index % tickStep == 0)
            {
                Handles.DrawLine(new Vector3(i,rect.height-10),new Vector3(i,rect.height));
                string IndexStr = index.ToString();
                GUI.Label(new Rect(i-IndexStr.Length*4.5f,0,35,20),IndexStr);
            }
            else
            {
                Handles.DrawLine(new Vector3(i,rect.height-3),new Vector3(i,rect.height));

            }
            index += 1;
        }
        
        Handles.EndGUI();
    }

   
    #endregion

    #region Console

    private Button PreviouFrameButton;
    private Button PlayButton;
    private Button NextFrameButton;

    private IntegerField CurrentFrameField;
    private IntegerField FrameCountField;

    private void InitConsole()
    {
        PreviouFrameButton = root.Q<Button>(nameof(PreviouFrameButton));
        PlayButton = root.Q<Button>(nameof(PlayButton));
        NextFrameButton = root.Q<Button>(nameof(NextFrameButton));

        CurrentFrameField = root.Q<IntegerField>(nameof(CurrentFrameField));
        FrameCountField = root.Q<IntegerField>(nameof(FrameCountField));

        PreviouFrameButton.clicked += PreviouFrameButtonClick;
        PlayButton.clicked += PlayButtonClick;
        NextFrameButton.clicked += NextFrameButtonClick;
        
        CurrentFrameField.RegisterValueChangedCallback(CurrentFrameFieldValueChanged);
        FrameCountField.RegisterValueChangedCallback(FrameCountFieldValueChanged);
    }

    private void FrameCountFieldValueChanged(ChangeEvent<int> evt)
    {
        if(CurrentFrameCount!=evt.newValue)CurrentFrameCount = evt.newValue ;
    }

    private void CurrentFrameFieldValueChanged(ChangeEvent<int> evt)
    {
        if(CurrentSelectFrameIndex!=evt.newValue)CurrentSelectFrameIndex = evt.newValue ;
    }


    private void NextFrameButtonClick()
    {
        IsPlaying = false;
        CurrentSelectFrameIndex+=1;

    }

    private void PlayButtonClick()
    {
        IsPlaying = !IsPlaying;
    }

    private void PreviouFrameButtonClick()
    {
        IsPlaying = false;
        CurrentSelectFrameIndex-=1;

    }

    #endregion

    #region Config

    private SkillConfig skillConfig;
    public SkillConfig SkillConfig
    {
        get => skillConfig;
    }
    private SkillEditorConfig skillEditorConfig = new SkillEditorConfig();

    public void SaveConfig()
    {
        if (skillConfig != null)
        {
            EditorUtility.SetDirty(skillConfig);//先标记
            AssetDatabase.SaveAssetIfDirty(skillConfig);//再保存
            ResetTrackData();
        }
    }

    private void ResetTrackData()
    {
        for (int i = 0; i < trackList.Count; i++)
        {
            trackList[i].OnConfigChanged();
        }
    }
    #endregion

    
    #region Track

    private VisualElement trackMenuParent;
    private VisualElement contentListView;
    private ScrollView mainContentView;
    private List<SkillTrackBase> trackList = new List<SkillTrackBase>(); 
    
    private void InitContent()
    {
        contentListView = root.Q<VisualElement>("ContentListView");
        trackMenuParent = root.Q<VisualElement>("TrackMenuList");
        mainContentView = root.Q<ScrollView>("MainContentView");
        mainContentView.verticalScroller.valueChanged += ContentVerticalScrollerValueChanged;
        UpdateContentSize();
        InitTrack();
    }

    private void ContentVerticalScrollerValueChanged(float obj)
    {
        Vector3 pos = trackMenuParent.transform.position;
        pos.y = contentContainer.transform.position.y;
        trackMenuParent.transform.position = pos;
    }

    private void InitTrack()
    {
        //如果没有配置,也不需要初始化轨道
        if(skillConfig==null)return;
        InitAnimationTrack();
        InitAudioTrack();
        InitEffectTrack();
        InitAttackDetectTrack();
        InitMoveTrack();
    }
    private void InitAnimationTrack()
    {
        AnimationTrack animationTrack = new AnimationTrack();
        animationTrack.Init(trackMenuParent,contentListView,skillEditorConfig.framUnitWidth);
        trackList.Add(animationTrack);
        getPositionForRootMotion = animationTrack.GetPositionForRootMotion;
    }

    private void InitAudioTrack()
    {
        AudioTrack audioTrack = new AudioTrack();
        audioTrack.Init(trackMenuParent,contentListView,skillEditorConfig.framUnitWidth);
        trackList.Add(audioTrack);

    }

    private void InitEffectTrack()
    {
        EffectTrack effectTrack = new EffectTrack();
        effectTrack.Init(trackMenuParent,contentListView,skillEditorConfig.framUnitWidth);
        trackList.Add(effectTrack);
    }

    private void InitAttackDetectTrack()
    {
        AttackDetectTrack attackDetectTrack = new AttackDetectTrack();
        attackDetectTrack.Init(trackMenuParent,contentListView,skillEditorConfig.framUnitWidth);
        trackList.Add(attackDetectTrack);
    }

    private void InitMoveTrack()
    {
        MoveTrack moveTrack = new MoveTrack();
        moveTrack.Init(trackMenuParent,contentListView,skillEditorConfig.framUnitWidth);
        trackList.Add(moveTrack);
    }
    private void ResetTrack()
    {
        //如果配置文件是null,清理掉所有的轨道
        if (skillConfig == null)
        {
            DestoryTrack();
        }
        else
        {
            //若轨道列表里面没有数据,说明没有轨道,当前用户是有配置的，需要初始化轨道
            if (trackList.Count == 0)
            {
                InitTrack();
            }

            //更新视图
            for (int i = 0; i < trackList.Count; i++)
            {
                trackList[i].ResetView(skillEditorConfig.framUnitWidth);   
            }
            
        }
       
    }

    private void DestoryTrack()
    {
        for (int i = 0; i < trackList.Count; i++)
        {
            trackList[i].Destory();   
        }
        trackList.Clear();
    }
    private void UpdateContentSize()
    {
        contentListView.style.width = skillEditorConfig.framUnitWidth*CurrentFrameCount;
    }


    public void ShowTrackItemOnInspector(TrackItemBase trackItem,SkillTrackBase skillTrackBase)
    {
        SkillEditorInspector.SetTrackItem(trackItem,skillTrackBase);
        Selection.activeObject = this;
    }
    #endregion

    #region Preview

    private bool isPlaying;

    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            isPlaying = value;
            if (isPlaying)
            {
                startTime = DateTime.Now;
                startFrameIndex = currentSelectFrameIndex;
                
                //OnPlay
                for (int i = 0; i < trackList.Count; i++)
                {
                    trackList[i].OnPlay(currentSelectFrameIndex);
                }
            }
            else
            {
                //OnSTOP
                for (int i = 0; i < trackList.Count; i++)
                {
                    trackList[i].OnStop();
                }
            }
        }
    }

    private DateTime startTime;
    private int startFrameIndex;
    private void Update()
    {
        if (IsPlaying)
        {
            //得到时间差
            float time = (float)DateTime.Now.Subtract(startTime).TotalSeconds;
            //确定时间轴的帧率
            float frameRote;
            if (skillConfig != null) frameRote = skillConfig.FrameRote;
            else frameRote = skillEditorConfig.defaultFrameRote;
            //根据时间差计算当前的选中帧
            CurrentSelectFrameIndex = (int)(time * frameRote + startFrameIndex);
            //到达最后一帧自动暂停
            if (CurrentSelectFrameIndex == CurrentFrameCount)
            {
                IsPlaying = false;
            }
        }

    }

    private void TickSkill()
    {
        //驱动技能表现
        if (skillConfig != null && currentPreviewCharacterObj != null)
        {
            for (int i = 0; i < trackList.Count; i++)
            {
                trackList[i].TickView(currentSelectFrameIndex);
            }
                
        }
    }

    private Func<int,bool, Vector3> getPositionForRootMotion;

    public Vector3 GetPositionForRootMotion(int frameIndex) =>
        getPositionForRootMotion(frameIndex,true);

    #endregion

}

public class SkillEditorConfig
{
    public const int standFramUnitWidth = 10;//标准每帧相差像素
    public const int maxFramUnitWidthLV = 10;//最大当前每帧像素差距倍数

    public int framUnitWidth = 10;//当前每帧像素差距
    public float defaultFrameRote = 10;//默认帧率
}
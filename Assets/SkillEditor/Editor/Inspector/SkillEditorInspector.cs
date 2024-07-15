using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


[CustomEditor(typeof(SkillEditorWindow))]
public class SkillEditorInspector :Editor
{
    private VisualElement root;
    public static SkillEditorInspector Instance;
    private static TrackItemBase currentTrackItem;
    private static SkillTrackBase currentTrack;
    public static void SetTrackItem(TrackItemBase trackItem,SkillTrackBase track)
    {
        if (currentTrackItem != null)
        {
            currentTrackItem.OnUnSelect();
        }
        currentTrackItem = trackItem;
        currentTrackItem.OnSelect();
        currentTrack = track;
        //避免已经打开了Inspector,不刷新数据
        if (Instance != null) Instance.Show();
        
    }

    private void OnDestroy()
    {
        //说明窗口卸载
        if (currentTrackItem != null)
        {
            currentTrackItem.OnUnSelect();
            currentTrackItem = null;
            currentTrack = null;
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        Instance = this;
        root = new VisualElement();
       Show();
        return root;
    }

    private void Show()
    {
        Clear();
        if (currentTrackItem == null) return;
        Type itemType = currentTrackItem.GetType();
        if (itemType == typeof(AnimationTrackItem))
        {
            DrawAnimationTrackItem(currentTrackItem as AnimationTrackItem);
        }else if (itemType == typeof(AudioTrackItem))
        {
            DrawAudioTrackItem(currentTrackItem as AudioTrackItem);

        }else if (itemType == typeof(EffectTrackItem))
        {
            DrawEffectTrackItem(currentTrackItem as EffectTrackItem);
        }else if (itemType == typeof(AttackDetectTrackItem))
        {
            DrawAttackDetectTrackItem(currentTrackItem as AttackDetectTrackItem);
        }else if (itemType == typeof(MoveTrackItem))
        {
            DrawMoveTrackItem(currentTrackItem as MoveTrackItem);
        }
        
    }

 

    private void Clear()
    {
        if (root != null)
        {
            for (int i = root.childCount-1; i >=0; i--)
            {
                root.RemoveAt(i);
            }
        }
    }
    private int trackItemFrameIndex;

    public void SetTrackItemFrameIndex(int trackItemFrameIndex)
    {
        this.trackItemFrameIndex = trackItemFrameIndex;
    }
  

    #region 动画轨道

    private Label clipFrameLabel;
    private Toggle rootMotionToggle;
    private Label isLoopLabel;
    private IntegerField durationField;
    private FloatField transitionTimeField;
    private void DrawAnimationTrackItem(AnimationTrackItem animationTrackItem)
    {
        trackItemFrameIndex = animationTrackItem.FrameIndex;
        //动画资源
        ObjectField animationClipAssetField = new ObjectField("动画资源");
        animationClipAssetField.objectType = typeof(AnimationClip);
        animationClipAssetField.value = animationTrackItem.AnimationEvent.AnimationClip;
        animationClipAssetField.RegisterValueChangedCallback(AnimationClipAssetFieldValueChanged);
        root.Add(animationClipAssetField);

        //根运动
        rootMotionToggle = new Toggle("应用根运动");
        rootMotionToggle.value = animationTrackItem.AnimationEvent.ApplyRootMotion;
        rootMotionToggle.RegisterValueChangedCallback(RootMotionToggleValueChanged);
        root.Add(rootMotionToggle);
        
        //轨道长度
         durationField = new IntegerField("轨道长度");
        durationField.value = animationTrackItem.AnimationEvent.DurationFrame;
        durationField.RegisterCallback<FocusInEvent>(DurationFieldFocusIn);
        durationField.RegisterCallback<FocusOutEvent>(DurationFieldFocusOut);
        root.Add(durationField);
        
        //过渡时间
         transitionTimeField = new FloatField("过渡时间");
        transitionTimeField.value = animationTrackItem.AnimationEvent.TransitionTime;
        transitionTimeField.RegisterCallback<FocusInEvent>(TransitionTimeFieldFocusIn);
        transitionTimeField.RegisterCallback<FocusOutEvent>(TransitionTimeFieldFocusOut);
        root.Add(transitionTimeField);
        
        //动画相关信息
        int clipFrameCount = (int)(animationTrackItem.AnimationEvent.AnimationClip.length *
                                   animationTrackItem.AnimationEvent.AnimationClip.frameRate);
        clipFrameLabel = new Label("动画资源长度："+clipFrameCount);
        root.Add(clipFrameLabel);
         isLoopLabel =new Label("循环动画："+animationTrackItem.AnimationEvent.AnimationClip.isLooping);
        root.Add(isLoopLabel);

        //删除
        Button deleteButton = new Button(DeleteButtonClick);
        deleteButton.text = "删除";
        deleteButton.style.backgroundColor = new Color(1, 0, 0, 0.5f);
        root.Add(deleteButton);

    }

    private void RootMotionToggleValueChanged(ChangeEvent<bool> evt)
    {
        (currentTrackItem as AnimationTrackItem).AnimationEvent.ApplyRootMotion = evt.newValue;
        SkillEditorWindow.Instance.SaveConfig();
    }

    private int oldDurationValue;
    private void DurationFieldFocusIn(FocusInEvent evt)
    {
        oldDurationValue = durationField.value;
    } 
    private void DurationFieldFocusOut(FocusOutEvent evt)
    {
        if (durationField.value != oldDurationValue)
        {
        
            //安全校验
            if (((AnimationTrack)currentTrack).CheckFrameIndexOnDrag(trackItemFrameIndex + durationField.value,trackItemFrameIndex,false))
            {
                //修改数据,刷新视图 
                (currentTrackItem as AnimationTrackItem).AnimationEvent.DurationFrame = durationField.value;
                (currentTrackItem as AnimationTrackItem).CheckFrameCount();

                SkillEditorWindow.Instance.SaveConfig();
                currentTrackItem.ResetView();
            }
            else
            {
                durationField.value = oldDurationValue;
            }
        }
    }
    
    
    private float oldTransitionTimeValue;
    private void TransitionTimeFieldFocusIn(FocusInEvent evt)
    {
        oldTransitionTimeValue = transitionTimeField.value;
    } 
    private void TransitionTimeFieldFocusOut(FocusOutEvent evt)
    {
        if (transitionTimeField.value != oldTransitionTimeValue)
        {
            ((AnimationTrackItem)currentTrackItem).AnimationEvent.TransitionTime =
                transitionTimeField.value;
       
        }
    }
    private void DeleteButtonClick()
    {
        currentTrack.DeleteTrackItem(trackItemFrameIndex);//此函数提供保存和刷新视图逻辑
        Selection.activeObject = null;
        
    }

 
  

    private void AnimationClipAssetFieldValueChanged(ChangeEvent<Object> evt)
    {
        AnimationClip clip=evt.newValue as AnimationClip;
        //修改自身显示效果
        clipFrameLabel.text = "动画资源长度：" + (int)(clip.length * clip.frameRate);
        isLoopLabel.text = "循环动画：" + clip.isLooping;
        //保存到配置
        (currentTrackItem as AnimationTrackItem).AnimationEvent. AnimationClip = clip;
        SkillEditorWindow.Instance.SaveConfig();
        currentTrackItem.ResetView();
        
    }

    #endregion

    #region 音效軌道

    private FloatField voluemField;
    private void DrawAudioTrackItem(AudioTrackItem audioTrackItem)
    {
        //音效资源
        ObjectField audioClipAssetField = new ObjectField("音效資源");
        audioClipAssetField.objectType = typeof(AudioClip);
        audioClipAssetField.value = audioTrackItem.SkillAudioEvent.AudioClip;
        audioClipAssetField.RegisterValueChangedCallback(AudioClipAssetFieldValueChanged);
        root.Add(audioClipAssetField);

        //音量
        voluemField = new FloatField("播放音量");
        voluemField.value = audioTrackItem.SkillAudioEvent.Volume;
        voluemField.RegisterCallback<FocusInEvent>(VoluemFieldFocusIn);
        voluemField.RegisterCallback<FocusOutEvent>(VoluemFieldFocusOut);
        root.Add(voluemField);
    }
    private float oldVoluemFieldValue;

    private void VoluemFieldFocusOut(FocusOutEvent evt)
    {
        if (voluemField.value != oldVoluemFieldValue)
        {
            ((AudioTrackItem)currentTrackItem).SkillAudioEvent.Volume =
                voluemField.value;
       
        }
    }

    private void VoluemFieldFocusIn(FocusInEvent evt)
    {
        oldVoluemFieldValue = voluemField.value;

    }

    private void AudioClipAssetFieldValueChanged(ChangeEvent<Object> evt)
    {
        AudioClip audioClip=evt.newValue as AudioClip;
        //保存到配置中
        ((AudioTrackItem)currentTrackItem).SkillAudioEvent.AudioClip = audioClip;
        currentTrackItem.ResetView();
    }
  
    #endregion

    #region 特效轨道

    private FloatField effectDurationFiled;

    private void DrawEffectTrackItem(EffectTrackItem trackItem)
    {
        //预制体
        ObjectField effectPrefabAssetField = new ObjectField("特效预制体");
        effectPrefabAssetField.objectType = typeof(GameObject);
        effectPrefabAssetField.value = trackItem.SkillEffectEvent.Prefab;
        effectPrefabAssetField.RegisterValueChangedCallback(EffectPrefabAssetFieldValueChanged);
        root.Add(effectPrefabAssetField);
        
        //坐标
        Vector3Field posFiled = new Vector3Field("坐标");
        posFiled.value = trackItem.SkillEffectEvent.Position;
        posFiled.RegisterValueChangedCallback(EffectPosFiledAssetFieldValueChanged);
        root.Add(posFiled);
        
        //旋转
        Vector3Field rotFiled = new Vector3Field("旋转");
        rotFiled.value = trackItem.SkillEffectEvent.Rotation;
        rotFiled.RegisterValueChangedCallback(EffectRotFiledAssetFieldValueChanged);
        root.Add(rotFiled);  
        
        //缩放
        Vector3Field scaleFiled = new Vector3Field("缩放");
        scaleFiled.value = trackItem.SkillEffectEvent.Scale;
        scaleFiled.RegisterValueChangedCallback(EffectScaleFiledAssetFieldValueChanged);
        root.Add(scaleFiled);
        
        //自动销毁
        Toggle autoDestructToggle = new Toggle("自动销毁");
        autoDestructToggle.value = trackItem.SkillEffectEvent.AutoDestruct;
        autoDestructToggle.RegisterValueChangedCallback(EffectAutoDestructToggleValueChanged);
        root.Add(autoDestructToggle);
        
        //时间
        effectDurationFiled = new FloatField("持续时间");
        effectDurationFiled.value = trackItem.SkillEffectEvent.Duration;
        effectDurationFiled.RegisterCallback<FocusInEvent>(EffectDurationFiledFocusIn);
        effectDurationFiled.RegisterCallback<FocusOutEvent>(EffectDurationFiledFocusOut);
        root.Add(effectDurationFiled);
        
        //时间计算按钮
        Button calculateEffectButton = new Button(CalculateEffectDuration);
        calculateEffectButton.text = "重新计时";
        root.Add(calculateEffectButton); 
        
        //引用模型Transform属性
        Button applyModelTransformData = new Button(ApplyModelTransformData);
        applyModelTransformData.text = "引用模型Transform属性";
        root.Add(applyModelTransformData);
    }

    private void ApplyModelTransformData()
    {
        EffectTrackItem effectTrackItem = ((EffectTrackItem)currentTrackItem);
        effectTrackItem.ApplyModelTransformData();
        Show();
    }

    private void CalculateEffectDuration()
    {
        EffectTrackItem effectTrackItem = ((EffectTrackItem)currentTrackItem);
        ParticleSystem[] particleSystems = effectTrackItem.SkillEffectEvent.Prefab.GetComponentsInChildren<ParticleSystem>();

        float max = -1;
        int curr = -1;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i].main.duration > max)
            {
                max = particleSystems[i].main.duration;
                curr = i;
            }
        }

        effectTrackItem.SkillEffectEvent.Duration = particleSystems[curr].main.duration;
        effectDurationFiled.value = effectTrackItem.SkillEffectEvent.Duration;
        effectTrackItem.ResetView();
    }
    
    private float oldEffectDurationFiled;

    private void EffectDurationFiledFocusOut(FocusOutEvent evt)
    {
        if (effectDurationFiled.value != oldEffectDurationFiled)
        {
            EffectTrackItem effectTrackItem = ((EffectTrackItem)currentTrackItem);
            effectTrackItem.SkillEffectEvent.Duration =
                effectDurationFiled.value;
            effectTrackItem.ResetView();
        }
    }

    private void EffectDurationFiledFocusIn(FocusInEvent evt)
    {
        oldEffectDurationFiled = effectDurationFiled.value;

    }


    private void EffectAutoDestructToggleValueChanged(ChangeEvent<bool> evt)
    {
        EffectTrackItem effectTrackItem = ((EffectTrackItem)currentTrackItem);
        effectTrackItem.SkillEffectEvent.AutoDestruct = evt.newValue;
    }

    private void EffectPosFiledAssetFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        EffectTrackItem effectTrackItem = ((EffectTrackItem)currentTrackItem);
        effectTrackItem.SkillEffectEvent.Position = evt.newValue;
        effectTrackItem.ResetView(); 
        
      
    }
    private void EffectRotFiledAssetFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        EffectTrackItem effectTrackItem = ((EffectTrackItem)currentTrackItem);
        effectTrackItem.SkillEffectEvent.Rotation = evt.newValue;
        effectTrackItem.ResetView();

       
    }  private void EffectScaleFiledAssetFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        EffectTrackItem effectTrackItem = ((EffectTrackItem)currentTrackItem);
        effectTrackItem.SkillEffectEvent.Scale = evt.newValue;
        effectTrackItem.ResetView();

       
    }
    

    private void EffectPrefabAssetFieldValueChanged(ChangeEvent<Object> evt)
    {
        GameObject prefab=evt.newValue as GameObject;
        EffectTrackItem effectTrackItem = ((EffectTrackItem)currentTrackItem);
        effectTrackItem.SkillEffectEvent.Prefab = prefab;
       
        //重新计时
        CalculateEffectDuration();
        effectTrackItem.ResetView();

    }

    #endregion

    #region 攻击检测轨道
    private void DrawAttackDetectTrackItem(AttackDetectTrackItem attackDetectTrackItem)
    {
        trackItemFrameIndex = attackDetectTrackItem.FrameIndex;
        IntegerField integerField = new IntegerField("检测持续帧");
        integerField.value = attackDetectTrackItem.SkillAttackDetectEvent.DurationFrameLength;
        integerField.RegisterValueChangedCallback(AttackDetectIntegerValueChange);
        

        EnumField enumField = new EnumField("攻击检测类型", attackDetectTrackItem.SkillAttackDetectEvent.AttackDetect);
        enumField.RegisterValueChangedCallback(AttackDetectEnumValueChange);

        Button button = new Button(AttackDetectDelete);
        button.text = "删除";
        button.style.backgroundColor = new Color(1, 0, 0, 0.5f);
        root.Add(integerField);
        root.Add(enumField);
        root.Add(button);

    }

    private void AttackDetectDelete()
    {
        currentTrack.DeleteTrackItem(trackItemFrameIndex);//此函数提供保存和刷新视图逻辑
        Selection.activeObject = null;
    }

    private void AttackDetectEnumValueChange(ChangeEvent<Enum> evt)
    {
        AttackDetectTrackItem attackDetectTrackItem = (AttackDetectTrackItem)currentTrackItem;
        attackDetectTrackItem.SkillAttackDetectEvent.AttackDetect = (SkillAttackDetectEnum)evt.newValue;
        attackDetectTrackItem.ResetView();

    }

    private void AttackDetectIntegerValueChange(ChangeEvent<int> evt)
    {
        AttackDetectTrackItem attackDetectTrackItem = (AttackDetectTrackItem)currentTrackItem;
        attackDetectTrackItem.SkillAttackDetectEvent.DurationFrameLength = evt.newValue;
        attackDetectTrackItem.ResetView();
    }

    #endregion

    #region 开始移动轨道

    private void DrawMoveTrackItem(MoveTrackItem moveTrackItem)
    {
        trackItemFrameIndex = moveTrackItem.FrameIndex;
        IntegerField integerField = new IntegerField("持续移动帧");
        integerField.value = moveTrackItem.SkillMoveEvent.DurationFrameLength;
        integerField.RegisterValueChangedCallback(MoveTrackItemIntegerValueChange);
        root.Add(integerField);

        EnumField enumField = new EnumField("移动方向",moveTrackItem.SkillMoveEvent.MoveDirection);
        enumField.value = moveTrackItem.SkillMoveEvent.MoveDirection;
        enumField.RegisterValueChangedCallback(MoveTrackItemEnumValueChange);
        root.Add(enumField);

        FloatField floatField = new FloatField("移动距离");
        floatField.value=moveTrackItem.SkillMoveEvent.DisplacementDistance;
        floatField.RegisterValueChangedCallback(MoveTrackItemFloatValueChange);
        root.Add(floatField);

        
        Button button = new Button(MoveTrackItemDelete);
        button.text = "删除";
        button.style.backgroundColor = new Color(1, 0, 0, 0.5f);
        root.Add(button);

    }

    private void MoveTrackItemFloatValueChange(ChangeEvent<float> evt)
    {
        MoveTrackItem moveTrackItem = (MoveTrackItem)currentTrackItem;
        moveTrackItem.SkillMoveEvent.DisplacementDistance = evt.newValue;
        moveTrackItem.ResetView();
    }

    private void MoveTrackItemEnumValueChange(ChangeEvent<Enum> evt)
    {
        MoveTrackItem moveTrackItem = (MoveTrackItem)currentTrackItem;
        moveTrackItem.SkillMoveEvent.MoveDirection = (MoveDirectionEnum)evt.newValue;
        moveTrackItem.ResetView();
    }

    private void MoveTrackItemDelete()
    {
        currentTrack.DeleteTrackItem(trackItemFrameIndex);//此函数提供保存和刷新视图逻辑
        Selection.activeObject = null;
    }

    private void MoveTrackItemIntegerValueChange(ChangeEvent<int> evt)
    {
        MoveTrackItem moveTrackItem = (MoveTrackItem)currentTrackItem;
        moveTrackItem.SkillMoveEvent.DurationFrameLength = evt.newValue;
        moveTrackItem.ResetView();
    }

    #endregion
}

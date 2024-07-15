using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationTrack : SkillTrackBase
{
    private SkillSingleLineTrackStyle trackStyle;
    private Dictionary<int, AnimationTrackItem> trackItemDic = new Dictionary<int, AnimationTrackItem>();

    public SkillAnimationData AnimationData
    {
        get => SkillEditorWindow.Instance.SkillConfig.SkillAnimationData;
    }

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillSingleLineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "动画配置",AddTrack);
        trackStyle.contentRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        trackStyle.contentRoot.RegisterCallback<DragExitedEvent>(OnDragExited);
        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        //销毁当前已有
        foreach (var item in trackItemDic)
        {
            trackStyle.DeleteItem(item.Value.itemStyle.root);
        }

        trackItemDic.Clear();
        if (SkillEditorWindow.Instance.SkillConfig == null) return;
        //根据数据绘制TrackItem
        foreach (var item in AnimationData.FrameData)
        {
            CreateItem(item.Key, item.Value);
        }
    }

    private void CreateItem(int frameIndex, SkillAnimationEvent skillAnimationEvent)
    {
        AnimationTrackItem trackItem = new AnimationTrackItem();
        trackItem.Init(this, trackStyle, frameIndex, frameWidth, skillAnimationEvent);
        trackItemDic.Add(frameIndex, trackItem);
    }

    #region 拖拽资源

    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        //监听用户拖拽的是否是动画
        Object[] objs = DragAndDrop.objectReferences;
        AnimationClip clip = objs[0] as AnimationClip;
        if (clip != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }

    private void OnDragExited(DragExitedEvent evt)
    {
        //监听用户拖拽的是否是动画
        Object[] objs = DragAndDrop.objectReferences;
        AnimationClip clip = objs[0] as AnimationClip;
        if (clip != null)
        {
            //放置动画资源
            //当前选中的位置检测能否放置动画
            int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
            //检查选中帧不在任何已有的TrackItem之间
            bool canPlace = true;
            int durationFrame = -1; //代表可以用原本AnimationClip的持续时间、
            int clipFrameCount = (int)(clip.length * clip.frameRate);
            int nextTrackItem = -1;
            int currentOffset = int.MaxValue;
            foreach (var item in AnimationData.FrameData)
            {
                //不允许选中帧在TrackItem中间（动画事件的起点与终点间的位置）
                if (selectFrameIndex > item.Key && selectFrameIndex < item.Value.DurationFrame + item.Key)
                {
                    //不能放置
                    canPlace = false;
                    break;
                }

                //找到最右侧的TrackItem
                if (item.Key > selectFrameIndex)
                {
                    int tempOffset = item.Key - selectFrameIndex;
                    if (tempOffset < currentOffset)
                    {
                        currentOffset = tempOffset;
                        nextTrackItem = item.Key;
                    }
                }
            }

            //实际的放置
            if (canPlace)
            {
                //右边有其他TrackItem,要考虑Track不能重叠的问题
                if (nextTrackItem != -1)
                {
                    int offset = clipFrameCount - currentOffset;
                    if (offset < 0)
                    {
                        durationFrame = clipFrameCount;
                    }
                    else
                    {
                        durationFrame = currentOffset;
                    }
                }
                //右边啥都没有
                else
                {
                    durationFrame = clipFrameCount;
                }

                //构建动画数据
                SkillAnimationEvent animationEvent = new SkillAnimationEvent()
                {
                    AnimationClip = clip,
                    DurationFrame = durationFrame,
                    TransitionTime = 0.25f
                };
                //保存新增的动画数据
                AnimationData.FrameData.Add(selectFrameIndex, animationEvent);
                SkillEditorWindow.Instance.SaveConfig();
                //创建一个新的Item
                CreateItem(selectFrameIndex, animationEvent);
            }
        }
    }

    #endregion

    public override void AddTrack()
    {
        return;
        //构建动画数据
        SkillAnimationEvent animationEvent = new SkillAnimationEvent()
        {
            AnimationClip = null,
            DurationFrame = 10,
            TransitionTime = 0.25f
        };
        int tempLength=0;
      
        foreach (var item in AnimationData.FrameData)
        {
            if (item.Key + item.Value.DurationFrame > tempLength)
            {
                tempLength = item.Key + item.Value.DurationFrame;
            }
        }
        //保存新增的动画数据
        AnimationData.FrameData.Add(tempLength+1, animationEvent);
        SkillEditorWindow.Instance.SaveConfig();
        //创建一个新的Item
        CreateItem(tempLength+1, animationEvent);
    }

    public bool CheckFrameIndexOnDrag(int targetIndex, int selfIndex, bool isLeft)
    {
        foreach (var item in AnimationData.FrameData)
        {
            //规避拖拽时考虑自身
            if (selfIndex == item.Key) continue;
            //向左移动 && 原先在其右边 &&目标没有重叠
            if (isLeft && selfIndex > item.Key && targetIndex < item.Key + item.Value.DurationFrame)
            {
                return false;
            }
            //向右移动 && 原先在其左边 &&目标没有重叠

            else if (!isLeft && selfIndex < item.Key && targetIndex > item.Key)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 将oldIndex数据变为newIndex
    /// </summary>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    public void SetFrameIndex(int oldIndex, int newIndex)
    {
        if (AnimationData.FrameData.Remove(oldIndex, out SkillAnimationEvent animationEvent))
        {
            AnimationData.FrameData.Add(newIndex, animationEvent);
            trackItemDic.Remove(oldIndex, out AnimationTrackItem animationTrackItem);
            trackItemDic.Add(newIndex, animationTrackItem);
        }
    }

    public override void DeleteTrackItem(int frameIndex)
    {
        AnimationData.FrameData.Remove(frameIndex);
        if (trackItemDic.Remove(frameIndex, out AnimationTrackItem item))
        {
            trackStyle.DeleteItem(item.itemStyle.root);
        }
    }

    public override void OnConfigChanged()
    {
        foreach (var item in trackItemDic.Values)
        {
            item.OnConfigChanged();
        }
    }

    public Vector3 GetPositionForRootMotion(int frameIndex,bool flag=false)
    {
        GameObject previewGameObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        Animator animator = previewGameObject.GetComponent<Animator>();
        //根据帧找到目前是哪个动画
        Dictionary<int, SkillAnimationEvent> frameData = AnimationData.FrameData;
     

        //利用排序字典
        SortedDictionary<int, SkillAnimationEvent>
            frameDataSortedDic = new SortedDictionary<int, SkillAnimationEvent>(frameData);
        int[] keys = frameDataSortedDic.Keys.ToArray();
        Vector3 rootMotionTotalPos = Vector3.zero;
        
        for (int i = 0; i < keys.Length; i++)
        {
            int key = keys[i]; //当前动画的起始帧
            SkillAnimationEvent animationEvent = frameDataSortedDic[key];
            //只考虑根运动配置的动画
            if (animationEvent.ApplyRootMotion == false)
            {
                return rootMotionTotalPos;
            }
            int nextKeyFrame = 0;
            if (i + 1 < keys.Length) nextKeyFrame = keys[i + 1];
            else nextKeyFrame = SkillEditorWindow.Instance.SkillConfig.FrameCount;

            bool isBreak = false; //标记是最后一次采样
            if (nextKeyFrame > frameIndex)
            {
                nextKeyFrame = frameIndex;
                isBreak = true;
            }

            //持续的帧数=下一个动画的帧数-这个动画的开始时间
            int durationFrameCount = nextKeyFrame - key;
            if (durationFrameCount > 0)
            {
                //动画资源的总帧数
                float clipFrameCount = animationEvent.AnimationClip.length *
                                       SkillEditorWindow.Instance.SkillConfig.FrameRote;
                //计算当前的播放进度
                float totalProgress = durationFrameCount / clipFrameCount;
                //播放次数
                int playTimes = 1;
                //最终不完整的一次播放
                float lastProgress = 0;
                //只有循环动画才需要采样多次
                if (animationEvent.AnimationClip.isLooping)
                {
                    playTimes = (int)totalProgress;
                    lastProgress = totalProgress - (int)totalProgress;
                }
                else
                {
                    //因为总进度小于1,所以本身就是最后一次播放速度
                    if (totalProgress >= 1)
                    {
                        playTimes = 1;
                        lastProgress = 0;
                    }

                    if (totalProgress < 1f)
                    {
                        lastProgress = totalProgress;
                        playTimes = 0;
                    }
                }

                //采样计算
                animator.applyRootMotion = true;
                if (playTimes >= 1)
                {
                    //采样一次动画的完整进度
                    animationEvent.AnimationClip.SampleAnimation(previewGameObject,
                        animationEvent.AnimationClip.length);
                    Vector3 samplePos = previewGameObject.transform.position;
                    rootMotionTotalPos += samplePos * playTimes;
                }

                if (lastProgress > 0)
                {
                    //采样一次动画的不完整进度
                    animationEvent.AnimationClip.SampleAnimation(previewGameObject,
                        lastProgress * animationEvent.AnimationClip.length);
                    Vector3 samplePos = previewGameObject.transform.position;
                    rootMotionTotalPos += samplePos;
                }
            }

            if (isBreak) break;
        }

        if (flag)
        {
            UpdatePosTure(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
        }
        return rootMotionTotalPos;
    }

    private void UpdatePosTure(int frameIndex)
    {
        GameObject previewGameObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        if(!previewGameObject)return;
        Animator animator = previewGameObject.GetComponent<Animator>();
        //根据帧找到目前是哪个动画
        Dictionary<int, SkillAnimationEvent> frameData = AnimationData.FrameData;

        #region 关于当前帧的姿态

        //找到距离这一阵左边最近的一个动画,也就是当前要播放的动画
        int currentOffset = int.MaxValue; //最近的索引距离当前选中帧的差距
        int animationEventIndex = -1;
        foreach (var item in frameData)
        {
            int tempOffset = frameIndex - item.Key;
            if (tempOffset > 0 && tempOffset < currentOffset)
            {
                currentOffset = tempOffset;
                animationEventIndex = item.Key;
            }
        }

        if (animationEventIndex != -1)
        {
            SkillAnimationEvent animationEvent = frameData[animationEventIndex];
            //动画总帧数
            float clipFrameCount = animationEvent.AnimationClip.length * animationEvent.AnimationClip.frameRate;
            //计算当前的播放进度
            float progress = currentOffset / clipFrameCount;
            //循环动画的处理
            if (progress > 1 && animationEvent.AnimationClip.isLooping)
            {
                progress -= (int)progress; //只留小数部分
            }

            animator.applyRootMotion = animationEvent.ApplyRootMotion;
            animationEvent.AnimationClip.SampleAnimation(previewGameObject,
                progress * animationEvent.AnimationClip.length);
        }

        #endregion
    }

    public override void TickView(int frameIndex)
    {
        GameObject previewGameObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        if (previewGameObject != null)
        {
            previewGameObject.transform.position = GetPositionForRootMotion(frameIndex);
        }

        //更新姿态
        UpdatePosTure(frameIndex);
    }

    public override void Destory()
    {
        trackStyle.Destory();
    }
}
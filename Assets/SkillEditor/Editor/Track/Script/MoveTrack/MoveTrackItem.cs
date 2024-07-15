using UnityEngine;
using UnityEngine.UIElements;
    public class MoveTrackItem: TrackItemBase<MoveTrack>
    {
        private SkillMoveEvent moveEvent;
        public SkillMoveEvent SkillMoveEvent => moveEvent;
        private SkillMoveTrackItemStyle trackItemStyle;

        public void Init(MoveTrack moveTrack,SkillTrackStyleBase parentTrackStyle,int startFrameIndex, float frameUnitWidth,SkillMoveEvent skillMoveEvent)
        {
            track = moveTrack;
            this.frameIndex = startFrameIndex;
            this.frameUnitWidth = frameUnitWidth;
            this.moveEvent = skillMoveEvent;

            trackItemStyle = new SkillMoveTrackItemStyle();
            itemStyle = trackItemStyle;
            trackItemStyle.Init(parentTrackStyle);
            
            normalColor = new Color(0.388f, 0.85f, 0.905f, 0.5f);
            selectColor = new Color(0.388f, 0.85f, 0.905f, 1f);
            OnUnSelect();
            
            //绑定事件
            trackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
            trackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
            trackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
            trackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);

            trackItemStyle.animationOverLine.RegisterCallback<MouseDownEvent>(AnimationOverLineMouseDown);
            SkillEditorWindow.Instance.Root.RegisterCallback<MouseUpEvent>(AnimationOverLineMouseUp);
            SkillEditorWindow.Instance.Root.RegisterCallback<MouseMoveEvent>(AnimationOverLineMouseMove);
            ResetView(frameUnitWidth);
        }
        public override void ResetView(float frameUnitWidth)
        {
            base.ResetView(frameUnitWidth);
            this.frameUnitWidth = frameUnitWidth;
            trackItemStyle.SetTitle("开始移动");
            //位置计算

            trackItemStyle.SetPosition(frameIndex * frameUnitWidth);
            trackItemStyle.SetWidth(moveEvent.DurationFrameLength * frameUnitWidth);

            int animationClipFrameCount = moveEvent.DurationFrameLength;

            //计算动画结束线的位置
            trackItemStyle.animationOverLine.style.display = DisplayStyle.Flex;
            Vector3 overLinePos = trackItemStyle.animationOverLine.transform.position;
            overLinePos.x = animationClipFrameCount * frameUnitWidth - 2; //线条自身宽度为2
            trackItemStyle.animationOverLine.transform.position = overLinePos;
            track.TickView(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
        }
        #region 鼠标交互

    private bool mouseDrag = false;
    private float startDragPosX;
    private int startDrageFrameIndex;

    private void MouseMove(MouseMoveEvent evt)
    {
        if (mouseDrag)
        {
            float offsetPos = evt.mousePosition.x - startDragPosX;
            int offsetFrame = Mathf.RoundToInt(offsetPos / frameUnitWidth);
            int targetFtameIndex = startDrageFrameIndex + offsetFrame;
            bool checkDrag = false;
            if (targetFtameIndex < 0) return; //不考虑拖拽到负数的情况
            if (offsetFrame < 0)
            {
                checkDrag = track.CheckFrameIndexOnDrag(targetFtameIndex, startDrageFrameIndex, true);
            }
            else if (offsetFrame > 0)
            {
                checkDrag = track.CheckFrameIndexOnDrag(targetFtameIndex + moveEvent.DurationFrameLength,
                    startDrageFrameIndex, false);
            }
            else
            {
                return;
            }

            if (checkDrag)
            {
                //确定修改的数据
                frameIndex = targetFtameIndex;
                //如果超过右侧边界 拓展边界
                CheckFrameCount();
                //刷新视图
                ResetView(frameUnitWidth);
            }
        }
    }

    private void MouseOut(MouseOutEvent evt)
    {
        if (mouseDrag) ApplyDrag();
        mouseDrag = false;
    }

    private void MouseUp(MouseUpEvent evt)
    {
        if (mouseDrag) ApplyDrag();

        mouseDrag = false;
    }

    private void MouseDown(MouseDownEvent evt)
    {
        itemStyle.root.style.backgroundColor = selectColor;
        startDragPosX = evt.mousePosition.x;
        startDrageFrameIndex = frameIndex;
        mouseDrag = true;
        Select();
    }

    public void CheckFrameCount()
    {
        //如果超过右侧边界 拓展边界
        if (frameIndex + moveEvent.DurationFrameLength > SkillEditorWindow.Instance.SkillConfig.FrameCount)
        {
            SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + moveEvent.DurationFrameLength;
        }
    }

    private void ApplyDrag()
    {
        if (startDrageFrameIndex != frameIndex)
        {
            track.SetFrameIndex(startDrageFrameIndex, frameIndex);
            SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }

    #endregion
  #region 拖拽延长

    private bool mouseDragLine = false;
    private float startDragLinePosX;
    private int startDragLineFrameIndex;

    private void AnimationOverLineMouseDown(MouseDownEvent evt)
    {
        startDragLineFrameIndex = frameIndex;
        mouseDragLine = true;
        startDragLinePosX = evt.mousePosition.x;
    }

    private void AnimationOverLineMouseUp(MouseUpEvent evt)
    {
        mouseDragLine = false;
    }

    private void AnimationOverLineMouseMove(MouseMoveEvent evt)
    {
        if (mouseDragLine)
        {
            float offsetPosX = evt.mousePosition.x - startDragLinePosX;
            int offsetFrame = Mathf.RoundToInt(offsetPosX / frameUnitWidth);

            if (moveEvent.DurationFrameLength + offsetFrame < 2) return; //往左拉 持续时间不能少于2
            bool checkDrag = false;
            int selfFrame = startDragLineFrameIndex + moveEvent.DurationFrameLength;
            int tempKey = 0;
            if (offsetFrame > 0)
            {
                //判断右边
                foreach (var item in track.MoveData.FrameData)
                {
                    if (item.Key == startDragLineFrameIndex) continue;
                    if (item.Key > startDragLineFrameIndex && tempKey==0)
                    {
                        tempKey = item.Key;
                    }
                    
                    if (item.Key > startDragLineFrameIndex && item.Key < tempKey)
                    {
                        tempKey = item.Key;
                    }
                }
                if (selfFrame + offsetFrame <= tempKey || tempKey==0)
                {
                    moveEvent.DurationFrameLength += offsetFrame;
                    startDragLinePosX = evt.mousePosition.x;
                    ResetView(frameUnitWidth);
                }
            }
            else if (offsetFrame < 0)
            {
                if (moveEvent.DurationFrameLength + offsetFrame > 1)
                {
                    moveEvent.DurationFrameLength += offsetFrame;
                    startDragLinePosX = evt.mousePosition.x;
                    ResetView(frameUnitWidth);
                }
               
            }
        }
    }

    #endregion
    public override void OnConfigChanged()
    {
        moveEvent = track.MoveData.FrameData[frameIndex];
    }
    }

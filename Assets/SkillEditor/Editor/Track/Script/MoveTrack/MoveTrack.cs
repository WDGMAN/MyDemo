
    using System.Collections.Generic;
    using UnityEngine.UIElements;

    public class MoveTrack:SkillTrackBase
    {
        private SkillSingleLineTrackStyle trackStyle;
        private Dictionary<int, MoveTrackItem> trackItemDic = new Dictionary<int, MoveTrackItem>();
        public SkillMoveData MoveData => SkillEditorWindow.Instance.SkillConfig.skillMoveData;

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
        {
            base.Init(menuParent, trackParent, frameWidth);
            trackStyle = new SkillSingleLineTrackStyle();
            trackStyle.Init(menuParent, trackParent, "开始移动",AddTrack);
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
            foreach (var item in MoveData.FrameData)
            {
                CreateItem(item.Key, item.Value);
            }
        }
        private void CreateItem(int frameIndex, SkillMoveEvent skillMoveEvent)
        {
           MoveTrackItem trackItem = new MoveTrackItem();
            trackItem.Init(this, trackStyle, frameIndex, frameWidth, skillMoveEvent);
            trackItemDic.Add(frameIndex, trackItem);
        }
        public bool CheckFrameIndexOnDrag(int targetIndex, int selfIndex, bool isLeft)
        {
            foreach (var item in MoveData.FrameData)
            {
                //规避拖拽时考虑自身
                if (selfIndex == item.Key) continue;
                //向左移动 && 原先在其右边 &&目标没有重叠
                if (isLeft && selfIndex > item.Key && targetIndex < item.Key + item.Value.DurationFrameLength)
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
            if (MoveData.FrameData.Remove(oldIndex, out SkillMoveEvent moveEvent))
            {
                MoveData.FrameData.Add(newIndex, moveEvent);
                trackItemDic.Remove(oldIndex, out MoveTrackItem moveTrackItem);
                trackItemDic.Add(newIndex, moveTrackItem);
            }
        }
        public override void DeleteTrackItem(int frameIndex)
        {
            MoveData.FrameData.Remove(frameIndex);
            if (trackItemDic.Remove(frameIndex, out MoveTrackItem item))
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
        public override void TickView(int frameIndex)
        {
            // GameObject previewGameObject = SkillEditorWindow.Instance.PreviewCharacterObj;
          

        }
        public override void Destory()
        {
            trackStyle.Destory();
        }
        public override void AddTrack()
        {
            
            SkillMoveEvent skillMoveEvent = new SkillMoveEvent();
            int tempLength=0;
      
            foreach (var item in MoveData.FrameData)
            {
                if (item.Key + item.Value.DurationFrameLength > tempLength)
                {
                    tempLength = item.Key + item.Value.DurationFrameLength;
                }
            }
            MoveData.FrameData.Add(tempLength+1, skillMoveEvent);
            SkillEditorWindow.Instance.SaveConfig();
            //创建一个新的Item
            CreateItem(tempLength+1, skillMoveEvent);
        }

    }

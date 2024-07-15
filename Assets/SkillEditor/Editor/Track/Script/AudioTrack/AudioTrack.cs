
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class AudioTrack:SkillTrackBase
    {
        private SkillMultilineTrackStyle trackStyle;
        public SkillAudioData AudioData
        {
            get => SkillEditorWindow.Instance.SkillConfig.SkillAudioData;
        }

        private List<AudioTrackItem> trackItemList = new List<AudioTrackItem>();
        public override void Init(VisualElement menuParent, VisualElement trackParent,float frameWidth)
        {
            base.Init(menuParent, trackParent,frameWidth);
            trackStyle = new SkillMultilineTrackStyle();
            trackStyle.Init(menuParent,trackParent,"音效配置",AddChildTrack,CheckDeleteChildTrack,SwapChildTrack,UpdateChildTrackName);
         

            ResetView();
        }

    

        public override void ResetView(float  frameWidth)
        {
            base.ResetView(frameWidth);
            
            //销毁已有的
            for (int i = 0; i < trackItemList.Count; i++)
            {
                trackItemList[i].Destory();
            }
         
            trackItemList.Clear();

            if(SkillEditorWindow.Instance.SkillConfig==null)return;
            foreach (SkillAudioEvent item in AudioData.FrameData)
            {

                CreateItem(item);
            }
        }

        private void CreateItem(SkillAudioEvent skillAudioEvent)
        {
            AudioTrackItem item = new AudioTrackItem();
            item.Init(this,frameWidth,skillAudioEvent,trackStyle.AddChildTrack());
            item.SetTrackName(skillAudioEvent.TrackName);
            trackItemList.Add(item);
        }
        private void UpdateChildTrackName(SkillMultilineTrackStyle.ChildTrack childTrack, string newName)
        {
            //同步給配置
            AudioData.FrameData[childTrack.GetIndex()].TrackName = newName;
            SkillEditorWindow.Instance.SaveConfig();
        }
    
        private void AddChildTrack()
        {
            SkillAudioEvent skillAudioEvent=new SkillAudioEvent();
            AudioData.FrameData.Add(skillAudioEvent);
            CreateItem(skillAudioEvent);
            SkillEditorWindow.Instance.SaveConfig();
        }

        private bool CheckDeleteChildTrack(int index)
        {
            if (index < 0 || index >= AudioData.FrameData.Count)
            {
                return false;
            }
        
            SkillAudioEvent skillAudioEvent = AudioData.FrameData[index];
            if (skillAudioEvent != null)
            {
                trackItemList.RemoveAt(index);

                AudioData.FrameData.RemoveAt(index);
                SkillEditorWindow.Instance.SaveConfig();
            }
           
            return skillAudioEvent!=null;
        }


        private void SwapChildTrack(int index1,int index2)
        {
            SkillAudioEvent data1 = AudioData.FrameData[index1];
            SkillAudioEvent data2 = AudioData.FrameData[index2];

            AudioData.FrameData[index1] = data2;
            AudioData.FrameData[index2] = data1;
            //保存交给窗口的退出机制
        }
        public override void Destory()
        {
            trackStyle.Destory();
        }

        public override void OnPlay(int startFrameIndex)
        {
            for (int i = 0; i < AudioData.FrameData.Count; i++)
            {
                SkillAudioEvent audioEvent = AudioData.FrameData[i];
                if(audioEvent.AudioClip==null)continue;

                int audioFrameCount =
                    (int)(audioEvent.AudioClip.length * SkillEditorWindow.Instance.SkillConfig.FrameRote);
                int audioLastFrameIndex =audioFrameCount+audioEvent.FrameIndex;
                //意味著開始位置在左邊 並且 長度大於當前選中幀
                //也就是時間軸播放幀在軌道的中間部分
                if (audioEvent.FrameIndex < startFrameIndex &&  audioLastFrameIndex>startFrameIndex)
                {
                    int offset = startFrameIndex - audioEvent.FrameIndex;
                    float playRate = (float)offset / audioFrameCount;
                    EditorAudioUnility.PlayAudio(audioEvent.AudioClip,playRate);
                }else if (audioEvent.FrameIndex==startFrameIndex)
                {
                    //播放音效.從頭播放
                    EditorAudioUnility.PlayAudio(audioEvent.AudioClip,0);
                }
            }
        }

        public override void TickView(int frameIndex)
        {
            base.TickView(frameIndex);
            if (SkillEditorWindow.Instance.IsPlaying)
            {
                for (int i = 0; i < AudioData.FrameData.Count; i++)
                {
                    SkillAudioEvent audioEvent = AudioData.FrameData[i];
                    if (audioEvent.AudioClip != null && audioEvent.FrameIndex == frameIndex)
                    {
                        //播放音效.從頭播放
                        EditorAudioUnility.PlayAudio(audioEvent.AudioClip,0);
                    }
                }
            }
        }
    }

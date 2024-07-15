

    using System;
    using System.Collections.Generic;
    using Sirenix.Serialization;

    public class SkillAudioData
    {
        /// <summary>
        /// 动画帧事件
        /// Key：帧数
        /// Value：事件数据
        /// </summary>
        [NonSerialized,OdinSerialize]
        public List<SkillAudioEvent> FrameData = new List<SkillAudioEvent>();
    }

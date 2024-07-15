
    using System;
    using System.Collections.Generic;
    using Sirenix.Serialization;

    public class SkillMoveData
    {
        [NonSerialized,OdinSerialize]
        public Dictionary<int, SkillMoveEvent> FrameData = new Dictionary<int, SkillMoveEvent>();
    }

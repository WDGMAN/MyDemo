using System;
using System.Collections.Generic;
using Sirenix.Serialization;

namespace Config.Skill.Attack
{
    public class SkillAttackDetectData
    {
        /// <summary>
        /// 攻击检测事件
        /// </summary>
        [NonSerialized,OdinSerialize]
        public Dictionary<int, SkillAttackDetectEvent> FrameData = new Dictionary<int, SkillAttackDetectEvent>();
    }
}
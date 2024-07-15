
using System;
using System.Collections.Generic;
using Config.Skill.Attack;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/SkillConfig",fileName = "SkillConfig")]
public class SkillConfig : SerializedScriptableObject
{
  [LabelText("技能名称")]public string SkillName;
  [LabelText("帧数上限")]public int FrameCount=100;
  [LabelText("帧率")]public int FrameRote=30;

  [NonSerialized,OdinSerialize]
  public SkillAnimationData SkillAnimationData=new SkillAnimationData(); 
  
  [NonSerialized,OdinSerialize]
  public SkillAudioData SkillAudioData=new SkillAudioData(); 
  [NonSerialized,OdinSerialize]
  public SkillEffectData SkillEffectData=new SkillEffectData();
  [NonSerialized,OdinSerialize]
  public SkillAttackDetectData skillAttackDetectData = new SkillAttackDetectData();
  [NonSerialized,OdinSerialize]
  public SkillMoveData skillMoveData = new SkillMoveData();
  

  #if UNITY_EDITOR
  private static Action skillConfigValidate;

  public static void SetValidateAction(Action action)
  {
    skillConfigValidate = action;
  }
  private void OnValidate()
  {
    skillConfigValidate?.Invoke();
  }
  #endif
 
}




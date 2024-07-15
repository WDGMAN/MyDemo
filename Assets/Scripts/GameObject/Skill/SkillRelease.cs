using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WDGFrame;

/// <summary>
/// 技能释放器
/// </summary>
public class SkillRelease : MonoBehaviour
{
    private AnimancerComponent animancer;
    private bool isPlaying = false; //当前是否处于播放状态

    public bool IsPlaying
    {
        get => isPlaying;
    }

    private SkillConfig skillConfig; //当前播放的技能配置
    private int currentFrameIndex; //当前第几帧
    private float playerTotalTime; //当前播放的总时间
    private float frameRote; //当前技能帧率


    private Action<Vector3, Quaternion> rootMotionAction;
    private Action skillEndAction;
    private Action attackDetectAction; //攻击检测事件
    private Action attackDetectEndAction; //攻击检测结束
    private Action<float, MoveDirectionEnum, float> moveAction; //移动事件

    /// <summary>
    /// 播放技能
    /// </summary>
    /// <param name="skillConfig"></param>
    public void PlaySkill(SkillConfig skillConfig, Action skillEndAction, Action attackDetectAction,
        Action attackDetectEndAction, Action<float, MoveDirectionEnum, float> moveAction,
        Action<Vector3, Quaternion> rootMotionAction = null)
    {
        this.moveAction = moveAction;
        this.attackDetectEndAction = attackDetectEndAction;
        this.attackDetectAction = attackDetectAction;
        this.skillConfig = skillConfig;
        currentFrameIndex = -1;
        frameRote = skillConfig.FrameRote;
        playerTotalTime = 0;
        isPlaying = true;
        this.skillEndAction = skillEndAction;
        this.rootMotionAction = rootMotionAction;
        TickSkill();
    }

    private void Start()
    {
        animancer = GetComponent<AnimancerComponent>();
    }

    private void Update()
    {
        if (isPlaying)
        {
            playerTotalTime += Time.deltaTime;
            //根据总时间判断当前第几帧
            int targetFrameIndex = (int)(playerTotalTime * frameRote);
            //防止一帧延迟过大,追帧
            while (currentFrameIndex < targetFrameIndex)
            {
                //驱动一次技能
                TickSkill();
            }

            //如果到达最后一帧,技能结束
            if (targetFrameIndex >= skillConfig.FrameCount)
            {
                isPlaying = false;
                skillConfig = null;
                // if(rootMotionAction!=null)
                rootMotionAction = null;
                skillEndAction?.Invoke();
            }

            //进行移动操作
            if (isStartMove)
            {
                moveAction?.Invoke(
                    (float)skillConfig.skillMoveData.FrameData[currentMoveFrameIndex].DurationFrameLength /
                    skillConfig.FrameRote, skillConfig.skillMoveData.FrameData[currentMoveFrameIndex].MoveDirection,
                    skillConfig.skillMoveData.FrameData[currentMoveFrameIndex].DisplacementDistance);
            }
            //进行攻击检测
            if (isStartAttackDetect)
            {
                attackDetectAction?.Invoke();
            }
        }
    }

    private bool isStartAttackDetect = false;
    private int currentAttackDetectFrameIndex;

    private bool isStartMove = false;
    private int currentMoveFrameIndex;


    private void TickSkill()
    {
        currentFrameIndex += 1;

        //驱动动画
        if (skillConfig.SkillAnimationData.FrameData.TryGetValue(currentFrameIndex,
                out SkillAnimationEvent skillAnimationEvent))
        {
            animancer.Play(skillAnimationEvent.AnimationClip, skillAnimationEvent.TransitionTime);
            if (skillAnimationEvent.ApplyRootMotion)
            {
                //暂定
            }
        }

        //驱动音效
        for (int i = 0; i < skillConfig.SkillAudioData.FrameData.Count; i++)
        {
            SkillAudioEvent audioEvent = skillConfig.SkillAudioData.FrameData[i];
            if (audioEvent.AudioClip != null && audioEvent.FrameIndex == currentFrameIndex)
            {
                //播放音效.從頭播放
                //自己写
            }
        }


        //驱动特效
        for (int i = 0; i < skillConfig.SkillEffectData.FrameData.Count; i++)
        {
            SkillEffectEvent effectEvent = skillConfig.SkillEffectData.FrameData[i];
            if (effectEvent.Prefab != null && effectEvent.FrameIndex == currentFrameIndex)
            {
                GameObject effectObj = PoolManager.Instance.GetGameObject("SkillPartic" + i);
                //播放特效
                if (effectObj == null)
                {
                    effectObj = Instantiate(effectEvent.Prefab);
                }

                effectObj.transform.position =transform.TransformPoint(effectEvent.Position);
                effectObj.transform.rotation =Quaternion.Euler(transform.rotation.eulerAngles+effectEvent.Rotation);

                effectObj.transform.localScale = effectEvent.Scale;
                effectObj.GetComponent<ParticleSystem>().Play();
                StartCoroutine(EffectEndIE(effectEvent.Duration, effectObj, i));
                break;
            }
        }

        //驱动攻击检测
        if (skillConfig.skillAttackDetectData.FrameData.TryGetValue(currentFrameIndex,
                out SkillAttackDetectEvent attackDetectEvent))
        {
            isStartAttackDetect = true;
            currentAttackDetectFrameIndex = currentFrameIndex;
        }
        if (isStartAttackDetect)
        {
            int durationTime =
                skillConfig.skillAttackDetectData.FrameData[currentAttackDetectFrameIndex].DurationFrameLength +
                currentAttackDetectFrameIndex;
            if (durationTime == currentFrameIndex)
            {
                isStartAttackDetect = false;
                attackDetectEndAction?.Invoke();
            }
        }

        //驱动移动
        if (skillConfig.skillMoveData.FrameData.TryGetValue(currentFrameIndex, out SkillMoveEvent skillMoveEvent))
        {
            isStartMove = true;
            currentMoveFrameIndex = currentFrameIndex;
        }

        if (isStartMove)
        {
            int durationTime = skillConfig.skillMoveData.FrameData[currentMoveFrameIndex].DurationFrameLength +
                               currentMoveFrameIndex;
            if (durationTime == currentFrameIndex)
            {
                isStartMove = false;
            }
        }
    }

    private IEnumerator EffectEndIE(float duration, GameObject gameObject, int index)
    {
        yield return new WaitForSeconds(duration);
        PoolManager.Instance.PushGameObject(gameObject, "SkillPartic" + index);
    }
}
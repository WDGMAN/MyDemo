using System;
using Animancer;
using UnityEngine;

public enum ClimbEnum
{
    Cross, //跨越
    Vault, //翻越
    Climb, //攀爬
    None
}

public class Climb : MonoBehaviour
{
    private Transform OwnTransofrm;
    private float Radius; //半径
    public AnimatedFloat AnimationFloat { get; private set; }
    private AnimancerComponent animancer;
    private Animator animator;
    private Action action;

    public void Init(Transform transform, float radius, AnimancerComponent animancerComponent, Animator animator)
    {
        OwnTransofrm = transform;
        Radius = radius;
        animancer = animancerComponent;
        this.animator = animator;
        AnimationFloat = new AnimatedFloat(animancer, "ClimbHeight", "ClimbForward", "LeftHandIK", "RightHandIK");
    }

    #region vault翻越

    /// <summary>
    /// 翻越
    /// </summary>
    public float MaxClimbHeight = 2f; //最大可翻越高度

    public float OffsetInterval = 0.3f; //每次检测的间隔
    public float OffsetDistance = 0.3f; //间隔多远就可以开始爬墙检测

    public ClimbWallData DetectClimbWall()
    {
        RaycastHit hit = new RaycastHit();
        ClimbWallData climbWallData = detectClimbWallRecursion(0, ref hit);
        if (climbWallData != null)
        {
            climbWallData.AnimatedFloat = AnimationFloat;
            action = () =>
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand,
                    climbWallData.TargetPosHit.point + Vector3.Cross(-climbWallData.WallHit.normal, Vector3.up) * 0.3f +
                    Vector3.up * 0.03f);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, AnimationFloat[2]);
                animator.SetIKPosition(AvatarIKGoal.RightHand,
                    climbWallData.TargetPosHit.point +
                    Vector3.Cross(-climbWallData.WallHit.normal, Vector3.down) * 0.3f + Vector3.up * 0.03f);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, AnimationFloat[3]);
                if (AnimationFloat[2] != 0 || AnimationFloat[3] != 0)
                {
                    
                }
                // Quaternion tempLeftHand =
                //     Quaternion.LookRotation(Vector3.Cross(-climbWallData.WallHit.normal, Vector3.up));
                // Quaternion tempRightHand =
                //     Quaternion.LookRotation(Vector3.Cross(-climbWallData.WallHit.normal, Vector3.down));
                Quaternion temp = Quaternion.LookRotation(-climbWallData.WallHit.normal);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, temp);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, AnimationFloat[2]);
                animator.SetIKRotation(AvatarIKGoal.RightHand, temp);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, AnimationFloat[3]);
            };
        }

        
        return climbWallData;
    }

    private ClimbWallData detectClimbWallRecursion(float offsetIntervalTemp, ref RaycastHit hit)
    {
        offsetIntervalTemp += OffsetInterval;
        if (Physics.Raycast(OwnTransofrm.position + Vector3.up * offsetIntervalTemp, OwnTransofrm.forward,
                out RaycastHit tempHit,
                Radius + OffsetDistance))
        {
            if (offsetIntervalTemp > MaxClimbHeight)
            {
                return null;
            }

            return detectClimbWallRecursion(offsetIntervalTemp, ref tempHit);
        }
        else
        {
            //如果是第一次就没有检测到 说明没有墙
            if (offsetIntervalTemp == OffsetInterval)
            {
                return null;
            }

            Vector3 origin = hit.point + Vector3.up * OffsetInterval + (-hit.normal * 0.1f); //向下检测的起点位置
            if (Physics.Raycast(origin, Vector3.down,
                    out RaycastHit resultHit, OffsetInterval + 0.01f))
            {
                Vector3 dir = Vector3.Cross(hit.normal, Vector3.up);

                //检测到墙后 检测两边距离是否足够攀爬
                if (Physics.Raycast(origin + dir * Radius, Vector3.down,
                        OffsetInterval + 0.01f) && Physics.Raycast(origin - dir * Radius, Vector3.down,
                        OffsetInterval + 0.01f))
                {
                    //两边距离都满足才能够爬墙
                    float distance = offsetIntervalTemp - resultHit.distance;
                    ClimbWallData climbData = new ClimbWallData();
                    climbData.WallHit = hit;
                    climbData.TargetPosHit = resultHit;
                    climbData.WallHeight = distance;
                    climbData.ClimbEnum = decideClimbEnum(distance);
                    return climbData;
                }

                //两边距离不够 不能爬
                return null;
            }
        }

        return null;
    }

    private ClimbEnum decideClimbEnum(float height)
    {
        if (height > 0.3f && height < 1f)
        {
            return ClimbEnum.Cross;
        }
        else if (height >= 1f && height < 2f)
        {
            return ClimbEnum.Vault;
        }
        else
        {
            return ClimbEnum.Climb;
        }
    }

    protected void OnAnimatorIK(int layerIndex)
    {
        action?.Invoke();
    }

    #endregion
}

public class ClimbWallData
{
    public RaycastHit WallHit;
    public RaycastHit TargetPosHit;
    public float WallHeight;
    public ClimbEnum ClimbEnum;
    public AnimatedFloat AnimatedFloat;
}
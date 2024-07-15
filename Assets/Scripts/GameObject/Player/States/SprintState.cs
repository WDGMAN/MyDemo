using Animancer;
using UnityEngine;
using WDGFrame;

public class SprintState_Player:PlayerStateBase
{
    private AnimationClip animationClip;
    private float sprintSpeed=15f;//冲刺速度
    private Vector3 moveDir;//移动方向
    private Quaternion targetRotation;//目标旋转值
    public SprintState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
    {
        animationClip = Resources.Load<AnimationClip>("Animation/Player/Sprint");
    }

    public override void Enter()
    {
        Vector3 dir = new Vector3(InputManager.Instance.WasPressedMove().x, 0,
            InputManager.Instance.WasPressedMove().y);
        moveDir = controller.GetCameraToForwardDir(dir);
        targetRotation = controller.GetCameraToForwardRotation(dir);
     AnimancerState animancerState=controller.Animancer.Play(animationClip, 0.25f,FadeMode.FromStart);
     sprintSpeed = 10f;
     animancerState.Events.Add(0.39f, () =>
     {
         sprintSpeed = 0;
     });
     animancerState.Events.Add(0.4f, () =>
     {
         if (Input.WasPressedMove() != Vector2.zero)
         {
             fsm.ChangeState(PlayerStateEnum.Move);
         }
     });
     animancerState.Events.OnEnd = () =>
     {
         fsm.ChangeState(PlayerStateEnum.Idle);
     };
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {
        controller.PlayerMove(moveDir,sprintSpeed,targetRotation);
    }
}
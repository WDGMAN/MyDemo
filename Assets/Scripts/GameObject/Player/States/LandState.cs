using UnityEngine;
using WDGFrame;

public class LandState_Player : PlayerStateBase
{
    private AnimationClip animationClip;

    public LandState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
    {
        animationClip = Resources.Load<AnimationClip>("Animation/Player/JumpLand");
    }

    public override void Enter()
    {
        controller.Animancer.Play(animationClip, 0.25f).Events.Add(0.8f,
            () => { fsm.ChangeState(PlayerStateEnum.Idle); }
        );
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (Input.WasPressedMove() != Vector2.zero)
        {
            fsm.ChangeState(PlayerStateEnum.Move);
        }
    }
}

using Animancer;
using UnityEngine;
using WDGFrame;

public class IdleState_Player : PlayerStateBase
{
  private AnimationClip animationClipA;
  private AnimationClip animationClipB;

  private float duration;
  public IdleState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
  {
    animationClipA = Resources.Load<AnimationClip>("Animation/Player/IdleA");
    animationClipB = Resources.Load<AnimationClip>("Animation/Player/IdleB");
  }
 

  public override void Enter()
  {
    fsm.SetShareData("currentSpeed",0f);

    controller.Animancer.Play(animationClipB,0.35f,FadeMode.FromStart);
  }

  public override void Exit()
  {
   
    
  }

  public override void Update()
  {
    if (Input.WasPressedMove() != Vector2.zero)
    {
      fsm.ChangeState(PlayerStateEnum.Move);
    }else if (Input.WasPressedJump())
    {
      fsm.ChangeState(PlayerStateEnum.Up);
    }else if (Input.WasPressedSprint())
    {
      fsm.ChangeState(PlayerStateEnum.Sprint);
    }else if (Input.WasPressedAttack())
    {
      fsm.ChangeState(PlayerStateEnum.Attack);
    }

    duration += Time.deltaTime;
  }



}

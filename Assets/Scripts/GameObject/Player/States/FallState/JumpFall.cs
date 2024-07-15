
    using Unity.VisualScripting;
    using UnityEngine;
    using WDGFrame;

    public class JumpFallState_Player:PlayerStateBase
    {
        private AnimationClip animationClip;
        public JumpFallState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
        {
            animationClip = Resources.Load<AnimationClip>("Animation/Player/JumpFall");
        }

        public override void Enter()
        {
            controller.Animancer.Play(animationClip, 0.25f);
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            if (controller.IsGround)
            {
                fsm.ParentFSM.ChangeState(PlayerStateEnum.Land);
            }
        }

        public override void FixedUpdate()
        {
            controller.MoveFixed(controller.transform.forward,fsm.ParentFSM.GetShareData<float>("currentSpeed"));
        }
    }

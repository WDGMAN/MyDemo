
    using UnityEngine;
    using WDGFrame;

    public class JumpUpState_Player:PlayerStateBase
    {
        private float bug;
        private AnimationClip animationClip;
        private float jumpHeight=1.2f;
        private float jumpVelocity;
        private Vector3 oldPos;
        private bool fall;
        public JumpUpState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
        {
            animationClip = Resources.Load<AnimationClip>("Animation/Player/JumpUp");
        }

        public override void Enter()
        {
            oldPos = Vector3.zero;
            fall = false;
            bug = 0;
            controller.Animancer.Play(animationClip, 0.3f);
            jumpVelocity = Mathf.Sqrt(2 * controller.GravityMagnitude * jumpHeight);
        }

        public override void Exit()
        {
        }

        public override void Update()
        {

            if (controller.transform.position.y < oldPos.y)
            {
                fall = true;
            }

            if (controller.IsGround && bug > 0.2f)
            {
                fsm.ParentFSM.ChangeState(PlayerStateEnum.Idle);
            }
            oldPos = controller.transform.position;
        }

        public override void FixedUpdate()
        {
            controller.MovePositionFixed(Vector3.up*jumpVelocity*Time.fixedDeltaTime);
            controller.MoveFixed(controller.transform.forward,fsm.ParentFSM.GetShareData<float>("currentSpeed"));
            if (fall && bug>0.2f)
            {
                fsm.ParentFSM.ChangeState(PlayerStateEnum.Fall);
            }

            bug += Time.fixedDeltaTime;
        }
        
    }

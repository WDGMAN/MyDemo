
    using Animancer;
    using UnityEngine;
    using WDGFrame;

    public class ClimbState_Player:PlayerStateBase
    {
        private AnimationClip vaultClimbAniamtionClip;
        private ClimbWallData climbWallData;
        private Vector3 currentPos;
        public ClimbState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
        {
            vaultClimbAniamtionClip = Resources.Load<AnimationClip>("Animation/Player/Vault");
        }

        public override void Enter()
        {
            currentPos = controller.transform.position;
            oldHeight = 0;
            oldForwardPos=Vector3.zero;
            controller.GravityFlag = false;
            climbWallData= fsm.GetShareData<ClimbWallData>("climbData");
            AnimancerState animancerState = controller.Animancer.Play(vaultClimbAniamtionClip, 0.25f);
            animancerState.ApplyAnimatorIK = true;
            animancerState.Events.OnEnd = () =>
            {
                controller.GravityFlag = true;
                animancerState.ApplyAnimatorIK = false;
                fsm.ParentFSM.ChangeState(PlayerStateEnum.Idle);
            };

        }

        public override void Exit()
        {
            controller.GravityFlag = true;
        }

    
        private float oldHeight;
        private Vector3 oldForwardPos;
        public override void FixedUpdate()
        {
            //首先将角色面相墙壁
            controller.Rotation(-climbWallData.WallHit.normal,4f);
            //然后移动
            //高度
            float tempHeight = Mathf.Lerp(currentPos.y, currentPos.y + climbWallData.WallHeight,
                climbWallData.AnimatedFloat[0]);
            if (oldHeight == 0) oldHeight = tempHeight;

            float differenceHeight = tempHeight - oldHeight;

            oldHeight = tempHeight;
            controller.MovePositionFixed(differenceHeight*Vector3.up);
            //前进
            currentPos.y = 0;
            Vector3 tempForwardPos = Vector3.Lerp(currentPos,
                currentPos + (-climbWallData.WallHit.normal * controller.CharacterController.radius),
                climbWallData.AnimatedFloat[1]);
            if (oldForwardPos == Vector3.zero) oldForwardPos = tempForwardPos;
            Vector3 forwardPos = tempForwardPos - oldForwardPos;
            oldForwardPos = tempForwardPos;

            controller.MovePositionFixed(-climbWallData.WallHit.normal*forwardPos.magnitude);
            
        }

        public override void Update()
        {
        }
    }

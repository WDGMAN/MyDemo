
    using UnityEngine;
    using WDGFrame;

    public class UpState_Player:PlayerStateBase
    {
     
        
        public PlayerStateMachine FSM { get; private set; }
        
        public UpState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
        {
            FSM = new PlayerStateMachine(fsm);
            FSM.AddShareData("climbData",null);
            initState();
        }

        public override void Enter()
        {
            FSM.ChangeState(decideState());
        }

        public override void FixedUpdate()
        {
            FSM.OnFixedUpdate();
        }

        public override void Update()
        {
            FSM.OnUpdate();
        }

        public override void LateUpdate()
        {
            FSM.OnLateUpdate();
        }

        public override void Exit()
        {

        }

        /// <summary>
        /// 初始化状态
        /// </summary>
        private void initState()
        {
            JumpUpState_Player jumpState = new JumpUpState_Player(FSM, controller);
            FSM.AddState(PlayerStateEnum.JumpUp,jumpState);
            ClimbState_Player climbState = new ClimbState_Player(FSM, controller);
            FSM.AddState(PlayerStateEnum.Climb,climbState);
        }

        /// <summary>
        /// 进入判断状态
        /// </summary>
        private PlayerStateEnum decideState()
        {
            ClimbWallData climbWallData=  controller.Climb.DetectClimbWall();
            if (climbWallData == null)
            {
                return PlayerStateEnum.JumpUp;
            }
            else
            {
                FSM.SetShareData("climbData",climbWallData);
                return PlayerStateEnum.Climb;
            }
        }
    }
    

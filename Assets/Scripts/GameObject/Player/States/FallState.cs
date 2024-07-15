
    using WDGFrame;

    public class FallState_Player:PlayerStateBase
    {
        public PlayerStateMachine FSM { get; private set; }
        public FallState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
        {
            FSM = new PlayerStateMachine(fsm);
            init();
        }

        public override void Enter()
        {
            FSM.ChangeState(decideState());
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            FSM.OnUpdate();
        }

        public override void FixedUpdate()
        {
            FSM.OnFixedUpdate();

        }

        public override void LateUpdate()
        {
            FSM.OnLateUpdate();
        }

        private void init()
        {
            JumpFallState_Player fallState = new JumpFallState_Player(FSM, controller);
            FSM.AddState(PlayerStateEnum.JumpFall,fallState);
        }

        private PlayerStateEnum decideState()
        {
            return PlayerStateEnum.JumpFall;
        }
    }

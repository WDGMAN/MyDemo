
    using UnityEngine.InputSystem;
    using WDGFrame;

    public class PlayerStateBase:StateBase<PlayerStateEnum>
    {
        protected InputManager Input;
        protected new PlayerController  controller;
        public PlayerStateBase(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
        {
            Input = InputManager.Instance;
            this.controller = controller;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        
    }

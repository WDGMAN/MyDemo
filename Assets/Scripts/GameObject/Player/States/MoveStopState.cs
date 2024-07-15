
    using Animancer;
    using UnityEngine;
    using WDGFrame;

    public class MoveStopState_Player:PlayerStateBase
    {
        
        public AnimationClip walkStopAnimation;
        public AnimationClip jogStopAnimation;
        public AnimationClip runStopAnimation;
        private LinearMixerState _MixerState;
        private AnimancerState currentState;

        public MoveStopState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
        {
            walkStopAnimation = Resources.Load<AnimationClip>("Animation/Player/WalkStop");
            jogStopAnimation = Resources.Load<AnimationClip>("Animation/Player/JogStop");
            runStopAnimation = Resources.Load<AnimationClip>("Animation/Player/RunStop");
            _MixerState = new LinearMixerState();
            _MixerState.Add(walkStopAnimation, 1.4f);
            _MixerState.Add(jogStopAnimation, 2.7f);
            _MixerState.Add(runStopAnimation, 5.9f);

        }

        private bool flag;
        public override void Enter( )
        {

            flag = false;
            _MixerState.Parameter =fsm.GetShareData<float>("currentSpeed");
            currentState=controller.Animancer.Play(_MixerState,0.2f,FadeMode.FromStart);
            currentState.Events.Add(0.8f, () =>
            {
                {
                    fsm.ChangeState(PlayerStateEnum.Idle);
                }
            });

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

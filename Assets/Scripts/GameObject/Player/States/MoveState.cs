using Animancer;
using UnityEngine;
using WDGFrame;


    public class MoveState_Player :PlayerStateBase
    {

        public AnimationClip walkAnimation;
        public AnimationClip jogAnimation;
        public AnimationClip runAnimation;
        
    
        private LinearMixerState  _MixerPlay;
        private float walkSpeed = 1.4f;
        private float jogSpeed = 2.7f;
        private float runSpeed = 5.9f;
        private AnimancerState currentState;
        private AttributeBase attribute;

        private float currentSpeed;

      
        public MoveState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
        {
            walkAnimation = Resources.Load<AnimationClip>("Animation/Player/Walk");
            jogAnimation = Resources.Load<AnimationClip>("Animation/Player/Jog");
            runAnimation = Resources.Load<AnimationClip>("Animation/Player/Run");

      
            
            
            
            _MixerPlay = new LinearMixerState();
            _MixerPlay.Add(walkAnimation, walkSpeed);
            _MixerPlay.Add(jogAnimation, jogSpeed);
            _MixerPlay.Add(runAnimation, runSpeed);

            
            attribute = PlayerController.Instance.Attribute;
            
            fsm.AddShareData("currentSpeed");
        }

        public override void Enter()
        {
            currentState = controller.Animancer.Play(_MixerPlay, 0.2f);
            currentSpeed = 0;
        }

        

        public override void Update()
        {
            if (Input.WasPressedMove() == Vector2.zero)
            {
                fsm.ChangeState(PlayerStateEnum.MoveStop);
            }else if (Input.WasPressedJump())
            {
                fsm.ChangeState(PlayerStateEnum.Up);
            }else if (controller.IsFall)
            {
                fsm.ChangeState(PlayerStateEnum.Fall);
            }else if (Input.WasPressedSprint())
            {
                fsm.ChangeState(PlayerStateEnum.Sprint);
            }
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (currentSpeed < attribute.SpeedCurrent) currentSpeed +=   Time.fixedDeltaTime * 15f;
          
           PlayerController.Instance.PlayerMove( currentSpeed);
            //移动
            _MixerPlay.Parameter = currentSpeed;
        }


        public override void Exit()
        {
            fsm.SetShareData("currentSpeed",currentSpeed);
        }
    }

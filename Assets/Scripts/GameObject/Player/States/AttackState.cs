using UnityEngine;

    using WDGFrame;

    public class AttackState_Player:PlayerStateBase
    {
        private bool exitFlag;
        public PlayerStateMachine FSM { get; private set; }
        public AttackState_Player(StateMachine<PlayerStateEnum> fsm, PlayerController controller) : base(fsm, controller)
        {
            FSM = new PlayerStateMachine(fsm);
            
        }

        public override void Enter()
        {
            exitFlag = false;
            controller.SkillRelease.PlaySkill(Resources.Load<SkillConfig>("Config/SkillConfig/AttackSkillConfigCombo1"),()=>SkillEnd(),()=>AttackDetect(),()=>AttackDetectEnd(),(durationTime,enumVal,distance)=>Move(durationTime,enumVal,distance));
        }

        public override void Exit()
        {
            
        }

        public override void Update()
        {
            if (exitFlag)
            {
                if (Input.WasPressedMove()!=Vector2.zero)
                {
                    fsm.ChangeState(PlayerStateEnum.Move);
                }
            }
        }

        private void SkillEnd()
        {
            fsm.ChangeState(PlayerStateEnum.Idle);
        }

        private void AttackDetect()
        {
            
        }

        private void AttackDetectEnd()
        {
            exitFlag = true;
        }

        private void Move(float durationTime,MoveDirectionEnum moveDirectionEnum,float distance)
        {
                
            switch (moveDirectionEnum)
            {
                case MoveDirectionEnum.Forward:
                    float dis = distance / durationTime;
                    controller.MovePositionUpdate(controller.transform.forward*(dis*Time.deltaTime));
                    break;
            }
        }
        
    }

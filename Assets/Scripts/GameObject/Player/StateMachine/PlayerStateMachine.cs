namespace WDGFrame
{
    public enum PlayerStateEnum
    {
        Idle,
        Move,
        MoveStop,
        Up,
        JumpUp,
        JumpFall,
        Fall,
        Sprint,
        Land,
        Attack,
        Climb
    }
    public class PlayerStateMachine:StateMachine<PlayerStateEnum>
    {
        public PlayerStateMachine()
        {
            
        }
        
        public PlayerStateMachine(StateMachine<PlayerStateEnum> parentFSM):base(parentFSM)
        {
            
        }
    }
}
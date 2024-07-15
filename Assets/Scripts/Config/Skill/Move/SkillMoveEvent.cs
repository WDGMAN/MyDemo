    public enum MoveDirectionEnum
    {
        Up,
        Forward,
        Back
    }
    public class SkillMoveEvent
    {
        public int DurationFrameLength = 2;//持续时间长度
        public MoveDirectionEnum MoveDirection;//移动方向
        public float DisplacementDistance;//位移距离
    }

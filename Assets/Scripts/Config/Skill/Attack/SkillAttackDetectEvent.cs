
public enum SkillAttackDetectEnum
{
    Weapon,//武器检测
    Scope,//范围检测
}
public class SkillAttackDetectEvent
{
    public int DurationFrameLength=4; //检测长度
    public SkillAttackDetectEnum AttackDetect;//攻击检测形式
}
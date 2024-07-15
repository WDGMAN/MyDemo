using System;
using Animancer;
using Sirenix.Utilities.Editor;
using Unity.VisualScripting;
using UnityEngine;
using WDGFrame;

public class PlayerController : GameObjectController<PlayerStateEnum>
{
    public static PlayerController Instance { get; private set; }
    public Transform CameraTransform;
    public Climb Climb { get; private set; }
    public EquipmentSystem Equipment;
    public SkillRelease SkillRelease { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Animator.avatar = Resources.Load<Avatar>("GameObject/Player/PlayerAvatar");

        Instance = this;

        CameraManage.Instance.ThirdPerson(transform); //摄像机第三人称
        CameraTransform = CameraManage.Instance.Camera.transform;

        Attribute = _transform.AddComponent<PlayerAttribute>();
        Attribute.Init(100, 100, 100, 100, 6, AttributeSpecies.Normal);
        FSM = new PlayerStateMachine();
        InitState();
        GravityFlag = false;
        CharacterController.skinWidth = 0.0001f;
        CharacterController.enabled = false;
        transform.position = new Vector3(2, 2f, 1);
        CharacterController.enabled = true;

        Climb = _transform.AddComponent<Climb>();
        Equipment = new EquipmentSystem(_transform);
        SkillRelease = transform.AddComponent<SkillRelease>();
    }

    private void Start()
    {
        Climb.Init(_transform,CharacterController.radius,Animancer,Animator);
        GravityFlag = true;
        Equipment.SetEquipmentWeapon(Animator,Resources.Load<WeaponConfig>("Config/Weapon/WeaponSickle"));
    }

    //初始化状态
    private void InitState()
    {
        IdleState_Player idleState = new IdleState_Player(FSM,this);
        FSM.AddState(PlayerStateEnum.Idle, idleState);

        MoveState_Player moveState = new MoveState_Player(FSM,this);
        FSM.AddState(PlayerStateEnum.Move, moveState);

        MoveStopState_Player moveStop = new MoveStopState_Player(FSM,this);
        FSM.AddState(PlayerStateEnum.MoveStop, moveStop);

        UpState_Player upState = new UpState_Player(FSM, this);
        FSM.AddState(PlayerStateEnum.Up, upState);

        FallState_Player fallState = new FallState_Player(FSM, this);
        FSM.AddState(PlayerStateEnum.Fall, fallState);

        LandState_Player landState = new LandState_Player(FSM, this);
        FSM.AddState(PlayerStateEnum.Land, landState);

        SprintState_Player sprintState = new SprintState_Player(FSM, this);
        FSM.AddState(PlayerStateEnum.Sprint, sprintState);

        AttackState_Player attackState = new AttackState_Player(FSM, this);
        FSM.AddState(PlayerStateEnum.Attack, attackState);

        
        FSM.ChangeState(PlayerStateEnum.Idle);
    }

    /// <summary>
    /// 玩家移动 自带旋转
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void PlayerMove(float speed)
    {
        if(speed==0)return;
        Vector3 dir = new Vector3(InputManager.Instance.WasPressedMove().x, 0,
            InputManager.Instance.WasPressedMove().y);
        Quaternion targetRotation = GetCameraToForwardRotation(dir);
        Rotation(targetRotation);
        MoveFixed(_transform.forward, speed);
    }

    /// <summary>
    /// 玩家移动 
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="speed"></param>
    /// <param name="rotation"></param>
    /// <param name="rotationSpeed"></param>
    public void PlayerMove(Vector3 dir, float speed,Quaternion rotation,float rotationSpeed=8f)
    {
        Rotation(rotation,rotationSpeed);
        MoveFixed(dir,speed);
    }

    /// <summary>
    /// 获取玩家相当于摄像机前方的目标方向
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public Vector3 GetCameraToForwardDir(Vector3 dir)
    {
        Vector3 targetRotate = CameraTransform.TransformDirection(dir);
        targetRotate.y = 0;
        return targetRotate;
    }
    /// <summary>
    /// 获取玩家相当于摄像机前方的旋转
    /// </summary>
    public Quaternion GetCameraToForwardRotation(Vector3 dir)
    {
        Vector3 targetRotate = GetCameraToForwardDir(dir);
        Quaternion targetRotation = Quaternion.LookRotation(targetRotate);
        return targetRotation;
    }
    /// <summary>
    /// 获取目标方向的角度差
    /// </summary>
    /// <returns></returns>
    public float GetTargetDirAngle(Vector3 dir)
    {
        //根据摄像机的前方旋转
        Vector3 targetRotate = Quaternion.Euler(0, CameraTransform.rotation.eulerAngles.y,0) * dir;  
        // cameraTransform.TransformDirection(dir);
        targetRotate.y = 0;
        Quaternion targetAngle = Quaternion.LookRotation(targetRotate);

       
       float angle=  Quaternion.Angle(targetAngle, _transform.rotation);
        return angle;
    }
}
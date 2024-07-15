using System;
using Animancer;
using Unity.VisualScripting;
using UnityEngine;

namespace WDGFrame
{
    public class GameObjectController<T> : MonoBehaviour
    {
        public AttributeBase Attribute { get; protected set; }
        protected Transform _transform;
        public StateMachine<T> FSM { get; protected set; }

        public CharacterController CharacterController { get; protected set; }

        public Animator Animator { get; protected set; }
        public AnimancerComponent Animancer { get; protected set; }

        protected virtual void Awake()
        {
            _transform = transform;


            Animator = _transform.AddComponent<Animator>();
            Animancer = _transform.AddComponent<AnimancerComponent>();
            Animancer.Animator = Animator;

            CharacterController = _transform.AddComponent<CharacterController>();
            CharacterController.radius = 0.4f;
            CharacterController.height = 2f;
            CharacterController.center = new Vector3(0, 1, 0);
        }

        protected virtual void Update()
        {
            FSM.OnUpdate();
        }

        private void FixedUpdate()
        {
            DetectIsGround();
            detectIsFall();
            FSM.OnFixedUpdate();
            OnGravity();
            CharacterController.Move(moveVector3);
            moveVector3=Vector3.zero;
        }

        private void LateUpdate()
        {
            FSM.OnLateUpdate();
        }

        #region 重力

        public float GravityMagnitude = 12f; //重力大小
        private float gravityMax = 10f; //重力最大值
        private float gravitationalAcceleration = 0; //重力加速度

        private bool gravityFlag = false;

        public bool GravityFlag
        {
            get => gravityFlag;
            set
            {
                if (value == gravityFlag) return;
                if (value == false) gravitationalAcceleration = 0;
                gravityFlag = value;
            }
        } //重力开关


        private void OnGravity()
        {
            if (IsGround) return;
            if (!GravityFlag) return;
            if (gravitationalAcceleration < gravityMax)
                gravitationalAcceleration += GravityMagnitude * Time.fixedDeltaTime * Time.fixedDeltaTime;
            CharacterController.Move(Vector3.down * gravitationalAcceleration);
        }

        #endregion

        #region 检测着地

        private bool isGround;

        public bool IsGround
        {
            get => isGround;
            private set
            {
                if (value == isGround) return;
                if (value) gravitationalAcceleration = 0;
                isGround = value;
            }
        }

        private bool isGroundFlag = true;

    

        private RaycastHit footDownHit;
        private float rayCastOffset = 0.1f; //偏移量 解决bug
        public bool IsFall { get; private set; }

        private float fallTime=0f;
        private void detectIsFall()
        {
            if (!IsGround)
            {
                if (fallTime > 0.2f)
                {
                    IsFall = true;
                    fallTime = 0;
                }
                else
                {
                    IsFall = false;
                    fallTime += Time.fixedDeltaTime;
                }
            }
            else
            {
                fallTime = 0;
                IsFall = false;
            }
        }
        private void DetectIsGround()
        {
            if (Physics.SphereCast(transform.position + Vector3.up * (CharacterController.radius + rayCastOffset),
                    CharacterController.radius,
                    Vector3.down, out footDownHit, rayCastOffset * 2))
            {
                if (footDownHit.distance < rayCastOffset ||
                    PublicFunc.CompareFloatEqual(footDownHit.distance, rayCastOffset))
                {
                    IsGround = true;
                }
                else
                {
                    IsGround = false;
                }
            }
            else
            {
                IsGround = false;
            }
        }

        #endregion

        #region 移动方法

        private Vector3 moveVector3;

    
        
        private void Move(Vector3 vector3)
        {
            moveVector3 += vector3;
        }


        /// <summary>
        /// 物理移动
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        public void MoveFixed(Vector3 direction, float speed)
        {
            direction = calculateOnSlopeSpeed(direction, speed);
            Move(direction * Time.fixedDeltaTime);
        }

        /// <summary>
        /// 直接移动 不包爬坡 没有*Time
        /// </summary>
        public void MovePositionFixed(Vector3 vector3)
        {
            Move(vector3);
        }

        /// <summary>
        /// 直接移动
        /// </summary>
        public void MovePositionUpdate(Vector3 vector3)
        {
            CharacterController.Move(vector3);
        }
        /// <summary>
        /// 根据目标方向进行旋转
        /// </summary>
        /// <param name="targetRotate"></param>
        /// <param name="roationSpeed"></param>
        public void Rotation(Vector3 targetRotate, float roationSpeed = 8f)
        {
            if (targetRotate == Vector3.zero) return;
            targetRotate.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetRotate);
            Rotation(targetRotation, roationSpeed);
        }

        /// <summary>
        /// 旋转到指定四元数
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="rotationSpeed"></param>
        public void Rotation(Quaternion rotation,float rotationSpeed=8f)
        {
            _transform.rotation =
                Quaternion.Slerp(_transform.rotation, rotation, Time.fixedDeltaTime * rotationSpeed);
        }
        #endregion

        #region 爬坡

        private float maxSlopeAngle = 45f; //最大爬坡角度

        /// <summary>
        /// 计算上下坡的向量
        /// </summary>
        /// <returns></returns>
        private Vector3 calculateOnSlopeSpeed(Vector3 direction, float speed)
        {
            if (footDownHit.transform != null)
            {
                float angle = Vector3.Angle(footDownHit.normal, moveVector3);
                if (angle > 90f)
                {
                    float resultAngle = angle - 90f;
                    if (resultAngle > maxSlopeAngle)
                    {
                        speed = 0;
                    }
                    else
                    {
                        speed -= resultAngle * 0.01f * speed;
                    }
                }
                else if (angle < 90f)
                {
                    float resultAngle = 90f - angle;
                    //坡度太陡 只能滑下去不能操控
                    if (resultAngle > maxSlopeAngle)
                    {
                        //暂定
                    }

                    direction = Vector3.ProjectOnPlane(direction, footDownHit.normal).normalized;
                }
            }

            return speed * direction;
        }

        #endregion

        #region 下楼
        /// <summary>
        /// 下楼
        /// </summary>
        private void goDownStairs(Vector3 dir,float speed)
        {
            // if(Physics.Raycast(_transform.position+dir*speed))
        }
        

        #endregion
    }

    public class GameObjectController : MonoBehaviour
    {
    }
}
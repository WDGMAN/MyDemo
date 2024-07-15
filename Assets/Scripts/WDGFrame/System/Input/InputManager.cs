using UnityEngine;
using UnityEngine.InputSystem;

namespace WDGFrame
{
    public class InputManager : Singleton<InputManager>
    {
        private PlayerInput input;

        private InputManager()
        {
            input = new PlayerInput();
            input.Enable();
        }

        #region 玩家控制
        public bool WasPressedJump()
        {
            return input.Player.Jump.WasPressedThisFrame();
        }  
        public InputAction GetJump()
        {
            return input.Player.Jump;
        }

        
        public Vector2 WasPressedMove()
        {
            return input.Player.Move.ReadValue<Vector2>();
        }    
        public InputAction GetMove()
        {
            return input.Player.Move;
        }

        public bool WasPressedSprint()
        {
            return input.Player.Sprint.WasPressedThisFrame();
        }

        public InputAction GetSprint()
        {
            return input.Player.Sprint;
        }
        
        public bool WasPressedAttack()
        {
            return input.Player.Attack.WasPressedThisFrame();

        }  
        public InputAction GetAttack()
        {
            return input.Player.Attack;

        }

        
      
        public Vector2 GetMouseMoveVal()
        {
            return input.Player.MouseMove.ReadValue<Vector2>();
        } 
        public InputAction GetMouseMove()
        {
            return input.Player.MouseMove;
        }

        public Vector2 GetMouseScrollVal()
        {
            return input.Player.Scroll.ReadValue<Vector2>();
        }    
        public InputAction GetMouseScroll()
        {
            return input.Player.Scroll;
        }

        public void SetPlayerActive(bool flag)
        {
            if (flag)
            {
                input.Player.Enable();
            }
            else
            {
                input.Player.Disable();
            }
        }
        #endregion

        #region 摄像机控制

        public Vector2 GetCameraRotationVal()
        {
            return input.Camera.Rotation.ReadValue<Vector2>();

        } 
        public InputAction GetCameraRotation()
        {
            return input.Camera.Rotation;

        } 
        
        
        public Vector2 GetCameraDistanceVal()
        {
            return input.Camera.Distance.ReadValue<Vector2>();
        }
        public InputAction GetCameraDistance()
        {
            return input.Camera.Distance;
        }
        
        public void SetCameraActive(bool flag)
        {
            if (flag)
            {
                input.Camera.Enable();
            }
            else
            {
                input.Camera.Disable();
            }
        }
        #endregion

        public void SetActive(bool flag)
        {
            if(flag) input.Enable();
            else input.Disable();
           
        }
      
    }
}
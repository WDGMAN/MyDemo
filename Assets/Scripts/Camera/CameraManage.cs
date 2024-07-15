
using System;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using WDGFrame;

public class CameraManage:SingletonAutoMono<CameraManage>
{
    public GameObject Camera { get; private set; }


    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        //创建摄像机
        GameObject camera = new GameObject("MainCamera");
        camera=  Instantiate(camera);
        camera.AddComponent<Camera>();
        camera.AddComponent<AudioListener>();
        camera.AddComponent<CinemachineBrain>();
        camera.tag = "MainCamera";
        DontDestroyOnLoad(camera);
        Camera = camera;
    }
    
   public CinemachineFreeLook freeLook;
   
   /// <summary>
   /// 第三人称
   /// </summary>
   public void ThirdPerson(Transform player)
   {
      gameObject.AddComponent<CinemachineInputProvider>().enabled=false;
       freeLook=  gameObject.AddComponent<CinemachineFreeLook>();
       freeLook.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
       freeLook.m_Lens.FieldOfView = 40f;
      // freeLook.m_YAxis.m_InvertInput = false;
      // freeLook.m_XAxis.m_InvertInput = false;
      freeLook.Follow = player;
      freeLook.LookAt = player;
      // freeLook.LookAt = player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
      freeLook.m_Orbits[0].m_Height = 4.5f;
      freeLook.m_Orbits[0].m_Radius = 1.75f;
      freeLook.m_Orbits[1].m_Height = 2.5f;
      freeLook.m_Orbits[1].m_Radius = 3f;

      freeLook.m_Orbits[2].m_Height = 0.4f;
      freeLook.m_Orbits[2].m_Radius = 1.3f;
      
      //修改Y轴偏移量
      var topTransposer = freeLook.GetRig(0).GetCinemachineComponent<CinemachineComposer>();
      topTransposer.m_TrackedObjectOffset.y = 1.5f;
      var middleTranspose = freeLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
      middleTranspose.m_TrackedObjectOffset.y = 1.3f;
      var bottomTranspose = freeLook.GetRig(2).GetCinemachineComponent<CinemachineComposer>();
      bottomTranspose.m_TrackedObjectOffset.y = 1f;
      //修改damping
      var topDamping= freeLook.GetRig(0).GetCinemachineComponent<CinemachineOrbitalTransposer>();
      topDamping.m_YDamping = 0;
      topDamping.m_XDamping = 0;
      topDamping.m_ZDamping = 0;
      var middleDamping= freeLook.GetRig(1).GetCinemachineComponent<CinemachineOrbitalTransposer>();
      middleDamping.m_YDamping = 0;
      middleDamping.m_XDamping = 0;
      middleDamping.m_ZDamping = 0;
      var bottomDamping= freeLook.GetRig(2).GetCinemachineComponent<CinemachineOrbitalTransposer>();
      bottomDamping.m_YDamping = 0;
      bottomDamping.m_XDamping = 0;
      bottomDamping.m_ZDamping = 0;
      InputManager.Instance.GetCameraRotation().performed+= (obj) =>
      {
          Vector2 v2 = obj.ReadValue<Vector2>();
          freeLook.m_YAxis.Value -= v2.y*0.001f;
          freeLook.m_XAxis.Value += v2.x*0.1f;
      };

      InputManager.Instance.GetCameraDistance().performed+= (obj) =>
      {
          float val = obj.ReadValue<Vector2>().y / 120f;
          freeLook.m_Lens.FieldOfView = Mathf.Clamp(freeLook.m_Lens.FieldOfView += val, 40f, 60f);
      };
   }

   private void Update()
   {
  
   }
}
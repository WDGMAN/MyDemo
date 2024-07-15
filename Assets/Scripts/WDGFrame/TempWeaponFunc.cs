
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;

    [ExecuteInEditMode]
    public class TempWeaponFunc:MonoBehaviour
    {

        private WeaponConfig weapon;

        [ContextMenu("SetWeapon")]
        public void TestFunc()
        {
            if (weapon == null)
            {
                Animator animator = GetComponent<Animator>();
                weapon = Resources.Load<WeaponConfig>("Config/Weapon/WeaponSickle");
                   GameObject obj= Instantiate(weapon.Weapon,animator.GetBoneTransform(HumanBodyBones.RightHand));
                   if(weapon.OffsetVector3==Vector3.zero)return;
                    obj.transform.localPosition=weapon.OffsetVector3;
                    obj.transform.rotation =Quaternion.Euler(weapon.OffsetRotation);
            }
               
            
        }
    }

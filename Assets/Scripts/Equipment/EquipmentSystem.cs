
    using UnityEngine;

    public class EquipmentSystem
    {
        public EquipmentSystem(Transform Own)
        {
            this.Own = Own;
        }
        public Transform Own;
        public EquipmentItem EquipmentItem=new EquipmentItem();

        
        
        public void SetEquipmentWeapon(Animator animator,WeaponConfig weaponConfig )
        {
           Transform pos= animator.GetBoneTransform(HumanBodyBones.RightHand);
           GameObject obj = GameObject.Instantiate(weaponConfig.Weapon, pos);
           obj.transform.localPosition = weaponConfig.OffsetVector3;
           obj.transform.localRotation=Quaternion.Euler(weaponConfig.OffsetRotation);
           AttackDetectionCapsule attackDetectionCapsule= obj.AddComponent<AttackDetectionCapsule>();
           attackDetectionCapsule.WeaponTransform = obj.transform;
           attackDetectionCapsule.WeaponDirection = weaponConfig.WeaponDirection;
           attackDetectionCapsule.WeaponLength = weaponConfig.WeaponLength;
           attackDetectionCapsule.Radius = weaponConfig.WeaponRadius;
           EquipmentItem.EquipWeapon(obj);
        }
    }

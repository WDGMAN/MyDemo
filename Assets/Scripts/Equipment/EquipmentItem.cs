
    using UnityEngine;

    public class EquipmentItem
    {
        private EquipmentWeapon weapon=new EquipmentWeapon();

        public EquipmentWeapon Weapon
        {
            get => weapon;
            private set
            {
                weapon = value;
            }
        }


        public void EquipWeapon(GameObject waponObj)
        {
            Weapon.Weapon = waponObj;
        }
        
        
    }


    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Config/Weapon",fileName = "WeaponSkill")]

    public class WeaponConfig : SerializedScriptableObject
    {
        [LabelText("武器名称")]public string WeaponName;
        [LabelText("手握偏移位置")] public Vector3 OffsetVector3;
        [LabelText("手握偏移旋转")] public Vector3 OffsetRotation;
        [LabelText("武器预制体")] public GameObject Weapon;
        [LabelText("武器方向")] public Vector3 WeaponDirection;
        [LabelText("武器长度")] public float WeaponLength;
        [LabelText("武器半径")] public float WeaponRadius;

        
        

    }

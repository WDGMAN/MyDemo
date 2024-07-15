using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionCapsule : AttackDetection
{
   

    public Transform WeaponTransform; //武器
    public Vector3 WeaponDirection; //武器方向
    public float WeaponLength; //武器长度
    public float Radius;//武器半径

    public bool debugDraw;

    private void OnDrawGizmos()
    {
        if (debugDraw && WeaponTransform != null)
        {
            

            Gizmos.color = Color.yellow;

            Vector3 pos = WeaponTransform.position;
            Vector3 dir = WeaponTransform.rotation * WeaponDirection;
            Vector3 radius = dir * Radius;
            Vector3 startPoint= pos+radius;
            Vector3 endPoint = pos +dir* WeaponLength-radius;
            
            Gizmos.DrawWireSphere(startPoint,Radius);
            Gizmos.DrawWireSphere(endPoint,Radius);

            Vector3 perpendicular = Vector3.Cross(dir, Vector3.up);
             
            Gizmos.DrawLine(startPoint+perpendicular*Radius,endPoint+perpendicular*Radius);
            Gizmos.DrawLine(startPoint-perpendicular*Radius,endPoint-perpendicular*Radius);

            perpendicular = Vector3.Cross(perpendicular, dir);
            Gizmos.DrawLine(startPoint+perpendicular*Radius,endPoint+perpendicular*Radius);
            Gizmos.DrawLine(startPoint-perpendicular*Radius,endPoint-perpendicular*Radius);
        }
    }


    private List<GameObject> waHits=new List<GameObject>();
    public override List<GameObject> Detection()
    {
        waHits.Clear();
        Vector3 pos = WeaponTransform.position;
        Vector3 dir = WeaponTransform.rotation * WeaponDirection;
        Vector3 radius = dir * Radius;
        Vector3 startPoint= pos+radius;
        Vector3 endPoint = pos +dir* WeaponLength-radius;
        Collider[] hits = Physics.OverlapCapsule(startPoint, endPoint, Radius);
        foreach (Collider item in hits)
        {
            if (item.TryGetComponent(out AttributeBase attributeBase) )
            {
                waHits.Add(item.gameObject);
            }
        }
        
        return waHits;
    }
}
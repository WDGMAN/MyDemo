
using System;
using System.Collections.Generic;
using UnityEngine;


    public abstract class AttackDetection:MonoBehaviour
    {
        
        // public List<GameObject> WasHit { get; private set; }

        private void Awake()
        {
            // WasHit = new List<GameObject>();
        }

        // public void ClearWasHit()
        // {
        //     WasHit.Clear();
        // }

        public abstract List<GameObject> Detection();

    }

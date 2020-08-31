/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using System.Collections;

namespace PyroTechnix
{
    public class DestroyOnExplosion : MonoBehaviour, IExplosionReactive
    {
        public float delay;
        private bool done;

        public void OnExplosionHit(Vector3 explosionPosition)
        {
            if (!done)
            {
                Destroy(gameObject, delay);
                done = true;
            }
        }

        public void OnExplosionForceHit(Vector4 explosionPosition_force)
        {

        }

        public void OnCriticalExplosionHit(Vector3 explosionPosition)
        {

        }
    }
}
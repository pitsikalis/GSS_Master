/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using System.Collections;

namespace PyroTechnix
{
    public class ExplosionKinematics : MonoBehaviour, IExplosionModule
    {
        public float upwardsForce;
        public float drag;

        private Vector3 velocity;
        private ExplosionSystem parent;

        #region IExplosionModule
        void IExplosionModule.OnExplode(ExplosionSystem parent)
        {
            this.parent = parent;
        }
        #endregion

        private void Awake()
        {
            velocity = -Physics.gravity * upwardsForce;
        }

        private void Update()
        {
            if (parent != null)
            {
                velocity *= drag;
                velocity -= Physics.gravity * Time.smoothDeltaTime;
                transform.position += velocity * Time.smoothDeltaTime * (1 - parent.NormalizedLife);
            }
        }
    }
}
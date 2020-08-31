/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using System.Collections;
using UnityEngine;

namespace PyroTechnix
{
    public class ExplosionFX : MonoBehaviour, IExplosionModule
    {
        public float delay;
        public ParticleSystem fxToSpawn;
        public bool parent;

        #region IExplosionModule
        void IExplosionModule.OnExplode(ExplosionSystem parent)
        {
            if (fxToSpawn != null)
            {
                StartCoroutine(SpawnFX());
            }
        }
        #endregion

        private IEnumerator SpawnFX()
        {
            yield return new WaitForSeconds(delay);

            ParticleSystem fx = Instantiate(fxToSpawn, transform.position, transform.rotation) as ParticleSystem;

            if (fx != null)
            {
                if (parent)
                {
                    fx.transform.parent = transform;
                }
            }
        }
    }
}
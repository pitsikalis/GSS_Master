/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using System.Collections;

namespace PyroTechnix
{
    public class SpawnOnExplosion : MonoBehaviour, IExplosionReactive
    {
        public GameObject objectToSpawn;
        public float delay;

        private bool done;

        public void OnExplosionHit(Vector3 explosionPosition)
        {
            if (!done)
            {
                StartCoroutine(Timer());
                done = true;
            }
        }

        public void OnExplosionForceHit(Vector4 explosionPosition_force)
        {

        }

        public void OnCriticalExplosionHit(Vector3 explosionPosition)
        {

        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(delay);
            Instantiate(objectToSpawn, transform.position, transform.rotation);
            Destroy(this);
        }
    }
}
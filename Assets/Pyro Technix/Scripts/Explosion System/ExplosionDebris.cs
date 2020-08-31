/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using System.Collections;

namespace PyroTechnix
{
    public sealed class ExplosionDebris : MonoBehaviour, IExplosionModule
    {
        public Rigidbody[] debrisList;
        public int minDebrisToSpawn = 1;
        public int maxDebrisToSpawn = 10;
        public float explosiveForce = 1000;
        public float minLifetime = 1;
        public float maxLifetime = 5;

        #region IExplosionModule
        void IExplosionModule.OnExplode(ExplosionSystem parent)
        {
            int amount = Random.Range(minDebrisToSpawn, maxDebrisToSpawn);

            for (int i = 0; i < amount; i++)
            {
                Rigidbody debris = Instantiate(Helpers.RandomFromArray(debrisList), transform.position + Random.onUnitSphere, Random.rotation) as Rigidbody;
                debris.AddExplosionForce(explosiveForce, transform.position, 10);

                float expectedLife = Random.Range(minLifetime, maxLifetime);
                Destroy(debris.gameObject, expectedLife);
            }
        }

        #endregion
    }
}
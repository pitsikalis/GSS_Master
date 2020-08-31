/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using System.Collections;

namespace PyroTechnix
{
    public class ExplosionSystem : MonoBehaviour, IExplosionModule
    {
        public PyroTechnixExplosion explosionPrefab;
        public float minLife = 1;
        public float maxLife = 4;

        private float lifeLived;
        private float lifeExpectancy;

        private GameObject explosionObject;
        private PyroTechnixExplosion explosionInstance;

        public float NormalizedLife { get { return lifeLived / lifeExpectancy; } }

        #region IExplosionModule
        public void OnExplode(ExplosionSystem me)
        {
            lifeExpectancy = Random.Range(minLife, maxLife);

            if (explosionPrefab != null)
            {
                explosionInstance = Instantiate(explosionPrefab, transform.position, transform.rotation) as PyroTechnixExplosion;
                explosionInstance.transform.parent = transform;
            }

            Destroy(gameObject, lifeExpectancy);

            foreach (IExplosionModule module in gameObject.GetInterfaces<IExplosionModule>())
            {
                if (module == this) continue;

                module.OnExplode(this);
            }
        }
        #endregion

        private void Awake()
        {
            OnExplode(this);
        }

        private void LateUpdate()
        {
            lifeLived += Time.deltaTime;
        }
    }
}
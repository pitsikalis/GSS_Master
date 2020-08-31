/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using System.Collections;
using UnityEngine;

namespace PyroTechnix
{
    public class ExplosionLight : MonoBehaviour, IExplosionModule
    {
        public float lightIntensity = 2;
        public float lightFalloffTime = 0.2f;
        public Color lightColour = Color.white;
        public float lightRadius = 5;

        private Light explosionLight;

        #region IExplosionModule
        void IExplosionModule.OnExplode(ExplosionSystem parent)
        {
            GameObject explosionLightObject = new GameObject("light");
            explosionLightObject.transform.parent = transform;
            explosionLightObject.transform.localPosition = Vector3.zero;
            explosionLightObject.transform.localRotation = Quaternion.identity;

            explosionLight = explosionLightObject.AddComponent<Light>();
            explosionLight.type = LightType.Point;
            explosionLight.color = lightColour;
            explosionLight.range = lightRadius;

            StartCoroutine(DoLightingFX());
        }
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = lightColour;
            Gizmos.DrawWireSphere(transform.position, lightRadius);
        }

        private IEnumerator DoLightingFX()
        {
            explosionLight.intensity = lightIntensity;

            float t = 0;

            while (t < lightFalloffTime)
            {
                explosionLight.intensity = Mathf.Lerp(lightIntensity, 0, t / lightFalloffTime);
                t += Time.deltaTime;
                yield return null;
            }

            Destroy(explosionLight.gameObject);
        }
    }
}
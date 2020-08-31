/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;

namespace PyroTechnix
{
    public class ExplosionDecal : MonoBehaviour, IExplosionModule
    {
        public PyroTechnixDecal decal;
        public float fadeDuration = 2;

        private PyroTechnixDecal instance;
        private float initialOpacity;
        private float timer;

        #region IExplosionModule
        void IExplosionModule.OnExplode(ExplosionSystem parent)
        {
            if (decal != null)
            {
                instance = Instantiate(decal, transform.position, transform.rotation) as PyroTechnixDecal;
                initialOpacity = instance.Opacity;
                timer = 0;
                Destroy(instance.gameObject, fadeDuration);
            }
        }
        #endregion

        private void Update()
        {
            if (instance != null)
            {
                instance.GetComponent<Renderer>().sortingOrder = 0;
                instance.Opacity = Mathf.Lerp(initialOpacity, 0, timer / fadeDuration);
                timer += Time.deltaTime;
            }
        }
    }
}
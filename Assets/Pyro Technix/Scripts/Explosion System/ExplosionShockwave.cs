/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;

namespace PyroTechnix
{
    public class ExplosionShockwave : MonoBehaviour, IExplosionModule
    {
        public Shader shockwaveShader;

        [SerializeField] float duration = 0.3f;
        [SerializeField] float initialRadius = 1.0f;
        [SerializeField] float finalRadius = 10.0f;
        [SerializeField] float strength = 2.0f;

        private GameObject shockwave;
        private Material material;
        private float timer;

        private static Mesh meshInstance;

        #region IExplosionModule
        void IExplosionModule.OnExplode(ExplosionSystem parent)
        {
            if (shockwaveShader == null)
            {
                shockwaveShader = Shader.Find("Hidden/PyroTechnix/Shockwave");
            }

            if (meshInstance == null)
            {
                meshInstance = Helpers.CreateRing(40);
            }

            shockwave = new GameObject("Shockwave");
            shockwave.transform.parent = transform;
            shockwave.transform.localPosition = Vector3.zero;
            shockwave.transform.localRotation = Quaternion.identity;

            shockwave.AddComponent<MeshFilter>().sharedMesh = meshInstance;
            shockwave.AddComponent<MeshRenderer>().sharedMaterial = material = new Material(shockwaveShader);
            shockwave.AddComponent<ShockwaveInstance>();

            Destroy(shockwave, duration);
        }
        #endregion

        private void Update()
        {
            if (shockwave == null) return;

            float nTime = timer / duration;
            float currentSize = Mathf.Lerp(initialRadius, finalRadius, nTime);

            material.SetFloat("_Strength", (1 - nTime) * strength);
            material.SetVector("_Position_Radius", new Vector4(transform.position.x, transform.position.y, transform.position.z, currentSize));
            
            timer += Time.deltaTime;
        }
    }
}
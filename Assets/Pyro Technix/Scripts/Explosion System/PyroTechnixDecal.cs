/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using System.Collections;

namespace PyroTechnix
{
    [ExecuteInEditMode]
    public class PyroTechnixDecal : MonoBehaviour
    {
        private const int kNoiseDimensions = 32;
        private const float kRecipNoiseDimensions = 1.0f / kNoiseDimensions;

        public float uvScale = 1;
        public float uvBias;
        public float noiseFrequencyFactor = 1.5f;
        public float noiseAmplitudeFactor = 0.5f;
        public float displacementWS = 0.5f;
        public float noiseScale = 0.05f;
        public float edgeSoftness = 1.0f;
        public float opacity = 1.0f;

        private static Mesh mesh;
        private Material material;

        public float UvScale                { get { return uvScale; }               set { uvScale = value; }    }
        public float UvBias                 { get { return uvBias; }                set { uvBias = value; }     }
        public float NoiseFrequencyFactor   { get { return noiseFrequencyFactor; }  set { noiseFrequencyFactor = value; } }
        public float NoiseAmplitudeFactor   { get { return noiseAmplitudeFactor; }  set { noiseAmplitudeFactor = value; } }
        public float NoiseScale             { get { return noiseScale; }            set { noiseScale = value; } }
        public float EdgeSoftness           { get { return edgeSoftness; }          set { edgeSoftness = value; } }
        public float Opacity                { get { return opacity; }               set { opacity = value; } }
             
        private float DiameterWS            { get { return RadiusWS * 2; } }
        private float RadiusWS              { get { return transform.lossyScale.x; } }
        private float InnerRadiusWS         { get { return RadiusWS - displacementWS; } }
        private float DisplacementWS        { get { return displacementWS; } }
        private Vector4 PositionRadiusWS    { get { return new Vector4(transform.position.x, transform.position.y, transform.position.z, RadiusWS); } }
        private Vector2 UvScaleBias         { get { return new Vector2(uvScale, uvBias); } }
        private Vector3 EyeCentricPosition  { get { return Camera.current.transform.position - transform.position; } }

        private void Awake()
        {
            {
                CreateRenderingComponents();
                CreateNoiseVolume();
            }
        }

        private void Update()
        {
            transform.rotation = Quaternion.identity;

            if (material != null)
            {
                material.renderQueue = 1;

                material.SetFloat("_RadiusSqrWS", RadiusWS * RadiusWS);
                material.SetFloat("_EdgeSoftness", EdgeSoftness);
                material.SetFloat("_NoiseScale", NoiseScale);
                material.SetFloat("_Opacity", Mathf.Clamp01(opacity));
                material.SetFloat("_InnerRadiusWS", InnerRadiusWS);
                material.SetFloat("_DisplacementWS", DisplacementWS);
                material.SetFloat("_NoiseAmplitudeFactor", NoiseAmplitudeFactor);
                material.SetFloat("_NoiseFrequencyFactor", NoiseFrequencyFactor);
                material.SetVector("_PositionWS_RadiusWS", PositionRadiusWS);
                material.SetVector("_UvScaleBias", UvScaleBias);
                material.SetTexture("_NoiseVolume", PyroTechnixExplosion.NoiseVolume);
            }
        }
	
        private void OnWillRenderObject()
        {
            Camera.current.depthTextureMode |= DepthTextureMode.Depth;

            material.SetMatrix("_Camera2World", Camera.current.transform.localToWorldMatrix);
            material.SetVector("_EyeRelativePositionWS", EyeCentricPosition);
        }

        #region Aux

        private void CreateRenderingComponents()
        {
            if (material == null)
            {
                material = new Material(Shader.Find("Hidden/PyroTechnix/Decal"));
            }

            if (mesh == null)
            {
                mesh = Helpers.CreatePlane(20);
            }

            MeshRenderer renderer = (GetComponent<MeshRenderer>() == null ? gameObject.AddComponent<MeshRenderer>() : GetComponent<MeshRenderer>());
            renderer.sharedMaterial = material;

            MeshFilter filter = (GetComponent<MeshFilter>() == null ? gameObject.AddComponent<MeshFilter>() : GetComponent<MeshFilter>());
            if (filter.sharedMesh == null)
            {
                filter.sharedMesh = mesh;
            }
        }

        private void CreateNoiseVolume()
        {
            if (PyroTechnixExplosion.NoiseVolume == null)
            {
                PyroTechnixExplosion.GenerateNoiseTexture();
            }
        }

        static float[] EncodeFloatRGBA(float val)
        {
            //Thanks to karljj1 for this function
            float[] kEncodeMul = new float[] { 1.0f, 255.0f, 65025.0f, 160581375.0f };
            float kEncodeBit = 1.0f / 255.0f;
            for (int i = 0; i < kEncodeMul.Length; ++i)
            {
                kEncodeMul[i] *= val;
                // Frac
                kEncodeMul[i] = (float)(kEncodeMul[i] - System.Math.Truncate(kEncodeMul[i]));
            }

            // enc -= enc.yzww * kEncodeBit;
            float[] yzww = new float[] { kEncodeMul[1], kEncodeMul[2], kEncodeMul[3], kEncodeMul[3] };
            for (int i = 0; i < kEncodeMul.Length; ++i)
            {
                kEncodeMul[i] -= yzww[i] * kEncodeBit;
            }

            return kEncodeMul;
        }

        #endregion
    }
}
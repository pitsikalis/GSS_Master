/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using System.Collections.Generic;

namespace PyroTechnix
{
    public enum PrimitiveType
    {
        Sphere = 0,
        Cone,
        Cylinder,
        Torus,
        Box
    }
    [ExecuteInEditMode]
    public class PyroTechnixExplosion : MonoBehaviour 
    {
        public Texture noiseReference;
        
        public static Texture NoiseVolume;
		private static float MinNoiseValue, MaxNoiseValue;
		private const int kNoiseDimensions = 32;
        private const float kRecipNoiseDimensions = 1.0f / kNoiseDimensions;

		// Paired with defines in the shaders.
		private const int kNumOctaves = 4;
		private const int kNumHullOctaves = 2;

		// DEBUGGING
		// Account for under-tessellation in the base mesh - magic number.
		public float kSkinThicknessBias = 0.03f;
		public bool forceBackOnly;
		// *********

        public PrimitiveType primitiveType;
        public Gradient colourGradient;
        public float uvScale = 1;
        public float uvBias;
        public int maxNumSteps = 50;
        public float noiseFrequencyFactor = 1.5f;
        public float noiseAmplitudeFactor = 0.5f;
        public Vector3 noiseAnimationSpeed = Vector3.one;
        public float displacementWS = 0.5f;
        public float noiseScale = 0.05f;
        public float edgeSoftness = 1.0f;
        public bool useLighting;
        public float density = 1.0f;
        public float opacity = 1.0f;
        public Light directionalLight;

        public bool isAxisAligned = true;

		private float skinThickness;


        [HideInInspector] private Texture2D colourGradientTex;
        [HideInInspector] private Color[] gradientTextureColours;

        public static Mesh mesh;
		public Material pyroMaterial;
        private bool isGradientDirty = true;
        private Material material { get { return GetComponent<Renderer>().sharedMaterial; } set { GetComponent<Renderer>().sharedMaterial = value; } }

        private static readonly Dictionary<PrimitiveType, string> keywordMap = new Dictionary<PrimitiveType, string>()
        {
            { PrimitiveType.Sphere,     "USE_SPHERE" },
            { PrimitiveType.Cone,       "USE_CONE" },
            { PrimitiveType.Cylinder,   "USE_CYLINDER" },
            { PrimitiveType.Torus,      "USE_TORUS" },
            { PrimitiveType.Box,        "USE_BOX" },
        };  

        public Gradient ColourGradient
        {
            get
            {
                isGradientDirty = true;
                return colourGradient;
            }
            set
            {
                isGradientDirty = true;
                colourGradient = value;
            }
        }

        public float UvScale                { get { return uvScale; }               set { uvScale = value; }    }
        public float UvBias                 { get { return uvBias; }                set { uvBias = value; }     }
        public int MaxNumSteps              { get { return maxNumSteps; }           set { maxNumSteps = value; }}
        public float NoiseFrequencyFactor   { get { return noiseFrequencyFactor; }  set { noiseFrequencyFactor = value; } }
        public float NoiseAmplitudeFactor   { get { return noiseAmplitudeFactor; }  set { noiseAmplitudeFactor = value; } }
        public float NoiseScale             { get { return noiseScale; }            set { noiseScale = value; } }
        public float EdgeSoftness           { get { return edgeSoftness; }          set { edgeSoftness = value; } }
        public Vector3 NoiseAnimationSpeed  { get { return noiseAnimationSpeed; }   set { noiseAnimationSpeed = value; } }
        public float Opacity                { get { return opacity; }               set { opacity = value; } }

        public float SkinThickness         { get { return skinThickness; } }
        public float StepSizeWS            { get { return DiameterWS/MaxNumSteps; } }
        public float DiameterWS            { get { return RadiusWS * 2; } }
        public float RadiusWS              { get { return transform.lossyScale.x; } }
        public float InnerRadiusWS         { get { return RadiusWS - displacementWS; } }
        public float DisplacementWS        { get { return displacementWS; } }
        public float LightingDensity       { get { return density; } }
        public Vector4 PositionRadiusWS    { get { return new Vector4(transform.position.x, transform.position.y, transform.position.z, RadiusWS); } }
        public Vector2 UvScaleBias         { get { return new Vector2(uvScale, uvBias); } }
        public Vector3 EyeCentricPosition  { get { return Camera.current.transform.position - transform.position; } }
        public Vector3 Scale               { get { return transform.lossyScale; } }

        private void Awake()
        {
            CreateRenderingComponents();
            GenerateNoiseTexture();

            noiseReference = NoiseVolume;

			material = pyroMaterial;
        }

        private void Update()
        {
            if (isAxisAligned)
            {
                transform.rotation = Quaternion.identity;
            }

            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.x);

            UpdateColourGradient();

			UpdateSkinThickness();
        }
	
        private void OnWillRenderObject()
        {
            Camera.current.depthTextureMode |= DepthTextureMode.Depth;

			bool renderingBackMaterial = forceBackOnly || IsPointInside(Camera.current.transform.position);

			material.EnableKeyword( renderingBackMaterial  ? "BACK_RENDERING" : "FRONT_RENDERING");
			material.DisableKeyword(!renderingBackMaterial ? "BACK_RENDERING" : "FRONT_RENDERING");

			material.SetFloat("_SkinThickness", skinThickness + kSkinThicknessBias);
			material.SetFloat("_RadiusSqrWS", RadiusWS * RadiusWS);
            material.SetFloat("_StepSizeWS", StepSizeWS);
            material.SetFloat("_MaxNumSteps", MaxNumSteps);
            material.SetFloat("_EdgeSoftness", -EdgeSoftness);
            material.SetFloat("_NoiseScale", NoiseScale);
            material.SetFloat("_Opacity", Mathf.Clamp01(opacity));
            material.SetFloat("_InnerRadiusWS", InnerRadiusWS);
            material.SetFloat("_DisplacementWS", DisplacementWS);
            material.SetFloat("_NoiseAmplitudeFactor", NoiseAmplitudeFactor);
            material.SetFloat("_NoiseFrequencyFactor", NoiseFrequencyFactor);
			material.SetVector("_EyeForwardWS", Camera.current.transform.forward);
            material.SetVector("_Scale", Scale);
            material.SetVector("_NoiseAnimationSpeed", NoiseAnimationSpeed);
            material.SetVector("_PositionWS_RadiusWS", PositionRadiusWS);
            material.SetVector("_UvScaleBias", UvScaleBias);
            material.SetTexture("_GradientTex", colourGradientTex);
            material.SetTexture("_NoiseVolume", NoiseVolume);

            if (useLighting && directionalLight == null)
            {
                directionalLight = SunlightHelper.Instance;
            }

            bool canUseLighting = useLighting && directionalLight != null;

            material.EnableKeyword( isAxisAligned  ? "ENABLE_AXIS_ALIGNED" : "DISABLE_AXIS_ALIGNED");
            material.DisableKeyword(!isAxisAligned ? "ENABLE_AXIS_ALIGNED" : "DISABLE_AXIS_ALIGNED");

            material.EnableKeyword( canUseLighting  ? "ENABLE_LIGHTING" : "DISABLE_LIGHTING");
            material.DisableKeyword(!canUseLighting ? "ENABLE_LIGHTING" : "DISABLE_LIGHTING");

            foreach (var pair in keywordMap)
            {
                if (pair.Key == primitiveType)  material.EnableKeyword(  pair.Value );
                else                            material.DisableKeyword( pair.Value );
            }

            if (canUseLighting)
            {
                material.SetVector("_SunLightDir", directionalLight.transform.forward);
                material.SetFloat("_LightingDensity", LightingDensity);
            }

            material.SetVector("_EyeRelativePositionWS", EyeCentricPosition);
            material.SetMatrix("_Camera2World", Camera.current.transform.localToWorldMatrix);
            material.SetMatrix("_World2Local", Matrix4x4.TRS(Vector3.zero, Quaternion.Inverse(transform.rotation), Vector3.one));
        }

        #region Aux
        private bool IsPointInside(Vector3 point)
        {
            return (transform.position - point).sqrMagnitude < RadiusWS * RadiusWS;
        }

        private void CreateRenderingComponents()
        {
            if (pyroMaterial == null)
            {
				pyroMaterial = new Material(Shader.Find("Hidden/PyroTechnix/Render"));
            }

            if (mesh == null)
            {
                mesh = Helpers.CreatePlane(50);
            }

            if (GetComponent<MeshRenderer>() == null)
            {
                gameObject.AddComponent<MeshRenderer>();
            }

            MeshFilter filter = (GetComponent<MeshFilter>() == null ? gameObject.AddComponent<MeshFilter>() : GetComponent<MeshFilter>());
            if (filter.sharedMesh == null)
            {
                filter.sharedMesh = mesh;
            }
        }

		private void UpdateSkinThickness()
		{
			float largestAbsoluteNoiseValue = 1;// Mathf.Max(Mathf.Abs(MinNoiseValue), Mathf.Abs(MaxNoiseValue));

			// Calculate the skin thickness, which is amount of displacement to add 
			//  to the geometry hull after shrinking it around the explosion primitive.
			skinThickness = 0;
			for (int i = kNumHullOctaves; i < kNumOctaves; i++)
			{
				skinThickness += largestAbsoluteNoiseValue * 0.4f * Mathf.Pow(NoiseAmplitudeFactor, (float)i);
			}
			// Add a little bit extra to account for under-tessellation.  This should be
			//  fine tuned on a per use basis for best performance.
			skinThickness += kSkinThicknessBias;
		}

        public static void GenerateNoiseTexture()
        {
            if (NoiseVolume == null)
			{
				float pi2 = Mathf.PI * 2;
				float scale = 10.0f;
				float scaleOverPi2 = scale / pi2;

				MinNoiseValue = float.MaxValue;
				MaxNoiseValue = -MinNoiseValue;

				float[] noiseValues = new float[kNoiseDimensions * kNoiseDimensions * kNoiseDimensions];
				Color[] c = new Color[noiseValues.Length];

				// Calculated noise volume.
				{
					int i = 0;
					for (int x = 0; x < kNoiseDimensions; x++)
					{
						for (int y = 0; y < kNoiseDimensions; y++)
						{
							for (int z = 0; z < kNoiseDimensions; z++)
							{
								float px = x * kRecipNoiseDimensions;
								float py = y * kRecipNoiseDimensions;
								float pz = z * kRecipNoiseDimensions;

								float nx = Mathf.Cos(px * pi2) * scaleOverPi2;
								float ny = Mathf.Sin(px * pi2) * scaleOverPi2;
								float nz = Mathf.Cos(py * pi2) * scaleOverPi2;
								float nw = Mathf.Sin(py * pi2) * scaleOverPi2;
								float nu = Mathf.Cos(pz * pi2) * scaleOverPi2;
								float nv = Mathf.Sin(pz * pi2) * scaleOverPi2;
								float noise = NoiseUtility.Simplex6D(nx, ny, nz, nw, nu, nv, 0);

								MaxNoiseValue = Mathf.Max(MaxNoiseValue, noise);
								MinNoiseValue = Mathf.Min(MinNoiseValue, noise);

								noiseValues[i++] = noise;
							}
						}
					}

					float recipNoiseRange = 1.0f / (MaxNoiseValue - MinNoiseValue);
					float minNoiseOverRange = -MinNoiseValue * recipNoiseRange;
					for (i = 0; i < c.Length; i++)
					{
						float noise = noiseValues[i] *recipNoiseRange + minNoiseOverRange;
						if (noise >= 1.0f) noise = 0.9999999f;

						float[] farray = EncodeFloatRGBA(noise);
						c[i] = new Color(farray[0], farray[1], farray[2], farray[3]);
					}
				}

				Texture3D volume = new Texture3D(kNoiseDimensions, kNoiseDimensions, kNoiseDimensions, TextureFormat.ARGB32, false);

				volume.SetPixels(c);
				volume.Apply();

				NoiseVolume = volume;
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


        /// <summary>
        /// Sets the is dirty flag, forcing a rebuild of the gradient texture.
        /// </summary>
        public void SetGradientDirty()
        {
            isGradientDirty = true;
        }

        /// <summary>
        /// Sets the colour gradient texture from the gradients set in the editor.
        /// </summary>
        private void UpdateColourGradient()
        {
            if (!isGradientDirty) return;

            const int gradientResolution = 64;

            if (colourGradientTex == null || colourGradientTex.width != gradientResolution)
            {
                colourGradientTex = new Texture2D(gradientResolution, 1, TextureFormat.RGBA32, false);
                colourGradientTex.wrapMode = TextureWrapMode.Clamp;
                colourGradientTex.filterMode = FilterMode.Bilinear;
            }

            int desiredArraySize = gradientResolution;
            if (gradientTextureColours == null || gradientTextureColours.Length != desiredArraySize)
            {
                gradientTextureColours = new Color[desiredArraySize];
            }

            int k = 0;
            for (float i = 0; i < gradientResolution; i++)
            {
                gradientTextureColours[k++] = colourGradient.Evaluate(1.0f - i / gradientResolution);
            }

            colourGradientTex.SetPixels(gradientTextureColours);
            colourGradientTex.Apply(false);
        }

        #endregion
    }
}
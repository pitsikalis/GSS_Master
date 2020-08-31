// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// ===========================================================================================================================================================================
/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/
// ===========================================================================================================================================================================
Shader "Hidden/PyroTechnix/Decal" 
{
	SubShader 
	{
		// ===========================================================================================================================================================================
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		// ===========================================================================================================================================================================
		Pass
		{
			// ===========================================================================================================================================================================
			Blend ONE OneMinusSrcAlpha
			ZTest Off
			ZWrite Off
			Cull back
			// ===========================================================================================================================================================================
			CGPROGRAM
			#pragma vertex RenderVS
			#pragma fragment RenderPS
			#pragma target 3.0
			#pragma glsl
			#include "UnityCG.cginc"
			// ===========================================================================================================================================================================
			float4 _PositionWS_RadiusWS;
			float2 _UvScaleBias;
			float _RadiusSqrWS;
			float _Opacity;
			float3 _EyeRelativePositionWS;
			float _NoiseAmplitudeFactor;
			float _NoiseFrequencyFactor;
			float _InnerRadiusWS;
			float _DisplacementWS;
			float _NoiseScale;
			float _EdgeSoftness;

			float4x4 _Camera2World;
			sampler2D _GradientTex;
			sampler2D _CameraDepthTexture;
			sampler3D _NoiseVolume;

			static const float kMaxNumSteps = 50.0f;
			// ===========================================================================================================================================================================
			struct PS_INPUT
			{
				float4 posPS			: SV_POSITION;
				float4 screenPos		: TEXCOORD0;
				float4 viewDirWS_viewZ	: TEXCOORD1;
			};
			// ===========================================================================================================================================================================
			float decodeFloatRGBA( float4 rgba ) 
			{
  				return dot( rgba, float4(1.0, 1/255.0, 1/65025.0, 1/160581375.0) );
			}
			// ===========================================================================================================================================================================
			float Noise( float3 uvw )
			{
				return abs( decodeFloatRGBA( tex3Dlod( _NoiseVolume, float4( uvw, 0 ) ) ) * 2 - 1);	
			}
			// ===========================================================================================================================================================================
			float FractalNoiseAtPositionWS( float3 posWS )
			{
				float3 uvw = posWS * _NoiseScale; 
				float amplitude = 0.4f;
	
				float noiseValue = 0;
				{
					noiseValue += amplitude * Noise( uvw ); 
					amplitude *= _NoiseAmplitudeFactor; 
					uvw *= _NoiseFrequencyFactor;

					noiseValue += amplitude * Noise( uvw ); 
					amplitude *= _NoiseAmplitudeFactor; 
					uvw *= _NoiseFrequencyFactor;
				}
				return noiseValue; 
			}
			// ===========================================================================================================================================================================
			float Sphere( float3 posWS, float3 spherePositionWS, float radiusWS, float displacementWS, out float displacementOut )
			{
				displacementOut = FractalNoiseAtPositionWS( posWS );

				const float3 relativePosWS = posWS - spherePositionWS;

				const float signedDistanceToPrimitive = length( relativePosWS ) - radiusWS;

				return -displacementOut * displacementWS + signedDistanceToPrimitive;
			}
			// ===========================================================================================================================================================================
			PS_INPUT RenderVS( appdata_base i )
			{
				PS_INPUT o;

				const float3 posMS = mul( unity_ObjectToWorld, float4( i.vertex.xyz, 0 ) ).xyz;
				const float2 posClipSpaceAbs = abs( posMS.xy );
				const float maxLen = max( posClipSpaceAbs.x, posClipSpaceAbs.y );

				const float3 posCS = float3( posMS.xy, -(maxLen - 1.0f) );
				const float3 posLS = normalize( mul( _Camera2World, float4(posCS, 0) ).xyz );
	
				float3 posWS = posLS * _PositionWS_RadiusWS.w + _PositionWS_RadiusWS.xyz;

				int kNumShrinkSteps = 3;

				for( int i=0 ; i<kNumShrinkSteps ; i++ )
				{
					float dispOut;
					const float dist = Sphere( posWS, _PositionWS_RadiusWS.xyz, _InnerRadiusWS, _DisplacementWS, dispOut );
					const float edgeFade = saturate(_EdgeSoftness );
					posWS += max(0,(dist - _EdgeSoftness * 2)) * -posLS * 0.2f;
				}

				const float4 posPS =  mul( UNITY_MATRIX_VP, float4(posWS, 1) );
				const float3 relativePositionWS = posWS - _WorldSpaceCameraPos.xyz;
				const float viewZ = posPS.z;

				o.posPS = posPS;
				o.screenPos = ComputeScreenPos( posPS );
				o.viewDirWS_viewZ = float4( relativePositionWS / viewZ, viewZ );

				return o;
			}
			// ===========================================================================================================================================================================
			float4 MapDisplacementToColour( const float displacement, const float2 uvScaleBias )
			{
				const float texcoord = smoothstep( 0.3f, 0.7f, displacement * uvScaleBias.x + uvScaleBias.y);

				return float4(0..xxx, texcoord + 0.3f);//tex2Dlod( _GradientTex, float4(texcoord, 0.5f, 0, 0) );
			}
			// ===========================================================================================================================================================================
			float4 SceneFunction( const float3 posWS, const float3 spherePositionWS, const float radiusWS, const float displacementWS, const float2 uvScaleBias )
			{
				float displacementOut;
				const float dist = Sphere( posWS, spherePositionWS, radiusWS, displacementWS, displacementOut );

				const float4 colour = MapDisplacementToColour( displacementOut, uvScaleBias );
				const float kD = -_EdgeSoftness;
				const float edgeFade = smoothstep( 0.5f - kD, 0.5f + kD, dist );//1 - saturate( dist * _EdgeSoftness );

				return colour * float4( 1..xxx, edgeFade );
			}
			// ===========================================================================================================================================================================
			float4 RenderPS( const PS_INPUT i ) : COLOR
			{
				const float3 rayDirectionWS = ( i.viewDirWS_viewZ.xyz );
				const float depthAtPos = UNITY_SAMPLE_DEPTH( tex2Dproj( _CameraDepthTexture, UNITY_PROJ_COORD( i.screenPos ) ) );
				const float depthVS = LinearEyeDepth( depthAtPos );

				const float3 finalStepWS = rayDirectionWS * depthVS  + _WorldSpaceCameraPos.xyz;

				return SceneFunction( finalStepWS, _PositionWS_RadiusWS.xyz, _InnerRadiusWS, _DisplacementWS, _UvScaleBias ) * float4( 1..xxx, _Opacity );
			}

			ENDCG
			// ===========================================================================================================================================================================
		}
	} 
}
// ===========================================================================================================================================================================
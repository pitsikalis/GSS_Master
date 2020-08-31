// ===========================================================================================================================================================================
/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/
// ===========================================================================================================================================================================
Shader "Hidden/PyroTechnix/Render" 
{
	SubShader 
	{
		// ===========================================================================================================================================================================
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		// ===========================================================================================================================================================================
		CGINCLUDE
		#include "UnityCG.cginc"

		#define NUM_HULL_OCTAVES 2
		#define NUM_OCTAVES 5

		// ===========================================================================================================================================================================
		float4 _PositionWS_RadiusWS;
		float2 _UvScaleBias;
		float _RadiusSqrWS;
		float _StepSizeWS;
		float _Opacity;
		float _MaxNumSteps;
		float3 _EyeRelativePositionWS;
		float3 _NoiseAnimationSpeed;
		float3 _SunLightDir;
		float3 _Scale;
		float _NoiseAmplitudeFactor;
		float _NoiseFrequencyFactor;
		float _InnerRadiusWS;
		float _DisplacementWS;
		float _NoiseScale;
		float _EdgeSoftness;
		float _LightingDensity;

		float _SkinThickness;
		float3 _EyeForwardWS;

		float4x4 _Camera2World;
		float4x4 _World2Local;
		sampler2D _GradientTex;
		sampler2D _CameraDepthTexture;
		
		sampler3D _NoiseVolume;
		// ===========================================================================================================================================================================
		struct PS_INPUT
		{
			float4 PosPS : SV_Position;
			float2 rayHitNearFar : TEXCOORD0;
			float3 rayDirectionWS : TEXCOORD1;
			float4 screenPos : TEXCOORD2;
			float zPS : TEXCOORD3;
		};
		// ===========================================================================================================================================================================
		float Noise( float3 uvw )
		{
			const float4 noiseVal = tex3Dlod( _NoiseVolume, float4( uvw, 0 ) );

			return abs( DecodeFloatRGBA( noiseVal ) * 2 - 1);	
		}
		// ===========================================================================================================================================================================
		float FractalNoiseAtPositionWS( float3 posWS, uint numOctaves )
		{
			const float3 animation = _NoiseAnimationSpeed * _Time.x;	// Add some simple animatin to the noise patterns. NOTE: Could link this to wind direction.

			float3 uvw = posWS * _NoiseScale + animation; 
			float amplitude = 0.4f;
	
			float noiseValue = 0;

			for(uint i=0 ; i<numOctaves ; i++)
			{
				noiseValue += abs(amplitude * Noise( uvw )); 
				amplitude *= _NoiseAmplitudeFactor; 
				uvw *= _NoiseFrequencyFactor;
			}
			
			return noiseValue; 
		}
		// ===========================================================================================================================================================================
		float Box( float3 relativePosWS, float3 b )
		{
			const float3 d = abs( relativePosWS ) - b;
			return min( max( d.x, max( d.y, d.z ) ), 0.0f ) + length( max( d, 0.0f ) );
		}
		// ===========================================================================================================================================================================
		float Torus( float3 relativePosWS, float radiusWS )
		{
			const float2 t = radiusWS.xx * float2( 1, 0.01f );
			float2 q = float2( length( relativePosWS.xz ) - t.x , relativePosWS.y );
			return length( q ) - t.y;
		}
		// ===========================================================================================================================================================================
		float Cone( float3 relativePosWS, float radiusWS )
		{
			float d = length( relativePosWS.xz ) - lerp( radiusWS*0.5f, 0, (radiusWS + relativePosWS.y) / (radiusWS) ); 
			d = max( d,-relativePosWS.y - radiusWS ); 
			d = max( d, relativePosWS.y - radiusWS ); 

			return d; 
		}
		// ===========================================================================================================================================================================
		float Cylinder( float3 relativePosWS, float radiusWS)
		{
			const float2 h = radiusWS.xx * float2( 0.7f, 1 );
			const float2 d = abs( float2( length( relativePosWS.xz ), relativePosWS.y ) ) - h;
			return min( max( d.x, d.y ), 0.0f) + length( max( d, 0.0f) );
		}
		// ===========================================================================================================================================================================
		float Sphere( float3 relativePosWS, float radiusWS)
		{
			return length( relativePosWS ) - radiusWS;
		}
		// ===========================================================================================================================================================================
		float DisplacedPrimitive( float3 posWS, float3 spherePositionWS, float radiusWS, float displacementWS, uint numOctaves, out float displacementOut )
		{
			float3 relativePosWS = posWS - spherePositionWS;

	#	if DISABLE_AXIS_ALIGNED
			relativePosWS = mul(_World2Local, float4(relativePosWS, 0)).xyz;
	#	endif

			displacementOut = FractalNoiseAtPositionWS( posWS, numOctaves );

	#	if USE_SPHERE
			const float signedDistanceToPrimitive = Sphere( relativePosWS, radiusWS );
	#	elif USE_CYLINDER
			const float signedDistanceToPrimitive = Cylinder( relativePosWS, radiusWS );
	#	elif USE_CONE
			const float signedDistanceToPrimitive = Cone( relativePosWS, radiusWS );
	#	elif USE_TORUS
			const float signedDistanceToPrimitive = Torus( relativePosWS, radiusWS );
	#	elif USE_BOX
			const float signedDistanceToPrimitive = Box( relativePosWS, sqrt(radiusWS*radiusWS/2).xxx );
	#	endif

			return signedDistanceToPrimitive-displacementOut * displacementWS + signedDistanceToPrimitive;
		}
		// ===========================================================================================================================================================================
		PS_INPUT RenderVS( appdata_base i )
		{
			const float3 posClipSpace = float3( i.vertex.xy, 0 );
			const float2 posClipSpaceAbs = abs( posClipSpace.xy );
			const float maxLen = max( posClipSpaceAbs.x, posClipSpaceAbs.y );

			float3 dir = normalize(float3(posClipSpace.xy, (maxLen - 1.0f)));

			// Even though our geometry only extends around the front of the explosion volume,
			//  we can calculate the reverse side of the hull here aswell.

			// First get the front world space position of the hull.
			float3 frontNormDir = dir;
			float3 frontPosLS = mul(_Camera2World, float4(frontNormDir, 0)).xyz;
			float3 frontPosWS = frontPosLS * _PositionWS_RadiusWS.w + _PositionWS_RadiusWS.xyz;
			float3 frontDirWS = normalize(frontPosLS);

			int kNumShrinkSteps =2;

			// Then perform the shrink wrapping step using sphere tracing.
			for(uint i=0 ; i<kNumShrinkSteps; i++)
			{
				float displacementOut; // na
				float dist = DisplacedPrimitive(frontPosWS, _PositionWS_RadiusWS.xyz, _InnerRadiusWS, _DisplacementWS, NUM_HULL_OCTAVES, displacementOut) - abs(_EdgeSoftness);

				frontPosWS -= frontDirWS * dist * 0.5f;
			}

			frontPosWS += frontDirWS * (_SkinThickness * _DisplacementWS);
			float4 frontPosPS = mul(UNITY_MATRIX_VP, float4(frontPosWS, 1));

			// Then repeat the process for the back faces.
			float3 backNormDir = dir * float3(1, 1, -1);
			float3 backPosLS = mul(_Camera2World, float4(backNormDir, 0)).xyz;
			float3 backPosWS = backPosLS * _PositionWS_RadiusWS.w + _PositionWS_RadiusWS.xyz;
			float3 backDirWS = normalize(backPosLS);

			for(uint j=0 ; j<kNumShrinkSteps ; j++)
			{
				float displacementOut; // na
				float dist = DisplacedPrimitive(backPosWS, _PositionWS_RadiusWS.xyz, _InnerRadiusWS, _DisplacementWS, NUM_HULL_OCTAVES, displacementOut) - abs(_EdgeSoftness);
				backPosWS -= backDirWS * dist * 0.5f;
			}

			backPosWS += backDirWS * (_SkinThickness * _DisplacementWS );
			float4 backPosPS = mul(UNITY_MATRIX_VP, float4(backPosWS, 1));

		#	if FRONT_RENDERING
			float4 posPS = frontPosPS;
			float3 posWS = frontPosWS;
		#	else
			float4 posPS = backPosPS;
			float3 posWS = backPosWS;
		#	endif

			float3 relativePosWS = posWS - _WorldSpaceCameraPos.xyz;
			float3 rayDirectionWS = normalize(relativePosWS);///posPS.z;

			PS_INPUT o = (PS_INPUT)0;
			{

				o.PosPS = posPS;
				o.rayHitNearFar = float2(frontPosPS.z, backPosPS.z) * posPS.w;
				o.rayDirectionWS = rayDirectionWS * posPS.w;
				o.screenPos = ComputeScreenPos( posPS );

				// Prepare for software perspective reversal.
				o.zPS = posPS.w;
			}

			return o;
		}
		// ===========================================================================================================================================================================
		float4 MapDisplacementToColour( const float displacement, const float2 uvScaleBias )
		{
			const float texcoord = smoothstep( 0.3f, 0.7f, displacement * uvScaleBias.x + uvScaleBias.y);

			return tex2Dlod( _GradientTex, float4(texcoord, 0.5f, 0, 0) );
		}
		// ===========================================================================================================================================================================
		float CalculateLighting( float3 posWS, float3 spherePositionWS, float radiusWS, float displacementWS )
		{
			const float lightingDisplacementWS = 0;
			const float lightingRadiusWS = radiusWS;
			
			const int kNumLightSteps = 4;
			const float kRecipNumLightSteps = 1.0f/kNumLightSteps;
			const float3 lightStep = -_SunLightDir * lightingRadiusWS * kRecipNumLightSteps;
			
			float transmittance = 1;
			int stepsTaken = 0;
			while( stepsTaken++ < kNumLightSteps && transmittance > 0 )
			{
				float not_used;
				const float lightDist = DisplacedPrimitive( posWS, spherePositionWS, lightingRadiusWS, lightingDisplacementWS, NUM_OCTAVES,  not_used );	
				const float kD = -_EdgeSoftness;
				const float lightEdgeFade = smoothstep( 0.5f - kD, 0.5f + kD, lightDist );
				transmittance *= saturate( 1 - lightEdgeFade * _LightingDensity );
			
				posWS += lightStep;
			}
			
			return transmittance;
		}
		// ===========================================================================================================================================================================
		float4 SceneFunction( const float3 posWS, const float3 spherePositionWS, const float radiusWS, const float displacementWS, const float2 uvScaleBias )
		{
			float displacementOut;
			const float dist = DisplacedPrimitive( posWS, spherePositionWS, radiusWS, displacementWS, NUM_OCTAVES, displacementOut );

			const float4 colour = MapDisplacementToColour( displacementOut, uvScaleBias );
			const float kD = _EdgeSoftness;
			const float edgeFade = smoothstep( 0.5f - kD, 0.5f + kD, dist );

	#	if ENABLE_LIGHTING
			const float lighting = CalculateLighting( posWS, spherePositionWS, radiusWS, displacementWS ) + Luminance(colour) * 2;
	#	else
			const float lighting = 1;
	#	endif

			return colour * float4( lighting.xxx, edgeFade );
		}
		// ===========================================================================================================================================================================
		inline float4 Blend( const float4 src, const float4 dst )
		{
			float4 col = 0..xxxx;
				
			const float t = dst.a * -src.a + dst.a;
			
			col.rgb = dst.rgb * t + src.rgb;
			col.a	= src.a + t;
			
			return col;
		}
		// ===========================================================================================================================================================================
		float4 RenderPS( PS_INPUT i ) : COLOR
		{
			i.rayHitNearFar /= i.zPS;
			i.rayDirectionWS /= i.zPS;

			float3 rayDirectionWS = i.rayDirectionWS;
			float nearD = i.rayHitNearFar.x, farD = i.rayHitNearFar.y;

			float depthAtPos = UNITY_SAMPLE_DEPTH( tex2Dproj( _CameraDepthTexture, UNITY_PROJ_COORD( i.screenPos ) ) );
			float depthVS = LinearEyeDepth( depthAtPos );

		#	if BACK_RENDERING
			if( nearD > depthVS )
			{
				return 0..xxxx;
			}
		#	endif

			farD = min( farD, depthVS );

			float4 output = 0..xxxx;

			float3 startWS = rayDirectionWS * nearD + _WorldSpaceCameraPos.xyz;
			float3 endWS   = rayDirectionWS * farD  + _WorldSpaceCameraPos.xyz;

			float3 stepAmountWS = rayDirectionWS * _StepSizeWS; 
			float numSteps = min( _MaxNumSteps, (farD - nearD) / _StepSizeWS );

			float3 posWS = startWS;

			float stepsTaken = 0;
			while( stepsTaken++ < numSteps && output.a < _Opacity )
			{
				float4 colour = SceneFunction( posWS, _PositionWS_RadiusWS.xyz, _InnerRadiusWS, _DisplacementWS, _UvScaleBias );
				output = Blend( output, colour );
        
				posWS += stepAmountWS;
			}

			return output * float4( 1..xxx, _Opacity );
		}

		ENDCG	
		// ===========================================================================================================================================================================
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Less
			ZWrite Off
			Cull back

			CGPROGRAM
			#pragma vertex RenderVS
			#pragma fragment RenderPS
			#pragma multi_compile ENABLE_LIGHTING DISABLE_LIGHTING
			#pragma multi_compile USE_SPHERE USE_CONE USE_CYLINDER USE_TORUS USE_BOX
			#pragma multi_compile ENABLE_AXIS_ALIGNED DISABLE_AXIS_ALIGNED
			#pragma multi_compile FRONT_RENDERING BACK_RENDERING

	#	ifdef SHADER_API_D3D11 
			#pragma target 5.0
	#	else
			#pragma target 3.0
			#pragma glsl
	#	endif

			ENDCG
		}
		// ===========================================================================================================================================================================
	} 
}
// ===========================================================================================================================================================================
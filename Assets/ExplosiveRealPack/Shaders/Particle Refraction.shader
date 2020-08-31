// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//A simple refraction shader with no ghosting. I'm not a professional shader programmer so please let me know if it can be improved.
Shader "Particles/Refraction" {
Properties {
 
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    _Multiplier("Multiplier", Range(0.01, 10.0)) = 1

}
 
Category {
    Tags { "Queue" = "Transparent+100" }
    SubShader {
        GrabPass {
            Name "BASE"
            Tags { "LightMode" = "Always" }
        }
        Pass {
            Name "BASE"
            Tags { "LightMode" = "Always" }      
            Fog { Color (0,0,0,0) }
            Lighting Off
			Cull Off

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile_particles
#include "UnityCG.cginc"
			

sampler2D _GrabTexture : register(s0);
sampler2D _MainTex;
float _Multiplier;

 
struct data {
 
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float4 texcoord : TEXCOORD0;
};
 
float4 _MainTex_ST;
struct v2f {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float4 screenPos : TEXCOORD0;
	float2 uvmain : TEXCOORD2;
	#ifdef SOFTPARTICLES_ON
	float4 projPos : TEXCOORD1;
	#endif
};
 
sampler2D _CameraDepthTexture;
float _InvFade;
v2f vert(data v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);    
    float depth = -mul( UNITY_MATRIX_MV, v.vertex ).z;  
    o.color = v.color / (depth*0.01); //The value '0.01' might need to be changed depending on how big the scene is. '0.1' for a small scene. '0.001' for a big scene
    o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);   
    o.screenPos = o.vertex;
	#ifdef SOFTPARTICLES_ON
	o.projPos = ComputeScreenPos (o.vertex);
	COMPUTE_EYEDEPTH(o.projPos.z);
	#endif
    return o;
}

half4 frag( v2f i ) : COLOR
{  

    half4 texColor = tex2D(_MainTex, i.uvmain);
    float amount = _Multiplier * i.color.a * texColor.a * 0.1;
	clip(amount-0.01);
	
    #ifdef SOFTPARTICLES_ON
	float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
	float partZ = i.projPos.z;
	float fade = saturate (_InvFade * (sceneZ-partZ));
	amount *= fade;
	#endif
	
    float2 screenPos = i.screenPos.xy / i.screenPos.w;
    screenPos.x = (screenPos.x + 1) * 0.5;
    screenPos.y = (screenPos.y + 1) * 0.5;
 
    if (_ProjectionParams.x < 0)
        screenPos.y = 1 - screenPos.y;

    screenPos.x += (texColor.r - 0.5) * amount;
    screenPos.y += (texColor.g - 0.5) * amount;

    half4 col = tex2D( _GrabTexture, screenPos );
    return col;
 
}
ENDCG
}
}
} 
}
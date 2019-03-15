
Shader "hyx/MapFx" 
{
	Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
	_Foam("Foam",2D)=""{}
	_WaterNormal ("WaterNormal", 2D) = "" {}
	_WaveMask("WaveMask",2D)="white"{}
	_ReflectionMap(" ReflectionMap",CUBE)=""{}
	_WaveSpeed("WaveSpeed",float)=1
	_FoamSpeed("FoamSpeed",float)=1
	_FoamOpacity("FoamOpacity",Range(0,1))=1
	_RefAmount("RefAmount",Range(0,1))=1
	_WaterColor("WaterColor",Color)=(1,1,1,1)
	_Waterdistortion("Waterdistortion",Range(0,1))=1
	}

	SubShader {
		Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		LOD 200
		GrabPass{}

		CGPROGRAM
		#pragma surface surf Lambert 
		#include "UnityCG.cginc"	
		struct Input 
		{
			float2 uv_MainTex;
			float3 worldRefl;
			float3 worldPos;
		};

		sampler2D _GrabTexture;
		sampler2D _MainTex;
		sampler2D _Foam;
		sampler2D _WaveMask;

		fixed4 _Color;
		sampler2D _WaterNormal;

		float4 _WaterNormal_ST;
		float4 _Foam_ST;

		float _WaveSpeed;
		float _FoamSpeed;
		float _FoamOpacity;
		
		samplerCUBE _ReflectionMap;
		float _RefAmount;
	    fixed4 _WaterColor;
		float _Waterdistortion;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) 
		{
			
			float3 WaterNM1 = UnpackNormal( tex2D( _WaterNormal, IN.uv_MainTex*_WaterNormal_ST.xy+_Time.y * _WaveSpeed));
			float3 WaterNM2 = UnpackNormal( tex2D( _WaterNormal, IN.uv_MainTex*_WaterNormal_ST.xy+_Time.w * -_WaveSpeed));

			float2 foamUV1=  IN.uv_MainTex*_Foam_ST.xy+_Time.y * _FoamSpeed;
			float2 foamUV2=  IN.uv_MainTex*_Foam_ST.xy+_Time.w * -_FoamSpeed;

			float3 WaterNM=WaterNM1+WaterNM2;

			float4 Mask = tex2D( _WaveMask, IN.uv_MainTex );
			float3 NM = WaterNM * Mask.r;

			float3 worldPos = IN.worldPos;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
			float dotNV = dot( WaterNM , worldViewDir );
			
			fixed4 Foam=(tex2D(_Foam,foamUV1)+tex2D(_Foam,foamUV2))*Mask.g*_FoamOpacity* _WaterColor;

			fixed4 baseColor = tex2D( _GrabTexture, IN.uv_MainTex+NM.xy*_Waterdistortion/1000)*_Color;

			fixed4 ref=texCUBE( _ReflectionMap, reflect(worldViewDir,WaterNM)) * Mask.r* _RefAmount * _WaterColor;

			o.Emission = baseColor+ref+Foam;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
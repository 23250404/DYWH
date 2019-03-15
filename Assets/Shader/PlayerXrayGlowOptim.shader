Shader "hyx/PlayerXrayGlowOptim" {
	Properties {
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_RenderTexture("RenderTexture", 2D) = "white" {}
		_Trans("Trans1", Float) = 0.25
		_MaskColor("MaskColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Transparent"  "Queue" = "AlphaTest"  }
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha 
		
		Cull Back
		CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows keepalpha
		#pragma target 3.0
		struct Input
		{
			float4 screenPos;
			float3 viewDir;
			float3 worldNormal;
			float2 uv_texcoord;
		};
		uniform float _Trans;
		uniform float4 _MaskColor;
		uniform sampler2D _RenderTexture;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Cutoff = 0.5;

		void surf (Input i, inout SurfaceOutput o) {
			float4 screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 screenUV = screenPos / screenPos.w;
			float MaskValuve = tex2D( _RenderTexture, screenUV.xy).b;
			float3 ase_worldNormal = i.worldNormal;
			float VdotN = dot( i.viewDir , ase_worldNormal );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 MainColor = tex2D( _MainTex, uv_MainTex );
			fixed4 MaskColor=(6-VdotN)*0.25*_MaskColor;
			o.Alpha = MaskValuve*_Trans+(1-MaskValuve);

			// half rim = 1.0 – saturate(dot (normalize(IN.viewDir), IN.worldNormal));

			o.Emission =lerp(MainColor, MaskColor,MaskValuve);

			// half rim = 1.0 – saturate(dot (normalize(IN.viewDir), IN.worldNormal));
			// o.Emission = _RimColor.rgb * pow (rim, _RimPower);

			clip(  MainColor.a - _Cutoff );
		}
		ENDCG
	}
	FallBack "Diffuse"
}
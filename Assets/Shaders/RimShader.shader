Shader "Custom/RimShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower ("Rim Power", Range(0,6)) = 3.0
	}
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Standard alpha

		struct Input {
			float4 color : Color;
			float3 worldNormal;
			//float2 uv_MainTex;
			float3 viewDir;
		};

		float4 _Color;
		//sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		float4 _RimColor;
		float _RimPower;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			IN.color = _Color;
			//float4 color = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = IN.color;
			float3 normal = IN.worldNormal;

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = IN.color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

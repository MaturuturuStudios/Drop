Shader "Custom/Drop Eyes Shader" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_DeepFactor ("Deep Factor", Range(-1,1)) = 0.1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower ("Rim Power", Range(0,6)) = 3.0
	}
	SubShader {
		Tags{ "Queue" = "Transparent+100" "RenderType" = "Opaque" }
		
		CGPROGRAM
		#pragma surface surf Standard vertex:vert

		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			float3 viewDir;
			float3 localNormal;
		};

		float4 _Color;
		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		float4 _RimColor;
		float _RimPower;
		float _DeepFactor;

		float Mirror;

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);

			o.localNormal = v.normal;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			if (Mirror == 1)
				IN.uv_MainTex.x = 1 - IN.uv_MainTex.x;

			float3 viewFactor = normalize(IN.viewDir);
			IN.uv_MainTex -= viewFactor / dot(viewFactor, IN.worldNormal) * _DeepFactor;

			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			float3 normal = IN.localNormal;
			half rim = 1.0 - saturate(normal.z);
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
	}
	FallBack "Diffuse"
}

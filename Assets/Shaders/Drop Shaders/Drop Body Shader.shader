Shader "Custom/Drop Body Shader" {
	Properties {
		[Header(Color)] _Color ("Color", Color) = (1,1,1,1)
		_BoostColor ("BoostColor", Color) = (0,0,0,0)

		[Header(PBR)] _Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		[Header(Rim Light)] _RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimFalloff ("Rim Falloff", Range(0,10)) = 3.0
		_RimDistance("Rim Distance", Range(0,1)) = 1.0
		_RimScale ("Rim Scale", Range(0,1)) = 1.0

		[Header(Distortion)] _Distort("Distortion", range(0,128)) = 10.0
		_DistortSpeed("Distortion Speed", range (0,128)) = 1.0

		[Header(Waves)] _WaveAmpX("Wave Amplitude X", Float) = 2.0
		_WaveAmpY("Wave Amplitude Y", Float) = 2.0
		[Space] _WaveSpeedX("Wave Speed X", Float) = 2.0
		_WaveSpeedY("Wave Speed Y", Float) = 5.0
		[Space] _WaveFreqX("Wave Frequency X", Float) = 20.0
		_WaveFreqY("Wave Frequency Y", Float) = 20.0
	}
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha

		GrabPass{}

		CGPROGRAM
		#pragma vertex vert
		#pragma surface surf Standard alpha

		sampler2D _GrabTexture;
		float4 _GrabTexture_TexelSize;

		struct Input {
			float3 worldNormal;
			float3 viewDir;
			float4 projection : TEXCOORD;
		};

		float4 _Color;
		float4 _BoostColor;

		half _Glossiness;
		half _Metallic;

		float4 _RimColor;
		float _RimFalloff;
		float _RimDistance;
		float _RimScale;

		float _Distort;
		float _DistortSpeed;

		float _WaveAmpX;
		float _WaveAmpY;
		float _WaveSpeedX;
		float _WaveSpeedY;
		float _WaveFreqX;
		float _WaveFreqY;

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float4 oPos = mul(UNITY_MATRIX_MVP, v.vertex);
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			o.projection.xy = (float2(oPos.x, oPos.y * scale) + oPos.w) * 0.5;
			o.projection.zw = oPos.zw;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float3 normal = IN.worldNormal;
			float3 viewDir = normalize(IN.viewDir);

			half rim = max(0.0001, _RimDistance - saturate(dot(viewDir, normal)));

			float distortion = _Distort * sin(_DistortSpeed * _Time.y);
			float2 offset = normal * distortion * _GrabTexture_TexelSize.xy;
			IN.projection.xy += offset * IN.projection.zz;
			IN.projection.x += _WaveAmpX * sin(_WaveSpeedX * _Time.y + IN.projection.y * _WaveFreqX) * _GrabTexture_TexelSize.xy;
			IN.projection.y += _WaveAmpY * sin(_WaveSpeedY * _Time.y + IN.projection.x * _WaveFreqY) * _GrabTexture_TexelSize.xy;
			half4 color = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.projection));

			o.Emission = _BoostColor + _RimColor.rgb * pow(rim, _RimFalloff) * _RimScale;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Albedo = _Color.rgb * color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

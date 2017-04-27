Shader "Custom/Drop Body Shader" {
	Properties {
		[Header(Color)] _Color("Color", Color) = (1,1,1,1)
		_BoostColor("BoostColor", Color) = (0,0,0,0)

		[Header(PBR)] _Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		[Header(Rim Light)] _RimColor("Rim Color", Color) = (1,1,1,1)
		_RimFalloff("Rim Falloff", Range(0,10)) = 3.0
		_RimDistance("Rim Distance", Range(0,1)) = 1.0
		_RimScale("Rim Scale", Range(0,1)) = 1.0

		[Header(Distortion)] _Distort("Distortion", range(0,128)) = 10.0
		_DistortSpeed("Distortion Speed", range(0,128)) = 1.0

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
			#pragma surface surf Standard alpha vertex:vert addshadow

			uniform float DeformationDistance;
			uniform float DeformationRays;
			uniform float3 DeformationCenter;
			uniform float DeformationOffsets[64];
			uniform float4 DeformationDirections[64];

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

				float PI = 3.14159;

				float4 globalPos = mul(unity_ObjectToWorld, v.vertex);

				float3 distance = globalPos.xyz - DeformationCenter.xyz;

				float angle = atan2(distance.y, distance.x) + 2 * PI;
				if (angle >= 2 * PI) {
					angle -= 2 * PI;
				}
				float angleFactor = DeformationRays * angle / (2 * PI);
				float factor = frac(angleFactor);

				int index = floor(angleFactor);
				int nextIndex = index + 1;
				if (nextIndex >= DeformationRays) {
					nextIndex = 0;
				}

				float offset = DeformationOffsets[index] * (1.0 - factor);
				offset += DeformationOffsets[nextIndex] * factor;
				float zFactor = length(distance.xy) / DeformationDistance;
				if (zFactor > 0) {
					globalPos.xy += offset * normalize(distance.xy) * zFactor;
				}

				v.vertex = mul(unity_WorldToObject, globalPos);
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

		// Pass to render object as a shadow collector
		Pass{
			Name "ShadowCollector"
			Tags{ "LightMode" = "ShadowCollector" }

			Fog{ Mode Off }
			ZWrite On ZTest LEqual

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_shadowcollector

				#define SHADOW_COLLECTOR_PASS
				#include "UnityCG.cginc"

				uniform float DeformationDistance;
				uniform float DeformationRays;
				uniform float3 DeformationCenter;
				uniform float DeformationOffsets[64];
				uniform float4 DeformationDirections[64];

				struct appdata {
					float4 vertex : POSITION;
				};

				struct v2f {
					V2F_SHADOW_COLLECTOR;
				};

				v2f vert(appdata v) {
					float PI = 3.14159;

					float4 globalPos = mul(unity_ObjectToWorld, v.vertex);

					float3 distance = globalPos.xyz - DeformationCenter.xyz;

					float angle = atan2(distance.y, distance.x) + 2 * PI;
					if (angle >= 2 * PI) {
						angle -= 2 * PI;
					}
					float angleFactor = DeformationRays * angle / (2 * PI);
					float factor = frac(angleFactor);

					int index = floor(angleFactor);
					int nextIndex = index + 1;
					if (nextIndex >= DeformationRays) {
						nextIndex = 0;
					}

					float offset = DeformationOffsets[index] * (1.0 - factor);
					offset += DeformationOffsets[nextIndex] * factor;
					float zFactor = length(distance.xy) / DeformationDistance;
					if (zFactor > 0) {
						globalPos.xy += offset * normalize(distance.xy) * zFactor;
					}

					v.vertex = mul(unity_WorldToObject, globalPos);

					// Shadow collector operations
					v2f o;
					TRANSFER_SHADOW_COLLECTOR(o);
					return o;
				}

				fixed4 frag(v2f i) : COLOR{
					SHADOW_COLLECTOR_FRAGMENT(i);
				}
			ENDCG
		}

		// Pass to render object as a shadow caster
		Pass{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }

			Fog{ Mode Off }
			ZWrite On ZTest LEqual Cull Off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_shadowcaster
				#pragma fragmentoption ARB_precision_hint_fastest

				#include "UnityCG.cginc"

				uniform float DeformationDistance;
				uniform float DeformationRays;
				uniform float3 DeformationCenter;
				uniform float DeformationOffsets[64];
				uniform float4 DeformationDirections[64];

				struct v2f {
					V2F_SHADOW_CASTER;
				};

				v2f vert(appdata_full v) {
					float PI = 3.14159;

					float4 globalPos = mul(unity_ObjectToWorld, v.vertex);

					float3 distance = globalPos.xyz - DeformationCenter.xyz;

					float angle = atan2(distance.y, distance.x) + 2 * PI;
					if (angle >= 2 * PI) {
						angle -= 2 * PI;
					}
					float angleFactor = DeformationRays * angle / (2 * PI);
					float factor = frac(angleFactor);

					int index = floor(angleFactor);
					int nextIndex = index + 1;
					if (nextIndex >= DeformationRays) {
						nextIndex = 0;
					}

					float offset = DeformationOffsets[index] * (1.0 - factor);
					offset += DeformationOffsets[nextIndex] * factor;
					float zFactor = length(distance.xy) / DeformationDistance;
					if (zFactor > 0) {
						globalPos.xy += offset * normalize(distance.xy) * zFactor;
					}

					v.vertex = mul(unity_WorldToObject, globalPos);

					// Shadow caster operations
					v2f o;
					TRANSFER_SHADOW_CASTER(o);
					return o;
				}

				float4 frag(v2f i) : COLOR{
					SHADOW_CASTER_FRAGMENT(i);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}

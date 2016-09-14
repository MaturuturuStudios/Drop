Shader "Custom/Glow" {
	Properties{
		_GlowColor("Glow Color", Color) = (0,0,0,1)
		_GlowPower("Glow Power", Range(0.0, 10.0)) = 1.0
		_GlowMap("Glow Map", 2D) = "white" {}
		_GlowDistance("Glow Distance", Range(0.0, 20.0)) = 1.0
		_FadePower("Fade Power", Range (0.0, 5.0)) = 1.0
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f {
		float4 pos : POSITION;
		float4 color : COLOR;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	uniform sampler2D _GlowMap;
	uniform float4 _GlowColor;
	uniform float _GlowPower;
	uniform float _GlowDistance;
	uniform float _FadePower;

	v2f vert(appdata v) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.uv;

		float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);

		o.pos.xy += offset * _GlowDistance;
		o.color = _GlowColor;
		o.normal = norm;
		return o;
	}
	ENDCG

	SubShader {
		Tags{ 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent"
		}

		Pass{
			Name "BASE"
			Cull Back
			Blend Zero One

			SetTexture[_OutlineColor]{
				ConstantColor(0,0,0,0)
				Combine constant
			}
		}

		Pass{
			Name "OUTLINE"
			Cull Front

			Blend SrcAlpha OneMinusSrcAlpha	// Normal
			//Blend One One						// Additive
			//Blend One OneMinusDstColor		// Soft Additive
			//Blend DstColor Zero				// Multiplicative
			//Blend DstColor SrcColor			// 2x Multiplicative

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) :COLOR {
				i.color *= tex2D(_GlowMap, i.uv);
				i.color.a *= _GlowPower * pow(i.normal.z, _FadePower);
				return i.color;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
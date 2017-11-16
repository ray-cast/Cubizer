Shader "Custom/Gradient"
{
	Properties
	{
		_Color1("Color", Color) = (1,1,1,1)
		_Color2("Color", Color) = (1,1,1,1)
		_Threshold("Threshold", Range(1,256)) = 128
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Edge("Edge", Range(0.0,30.0)) = 20.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM

			#pragma surface surf Standard vertex:vert fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
				float3 vertColor;
			};

			half _Glossiness;
			half _Metallic;
			half _Edge;
			half _Threshold;
			fixed4 _Color1;
			fixed4 _Color2;

			UNITY_INSTANCING_CBUFFER_START(Props)
			UNITY_INSTANCING_CBUFFER_END

			void vert(inout appdata_full v, out Input o)
			{
				o.uv_MainTex = v.texcoord;
				o.vertColor = lerp(_Color1, _Color2, v.vertex.y / _Threshold);
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				o.Albedo = IN.vertColor;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Occlusion = 1;
				o.Alpha = 1;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
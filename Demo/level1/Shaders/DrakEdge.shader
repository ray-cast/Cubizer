Shader "Custom/DarkEdge"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
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

			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			half _Edge;
			fixed4 _Color;

			UNITY_INSTANCING_CBUFFER_START(Props)
			UNITY_INSTANCING_CBUFFER_END

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb * lerp(0.5, 1.0, pow((1 - abs(IN.uv_MainTex.x * 2 - 1)) * (1 - abs(IN.uv_MainTex.y * 2 - 1)), 1.0 / _Edge));
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Occlusion = 1;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
Shader "Custom/Wireframe"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_LineWidth("LineWidth", Range(0,1)) = 0.05
		_LineColor("LineColor", Color) = (0,0,0,1)
	}
		SubShader
		{
			Tags
			{
				"RenderType" = "Transparent"
				"ForceNoShadowCasting" = "True"
				"Queue" = "Overlay"
			}

			LOD 200

			Cull off

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
			half _LineWidth;

			fixed4 _Color;
			fixed4 _LineColor;

			UNITY_INSTANCING_CBUFFER_START(Props)
			UNITY_INSTANCING_CBUFFER_END

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				float lx = step(_LineWidth, IN.uv_MainTex.x);
				float ly = step(_LineWidth, IN.uv_MainTex.y);
				float hx = step(IN.uv_MainTex.x, 1.0 - _LineWidth);
				float hy = step(IN.uv_MainTex.y, 1.0 - _LineWidth);

				float vis = lerp(1, 0, lx*ly*hx*hy);
				clip(vis - 1e-5);

				fixed4 c = _LineColor;
				o.Albedo = c.rgb;
				o.Metallic = 0;
				o.Smoothness = 0;
				o.Occlusion = 0;
				o.Emission = _LineColor.rgb;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
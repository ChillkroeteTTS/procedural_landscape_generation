Shader "Custom/OwnTerrainSurfaceShader"
{
	Properties{
		/* Surface properties*/
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0


		_TerrainSize("Terrain Size", Float) = 200
		_Height("Height", Float) = 100
		_LatticeSize("Lattice Size", Float) = 3
		_k("k", Float) = 8
		_Lacunarity("Lacunarity", Float) = 2
		_h("h", Float) = 1
		_LatticeTex("Lattice Tex", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		//#include "PerlinNoise.cginc"
		#include "OwnNoise.cginc"
		//#include "RidgedNoise.cginc"
		#pragma target 4.0

		/* Surface Shader */
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

		/* Onw Shader */

		float _Height;

		float _TerrainSize;

		float _LatticeSize;

		float _k;

		float _Lacunarity;

		float _h;

		sampler2D _LatticeTex;



		/********** SHADER ****************/
		struct vertIn {
			float4 vertex : POSITION; // vertex position input
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
		};

		void vert(inout appdata_base v)
		{
			//Calc pos
			v.vertex += float4(0, 
								_Height * GetFractalNoiseHeight(_LatticeTex, _LatticeSize, v.texcoord.x, v.texcoord.y, _k, _Lacunarity, _h),
								0,0);
			v.normal = normalize(cross(normalize(float3(0, -GetFractalNoiseDerivative(_LatticeTex, _LatticeSize, v.texcoord.x, v.texcoord.y, _k, _Lacunarity, _h, false), 1)),
									normalize(float3(1, -GetFractalNoiseDerivative(_LatticeTex, _LatticeSize, v.texcoord.x, v.texcoord.y, _k, _Lacunarity, _h, true), 0))));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
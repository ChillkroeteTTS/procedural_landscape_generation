Shader "Custom/OwnTerrain"
{
	Properties{
		/* Surface properties*/
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_TerrainSize("Terrain Size", Float) = 200
		_Height("Height", Float) = 200
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
		#pragma surface surf Standard fullforwardshadows
		#include "FractalNoise.cginc"
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

		/* OwnTErrain Shader*/
		struct v2f {
			fixed4 uv : TEXCOORD0;
			float4 pos : SV_POSITION;
		};

		float _Height;

		float _TerrainSize;

		float _LatticeSize;

		float _k;

		float _Lacunarity;

		float _h;

		sampler2D _LatticeTex;



		/********** SHADER ****************/

		v2f vert(
			float4 vertex : POSITION, // vertex position input
			uint vid : SV_VertexID, // vertex ID, needs to be uint
			float4 texCoord : TEXCOORD0
			)
		{
			float f = (float)vid;
			v2f o;

			//Calc pos
			o.pos = mul(UNITY_MATRIX_MVP, float4(vertex.x, _Height * GetFractalNoiseHeight(_LatticeTex, _LatticeSize, texCoord.x, texCoord.y, _k, _Lacunarity, _h)
												,vertex.z, vertex.w));
			//o.pos = mul(UNITY_MATRIX_MVP, float4(vertex.x, 0.2
			//									,vertex.z, vertex.w));


			//o.color = float4(simpleCalc(f), 0, 0, 0);
			o.uv = float4(texCoord.xy, 0, 0);
			return o;
		}


		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = tex2D(_MainTex, i.uv.xy); 
			//return i.color;
			return col;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
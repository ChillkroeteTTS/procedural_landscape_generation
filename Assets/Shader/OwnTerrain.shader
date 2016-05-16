Shader "Custom/OwnTerrain"
{
	Properties
	{
		_WaterTex("WaterTed", 2D) = "white" {}
		_SandTex("SandTex", 2D) = "white" {}
		_GrassTex("GrassTex", 2D) = "white" {}
		_MountainTex("MountainTex", 2D) = "white" {}
		_PeakTex("PeakTex", 2D) = "white" {}
		_LightPos("LightPos", Vector) = (0,0,0)
		_Mambient("Mambient", Float) = 0.2
		_Mdiff("Mdiff", Float) = 0.8
		_Mspec("Mspec", Float) = 0
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
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			//#include "PerlinNoise.cginc"
			#include "OwnNoise.cginc"
			//#include "RidgedNoise.cginc"
			#pragma target 4.0
			float4 _LightPos;
			float _Mambient;
			float _Mdiff;
			float _Mspec;
			fixed4 _Color;

			/* Onw Shader */

			float _Height;

			float _TerrainSize;

			float _LatticeSize;

			float _k;

			float _Lacunarity;

			float _h;

			sampler2D _LatticeTex;

			sampler2D _MainTex;

			
			sampler2D _WaterTex;
			float4 _WaterTex_ST;
			sampler2D _SandTex;
			sampler2D _GrassTex;
			sampler2D _MountainTex;
			sampler2D _PeakTex;
			#include "UnityCG.cginc"
			#include "Helper.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				uint id : SV_VertexID;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				fixed4 color : TEXCOORD1;
			};

			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				float val = GetFractalNoiseHeight(_LatticeTex, _LatticeSize, v.uv.x, v.uv.y, _k, _Lacunarity, _h);
				float4 objSpaceVert = float4(v.vertex.x,
											_Height * val,
											v.vertex.z, v.vertex.w);
				o.vertex = mul(UNITY_MATRIX_MVP, objSpaceVert);

				o.normal = normalize(cross(normalize(float3(0, -GetFractalNoiseDerivative(_LatticeTex, _LatticeSize, v.uv.x, v.uv.y, _k, _Lacunarity, _h, false), 1)),
						   normalize(float3(1, -GetFractalNoiseDerivative(_LatticeTex, _LatticeSize, v.uv.x, v.uv.y, _k, _Lacunarity, _h, true), 0))));

				o.uv = TRANSFORM_TEX(float2(v.id % _TerrainSize / (_TerrainSize-1),
							  v.id / _TerrainSize / (_TerrainSize - 1)), _WaterTex);

				//float3 toLight = normalize(WorldSpaceLightDir(objSpaceVert));
				float3 toLight = normalize(float3(_LightPos.x, _LightPos.y, _LightPos.z) - o.vertex);
				float i = dot(toLight, o.normal) * _Mdiff * 1 + _Mambient;
				float3 currentColor = float3((val+1)/2, 149/255.0, 22/255.0);
				//float3 currentColor = float3(0.5, 0.5, 0.5);
				o.color =  i*fixed4(currentColor.r, currentColor.g, currentColor.z, 0);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_WaterTex, i.uv);
				//fixed4 col = fixed4(i.normal.x, i.normal.y, i.normal.z, 1);
				//fixed4 col = i.color;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

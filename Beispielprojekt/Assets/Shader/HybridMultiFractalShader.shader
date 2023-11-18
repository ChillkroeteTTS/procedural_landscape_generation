Shader "Custom/HybridMultrifractal"
{
	Properties
	{
		_WaterTex("WaterTed", 2D) = "white" {}
		_SandTex("SandTex", 2D) = "white" {}
		_GrassTex("GrassTex", 2D) = "white" {}
		_MountainTex("MountainTex", 2D) = "white" {}
		_PeakTex("PeakTex", 2D) = "white" {}
		_TexHeightMax("TexHeightMax", Float) = 1
		_TexHeightMin("TexHeightMin", Float) = 0
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
		_Timer("Timer", Float) = 0
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
			#include "HybridMultiFractal.cginc"
			#pragma target 4.0
			float3 _LightPos;
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

			float _Timer;

			sampler2D _LatticeTex;

			sampler2D _MainTex;

			
			sampler2D _WaterTex;
			float4 _WaterTex_ST;
			sampler2D _SandTex;
			float4 _SandTex_ST;
			sampler2D _GrassTex;
			float4 _GrassTex_ST;
			sampler2D _MountainTex;
			float4 _MountainTex_ST;
			sampler2D _PeakTex;
			float4 _PeakTex_ST;

			float _TexHeightMax;
			float _TexHeightMin;
			#include "UnityCG.cginc"
			#include "Helper.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float2 uvLatticeTexture : TEXCOORD1;
				uint id : SV_VertexID;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				fixed4 color : TEXCOORD2;
				float val : Float;
			};

			fixed4 getTexColor(float2 ntuv, uint index) {
				fixed2 uv = TRANSFORM_TEX(ntuv, _GrassTex);
				if (index <= 0)  {
					uv = TRANSFORM_TEX(ntuv, _WaterTex);
					return tex2D(_WaterTex, fixed4(uv.x, uv.y, 0, 0));
				}
				if (index == 1) {
					uv = TRANSFORM_TEX(ntuv, _SandTex);
					return tex2D(_SandTex, fixed4(uv.x, uv.y, 0, 0));
				}
				if (index == 2) {
					uv = TRANSFORM_TEX(ntuv, _GrassTex);
					return tex2D(_GrassTex, fixed4(uv.x, uv.y, 0, 0));
				}
				if (index == 3) {
					uv = TRANSFORM_TEX(ntuv, _MountainTex);
					return tex2D(_MountainTex, fixed4(uv.x, uv.y, 0, 0));
				}
				if (index >= 4) {
					uv = TRANSFORM_TEX(ntuv, _PeakTex);
					return tex2D(_PeakTex, fixed4(uv.x, uv.y, 0, 0));
				}
				return fixed4(1, 1, 1, 1);
			}
			 /*
			 t: [0-1]*/
			fixed4 mapHeightToColor(float2 uv, float t) {
				t = min(max(0, t), 1);
				float tInRange = t * 4;
				fixed4 color1 = getTexColor(uv, (uint)floor(tInRange)),
					   color2 = getTexColor(uv, (uint)floor(tInRange) +1);
				
				//return interpolateColor(color1, color2, FadeFunction(tInRange - floor(tInRange)));
				return interpolateColor(color1, color2, tInRange - floor(tInRange));
			}

			
			v2f vert (appdata v)
			{
				v2f o;
				float offset = 1/250.0, //250 = resolution
				latticeScale = 2/_LatticeSize; //4 = Part of the lattice thats used for the terrain 
				float val = GetFractalNoiseHeight(_LatticeTex, _LatticeSize, (v.uvLatticeTexture.x + _Timer)*latticeScale, v.uvLatticeTexture.y*latticeScale, _k, _Lacunarity, _h),
					   valBeforeX = GetFractalNoiseHeight(_LatticeTex, _LatticeSize, (v.uvLatticeTexture.x - offset)*latticeScale, v.uvLatticeTexture.y*latticeScale, _k, _Lacunarity, _h),
					   valAfterX = GetFractalNoiseHeight(_LatticeTex, _LatticeSize, (v.uvLatticeTexture.x + offset)*latticeScale, v.uvLatticeTexture.y*latticeScale, _k, _Lacunarity, _h),
					valBeforeY = GetFractalNoiseHeight(_LatticeTex, _LatticeSize, v.uvLatticeTexture.x*latticeScale, (v.uvLatticeTexture.y - offset)*latticeScale, _k, _Lacunarity, _h),
					valAfterY = GetFractalNoiseHeight(_LatticeTex, _LatticeSize, v.uvLatticeTexture.x*latticeScale, (v.uvLatticeTexture.y + offset)*latticeScale, _k, _Lacunarity, _h);
				float4 objSpaceVert = float4(v.vertex.x,
											_Height * val,
											v.vertex.z, v.vertex.w);
				float4 worldSpaceVert = mul(_Object2World, objSpaceVert);
				o.vertex = mul(UNITY_MATRIX_MVP, objSpaceVert);


				/*o.normal = normalize(cross(normalize(float3(0, -GetFractalNoiseDerivative(_LatticeTex, _LatticeSize, v.uv.x, v.uv.y, _k, _Lacunarity, _h, false), 1)),
						   normalize(float3(1, -GetFractalNoiseDerivative(_LatticeTex, _LatticeSize, v.uv.x, v.uv.y, _k, _Lacunarity, _h, true), 0))));*/
				o.normal = normalize(cross(
					normalize(float3(offset * 2, valAfterX - valBeforeX, 0)),
					normalize(float3(0, valAfterY - valBeforeY, offset * 2))
					)
					);

				o.uv = v.uv;
				//o.uv = v.uvLatticeTexture;

				//float3 toLight = normalize(WorldSpaceLightDir(objSpaceVert));
				float3 toLight = normalize(_LightPos - worldSpaceVert);
				float i = min(1, dot(toLight, o.normal) * _Mdiff * 1 + _Mambient);


				o.color = fixed4(i, i, i, 1);
				//o.color = fixed4(toLight.x, toLight.y, toLight.z, 1);
				o.val = val;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = i.color * mapHeightToColor(i.uv,  ((i.val + 1) / 2 - _TexHeightMin) / (_TexHeightMax- _TexHeightMin));
				//fixed col = tex2D(_LatticeTex,  fixed4(i.uv.x, i.uv.y, 0, 0))
				//fixed4 col = fixed4(i.normal.x, i.normal.y, i.normal.z, 1);
				//fixed4 col = i.color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

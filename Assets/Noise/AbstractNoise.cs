using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngineInternal;
using Random = UnityEngine.Random;

public abstract class AbstractNoise
{

    public delegate float OneDAuxilaryFunction(int x);
    public delegate float TwoDFadeFunction(float t);

    public delegate float TransformDelegate(float height, float val, float k);

    public TwoDFadeFunction FadeFunction;

    public TransformDelegate TransformFunction;

    protected int AuxSize;

    public List<List<float>> ListenerLists = new List<List<float>>();

    public List<List<float>> LatticeListener = new List<List<float>>();


    public AbstractNoise(int seed=0, int auxSize=6) {
        AuxSize = auxSize;
        FadeFunction = (x) => { return x * x * x * (x * (x * 6 - 15) + 10); };
        TransformFunction = (height, val, k) => {
            if (height < 0) {
                return val * Mathf.Abs(height);
            }
            return val;
            /*if (k > 0) {
                return val * height;
            }
            return val;*/
        };
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">x coordinate[0-1]</param>
    /// <param name="y">y coordinate[0-1]</param>
    /// <param name="k">Fractal steps</param>
    /// <param name="Lacunarity">Lacunarity</param>
    /// <returns></returns>
    public float GetNoiseValue2D(float x, float y, int k, float Lacunarity, float h) {
        if (k < 0) {
            return 0;
        }
        float val = S1F(x*Mathf.Pow(Lacunarity, k), y*Mathf.Pow(Lacunarity, k))/Mathf.Pow(Lacunarity, k*h),
              currHeight = GetNoiseValue2D(x, y, --k, Lacunarity, h);

        val = TransformFunction(currHeight, val, k);


        // Value tracking
        if (k >= 0 && LatticeListener[k] != null && x == 0 && y == 0)
            for(int xAux=0; xAux <= (AuxSize-1)*(k+1); xAux++)
                LatticeListener[k].Add(GetLatticeFuncFloat(xAux%AuxSize, 0) / Mathf.Pow(2, k));
        if (k >= 0 && ListenerLists[k] != null && y==0)
            ListenerLists[k].Add(val);

        return currHeight + val;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">x coordinate[0-1]</param>
    /// <param name="y">y coordinate[0-1]</param>
    /// <param name="k">Fractal steps</param>
    /// <param name="r">Lacunarity</param>
    /// <returns></returns>
    public float GetNoiseValue2DDomainWarped(float x, float y, int kmax, float lacunarity, float h, Vector2 c1, Vector2 c2, Vector2 c3, Vector2 r0, Vector2 t0) {
        float rough0 = 1/lacunarity, // General roughness value
            resHeight = 0.0001f;
        Vector2 ri = r0;

        for (int k = 0; k <= kmax; k++) {
            float roughI = resHeight*rough0; //Height dependent roughness
            Vector2 rNext = (c1*roughI + c2 + ri).normalized,
                    tNext = c3 * resHeight / roughI;
            float s1X = rNext.x*x - rNext.y*y + tNext.x,
                s1Y = rNext.y*x + rNext.x*y + tNext.y;
            //Debug.Log(s1X + " " + s1Y);
            float val = S1F(s1X * Mathf.Pow(lacunarity, k), s1Y * Mathf.Pow(lacunarity, k)) / Mathf.Pow(lacunarity, k*h); // calc val for current frequency


            // Value Tracking
            if (k >= 0 && LatticeListener[k] != null && x == 0 && y == 0)
                for (int xAux = 0; xAux <= (AuxSize - 1) * (k + 1); xAux++)
                    LatticeListener[k].Add(GetLatticeFuncFloat(xAux % AuxSize, 0) / Mathf.Pow(2, k));
            if (k >= 0 && ListenerLists[k] != null && y == 0)
                ListenerLists[k].Add(val);

            resHeight += val;
        }



        return resHeight;
    }


    protected abstract float GetLatticeFuncFloat(int x, int y);
    

    /// <summary>
    /// Function transformes the given coordinates to ones that fit to the noise function and calls it afterwards
    /// </summary>
    /// <param name="x">x Coordinate in range 0 - 1</param>
    /// <param name="y">y Coordinate in range 0 - 1</param>
    /// <returns></returns>
    protected float S1F(float x, float y) {
        float truncX = x - Mathf.Floor(x),
              truncY = y - Mathf.Floor(y);
        /*if (x != 1f)
            truncX = x <= 1f
                ? x - Mathf.Floor(x)
                : (Mathf.FloorToInt(x)%2 != 0 ? 1 - (x - Mathf.Floor(x)) : x - Mathf.Floor(x));

        if (y != 1f)
            truncY = y <= 1f 
                ? y - Mathf.Floor(y) 
                : (Mathf.FloorToInt(y) % 2 != 0 ? 1 - (y - Mathf.Floor(y)) : y - Mathf.Floor(y));*/


        return S(truncX*(AuxSize-1), truncY * (AuxSize - 1));
    }


    protected abstract float S(float x, float y);

}

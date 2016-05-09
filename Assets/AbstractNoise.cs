using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngineInternal;
using Random = UnityEngine.Random;

public abstract class AbstractNoise
{

    public delegate float OneDAuxilaryFunction(int x);
    public delegate float TwoDFadeFunction(float t);

    public TwoDFadeFunction FadeFunction;

    protected int AuxSize;

    public List<List<float>> ListenerLists = new List<List<float>>();

    public List<List<float>> LatticeListener = new List<List<float>>();


    public AbstractNoise(int seed=0, int auxSize=6) {
        AuxSize = auxSize;
        FadeFunction = (x) => { return x * x * x * (x * (x * 6 - 15) + 10); };
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">x coordinate[0-1]</param>
    /// <param name="y">y coordinate[0-1]</param>
    /// <param name="k">Fractal steps</param>
    /// <param name="r">Lacunarity</param>
    /// <returns></returns>
    public float GetNoiseValue2D(float x, float y, int k, float r=2) {
        if (k < 0) {
            return 0;
        }
        float val = S1F(x*Mathf.Pow(r, k), y*Mathf.Pow(r, k))/Mathf.Pow(r, k);

        if (k >= 0 && LatticeListener[k] != null && x == 0 && y == 0)
            for(int xAux=0; xAux <= (AuxSize-1)*(k+1); xAux++)
                LatticeListener[k].Add(GetLatticeFuncFloat(xAux%AuxSize, 0) / Mathf.Pow(2, k));
        if (k >= 0 && ListenerLists[k] != null && y==0)
            ListenerLists[k].Add(val);

        return GetNoiseValue2D(x,y , --k, r) + val;
    }


    protected abstract float GetLatticeFuncFloat(int x, int y);
    

    /// <summary>
    /// Function transformes the given coordinates to ones that fit to the noise function and calls it afterwards
    /// </summary>
    /// <param name="x">x Coordinate in range 0 - 1</param>
    /// <param name="y">y Coordinate in range 0 - 1</param>
    /// <returns></returns>
    protected float S1F(float x, float y) {
        float truncX = x,
              truncY = y;
        if (x != 1f)
            truncX = x <= 1f
                ? x - Mathf.Floor(x)
                : (Mathf.FloorToInt(x)%2 != 0 ? 1 - (x - Mathf.Floor(x)) : x - Mathf.Floor(x));

        if (y != 1f)
            truncY = y <= 1f 
                ? y - Mathf.Floor(y) 
                : (Mathf.FloorToInt(y) % 2 != 0 ? 1 - (y - Mathf.Floor(y)) : y - Mathf.Floor(y));

        return S(truncX*(AuxSize-1), truncY * (AuxSize - 1));
    }


    protected abstract float S(float x, float y);

}

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

    public delegate Vector2 NoiseCalcFunction(float x, float y, int k, float lacunarity, float h, NoiseCalcFunction noiseCalcFunc);

    public TwoDFadeFunction FadeFunction;

    public TransformDelegate TransformFunction;

    public NoiseCalcFunction NoiseFunction;

    protected int AuxSize;

    public List<List<float>> ListenerLists = new List<List<float>>();

    public List<List<float>> LatticeListener = new List<List<float>>();

    public int Seed;

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
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="k"></param>
    /// <param name="lacunarity"></param>
    /// <param name="h"></param>
    /// <param name="noiseCalcFunc"></param>
    /// <returns>x value is current iteration value and y is current height</returns>
    public Vector2 NoiseFuncPlain(float x, float y, int k, float lacunarity, float h, NoiseCalcFunction noiseCalcFunc) {
        float val = S1F(x * Mathf.Pow(lacunarity, k), y * Mathf.Pow(lacunarity, k)) / Mathf.Pow(lacunarity, k * h),
              currHeight = GetNoiseValue2D(x, y, --k, lacunarity, h, noiseCalcFunc);

        return new Vector2(val, currHeight);
    }


    public Vector2 NoiseFuncRidged(float x, float y, int k, float lacunarity, float h, NoiseCalcFunction noiseCalcFunc) {
        float val = S1F(x * Mathf.Pow(lacunarity, k), y * Mathf.Pow(lacunarity, k)) / (Mathf.Pow(lacunarity, k) * Mathf.Pow(lacunarity, (1+2*h)/2)),
              currHeight = GetNoiseValue2D(x, y, --k, lacunarity, h, noiseCalcFunc);

        //return new Vector2(1 - Mathf.Abs(val), currHeight);
        return new Vector2(val, currHeight);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">x coordinate[0-1]</param>
    /// <param name="y">y coordinate[0-1]</param>
    /// <param name="k">Fractal steps</param>
    /// <param name="lacunarity">Lacunarity</param>
    /// <param name="noiseCalcFunc"></param>
    /// <returns></returns>
    public float GetNoiseValue2D(float x, float y, int k, float lacunarity, float h, NoiseCalcFunction noiseCalcFunc) {
        if (k < 0) {
            return 0;
        }
        Vector2 val = noiseCalcFunc(x , y, k, lacunarity, h, noiseCalcFunc);


        // Value tracking
        if (k >= 0 && LatticeListener[k] != null && x == 0 && y == 0)
            for(int xAux=0; xAux <= (AuxSize-1)*(k+1); xAux++)
                LatticeListener[k].Add(GetLatticeFuncFloat(xAux%AuxSize, 0) / Mathf.Pow(2, k*h));
        if (k >= 0 && ListenerLists[k] != null && y==0)
            ListenerLists[k].Add(val.x);

        return val.x + val.y;
    }



    /// <summary>
    /// After https://graphics.tudelft.nl/Publications-new/2008/Car08/Thesis-Giliam-Final.pdf p. 124,
    /// uses a wrong derivative of perlin noise to create a turbulence function.
    /// </summary>
    /// <param name="n">Perlin value for current </param>
    /// <param name="lambdaX">X Coord transformed</param>
    /// <param name="lambdaY">Y Coord transformed</param>
    /// <param name="k">Rec depth</param>
    /// <returns>Turbulence factor</returns>
    private float TurbulenceWithWrongDerivative(float n, float lambdaX, float lambdaY, float k) {
        return 0;
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

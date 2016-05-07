﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public class ValueNoise  {

    public delegate float OneDAuxilaryFunction(int x);
    public delegate float TwoDAuxilaryFunction(int x, int y);

    public float MaxVal = 0.01f;

    public TwoDAuxilaryFunction AuxFunc;

    public delegate float TwoDFadeFunction(float t);

    private float[,] _auxArray;

    public int AuxSize;

    private TwoDFadeFunction _fadeFunction;

    public List<List<float>> ListenerLists = new List<List<float>>();
    public List<List<float>> AuxListener = new List<List<float>>();

    public ValueNoise(int seed=0, int auxSize=6, float maxVal=0.1f) {
        AuxSize = auxSize;
        MaxVal = maxVal;
        _auxArray = new float[AuxSize, AuxSize];
        AuxFunc = GetFromAuxArray;
        _fadeFunction = (x) => { return x * x * x * (x * (x * 6 - 15) + 10); };

        TestValueNoise();
        if (seed==0)
            FillAuxArray2D();
        else 
            FillAuxArray2D(seed);
    }


    public float GetNoiseValue2D(float x, float y, int r) {
        if (r < 0) {
            return 0;
        }
        float val = S1F(x*Mathf.Pow(2, r), y*Mathf.Pow(2, r))/Mathf.Pow(2, r);

        if (r >= 0 && AuxListener[r] != null && x == 0 && y == 0)
            for(int xAux=0; xAux <= (AuxSize-1)*(r+1); xAux++)
                AuxListener[r].Add(AuxFunc(xAux%AuxSize, 0) / Mathf.Pow(2, r));
        if (r >= 0 && ListenerLists[r] != null && y==0)
            ListenerLists[r].Add(val);

        return GetNoiseValue2D(x,y , --r) + val;
    }


    /// <summary>
    /// Function transformes the given coordinates to ones that fit to the noise function and calls it afterwards
    /// </summary>
    /// <param name="x">x Coordinate in range 0 - 1</param>
    /// <param name="y">y Coordinate in range 0 - 1</param>
    /// <returns></returns>
    private float S1F(float x, float y) {
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

        //return S1(truncX*(AuxSize-1), truncY * (AuxSize - 1));
        return S1Gradient(truncX * (AuxSize - 1), truncY * (AuxSize - 1));
    }


    private float S1(float x, float y) {
        int floorX = Mathf.FloorToInt(x),
            ceilX = Mathf.CeilToInt(x),
            floorY = Mathf.FloorToInt(y),
            ceilY = Mathf.CeilToInt(y);
        float tx = x - floorX,
            ty = y - floorY;

        float retVal = (_fadeFunction(1 - ty)*
                        (AuxFunc(floorX, floorY)*_fadeFunction(1 - tx) + _fadeFunction(tx)*AuxFunc(ceilX, floorY))
                        +
                        _fadeFunction(ty)*
                        (AuxFunc(floorX, ceilY)*_fadeFunction(1 - tx) + _fadeFunction(tx)*AuxFunc(ceilX, ceilY)));

        //Debug.Log(String.Format("x: {0} y: {1} fadefunc for adjusted x: {2} and aux top left {3}  ->  {4}", 
        //            tx, ty, _fadeFunction(tx), _auxFunc(floorX, floorY), retVal));

        return retVal;
    }


    private float S1Gradient(float x, float y) {
        int floorX = Mathf.FloorToInt(x),
            ceilX = Mathf.CeilToInt(x),
            floorY = Mathf.FloorToInt(y),
            ceilY = Mathf.CeilToInt(y);
        float tx = x - floorX,
              ty = y - floorY;

        float retVal = (_fadeFunction(1 - ty) *
                        (AuxFunc(floorX, floorY) * tx * _fadeFunction(1 - tx) + _fadeFunction(tx) * (tx-1) * AuxFunc(ceilX, floorY))
                        +
                        _fadeFunction(ty) *
                        (AuxFunc(floorX, ceilY) * ty * _fadeFunction(1 - tx) + _fadeFunction(tx) * (ty-1) * AuxFunc(ceilX, ceilY)));

        //Debug.Log(String.Format("x: {0} y: {1} fadefunc for adjusted x: {2} and aux top left {3}  ->  {4}", 
        //            tx, ty, _fadeFunction(tx), _auxFunc(floorX, floorY), retVal));

        return retVal;
    }


    private float GetFromAuxArray(int x, int y=0) {
        return _auxArray[x, y];
    }


    private void FillAuxArray2D(int seed=0) {
        if (seed != 0)
            Random.seed = seed;

        for (int i = 0; i <= _auxArray.GetUpperBound(0); i++) {
            for (int j = 0; j <= _auxArray.GetUpperBound(1); j++) {
                _auxArray[i, j] = Random.value * 2 * MaxVal  - MaxVal;
            }
        }
    }


    private void TestValueNoise() {
        FillAuxArray2D();

        for (int i = 0; i < AuxSize; i++) {
            Assert.AreEqual(AuxFunc(i, 0), S1(i, 0));
            Assert.AreEqual(AuxFunc(i, 0), S1F(i / (float)(AuxSize-1), 0), "expected: "+ AuxFunc(i, 0)+" actual: "+ S1F(i / (float)(AuxSize - 1), 0)+" with i: " +i+" and x: "+ i / (float)(AuxSize - 1));
        }
    }
}

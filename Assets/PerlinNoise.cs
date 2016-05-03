using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public class ValueNoise  {

    public delegate float OneDAuxilaryFunction(int x);
    public delegate float TwoDAuxilaryFunction(int x, int y);

    public float MaxVal = 0.005f;

    private TwoDAuxilaryFunction _auxFunc;

    public delegate float TwoDFadeFunction(float t);

    private float[,] _auxArray;

    private int _auxSize = 36;
    private TwoDFadeFunction _fadeFunction;

    public ValueNoise() {
        _auxArray = new float[_auxSize, _auxSize];
        FillAuxArray2D();
        _auxFunc = GetFromAuxArray;
        _fadeFunction = (x) => { return x * x * x * (x * (x * 6 - 15) + 10); };
    }

    private void CalcHeightmap() {
        FillAuxArray2D();
    }


    public float GetNoiseValue2D(float x, float y, int r = 3) {
        if (r <= 0) {
            return 0;
        }
        return GetNoiseValue2D(x,y , --r) + S1F(x*Mathf.Pow(2, r), y*Mathf.Pow(2, r)) / Mathf.Pow(2, 3);
    }


    private float S1F(float x, float y) {
        x = x - Mathf.Floor(x);
        y = y - Mathf.Floor(y);

        return S1(x*(_auxSize-1), y * (_auxSize - 1));
    }


    private float S1(float x, float y) {
        int floorX = Mathf.FloorToInt(x),
            ceilX = Mathf.CeilToInt(x),
            floorY = Mathf.CeilToInt(y),
            ceilY = Mathf.FloorToInt(y);
        float tx = x - floorX,
            ty = y - floorY;
        //Debug.Log(String.Format("x: {0} y: {1} fadefunc for adjusted x: {2} and aux top left {3}", x, y, _fadeFunction(tx), _auxFunc(floorX, floorY)));
        return (_fadeFunction(1-ty) * (_auxFunc(floorX, floorY) * _fadeFunction(1-tx) + _fadeFunction(tx) * _auxFunc(ceilX, floorY))
                + _fadeFunction(ty) * (_auxFunc(floorX, ceilY) * _fadeFunction(1-tx) + _fadeFunction(tx) * _auxFunc(ceilX, ceilY)));
    }


    private float GetFromAuxArray(int x, int y=0) {
        return _auxArray[x, y];
    }


    private void FillAuxArray2D(int seed=0) {
        if (seed == 0)
            Random.seed = seed;

        for (int i = 0; i < _auxArray.GetUpperBound(0); i++) {
            for (int j = 0; j < _auxArray.GetUpperBound(1); j++) {
                _auxArray[i, j] = Random.value * MaxVal;
            }
        }
    }
}

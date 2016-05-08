using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public class ValueNoise : AbstractNoise {
    public delegate float TwoDAuxilaryFunction(int x, int y);

    public TwoDAuxilaryFunction AuxFunc;

    private float[,] _auxArray;

    public ValueNoise(int seed = 0, int auxSize = 6, float maxVal = 0.1f) : base(seed, auxSize, maxVal) {
        _auxArray = new float[AuxSize, AuxSize];
        AuxFunc = GetFromAuxArray;
        if (seed == 0)
            FillAuxArray2D();
        else
            FillAuxArray2D(seed);
        TestValueNoise();
    }

    protected override float GetAuxFuncFloat(int x, int y) {
        return AuxFunc(x, y);
    }

    protected override float S1(float x, float y) {
        int floorX = Mathf.FloorToInt(x),
            ceilX = Mathf.CeilToInt(x),
            floorY = Mathf.FloorToInt(y),
            ceilY = Mathf.CeilToInt(y);
        float tx = x - floorX,
            ty = y - floorY;

        float retVal = (FadeFunction(1 - ty)*
                        (AuxFunc(floorX, floorY)*FadeFunction(1 - tx) + FadeFunction(tx)*AuxFunc(ceilX, floorY))
                        +
                        FadeFunction(ty)*
                        (AuxFunc(floorX, ceilY)*FadeFunction(1 - tx) + FadeFunction(tx)*AuxFunc(ceilX, ceilY)));

        //Debug.Log(String.Format("x: {0} y: {1} fadefunc for adjusted x: {2} and aux top left {3}  ->  {4}", 
        //            tx, ty, FadeFunction(tx), _auxFunc(floorX, floorY), retVal));

        return retVal;
    }


    private float GetFromAuxArray(int x, int y = 0) {
        return _auxArray[x, y];
    }


    private void FillAuxArray2D(int seed = 0) {
        if (seed != 0)
            Random.seed = seed;

        for (int i = 0; i <= _auxArray.GetUpperBound(0); i++) {
            for (int j = 0; j <= _auxArray.GetUpperBound(1); j++) {
                _auxArray[i, j] = Random.value * 2 * MaxVal - MaxVal;
            }
        }
    }


    private void TestValueNoise() {
        for (int i = 0; i < AuxSize; i++) {
            Assert.AreEqual(AuxFunc(i, 0), S1(i, 0));
            Assert.AreEqual(AuxFunc(i, 0), S1F(i / (float)(AuxSize - 1), 0), "expected: " + AuxFunc(i, 0) + " actual: " + S1F(i / (float)(AuxSize - 1), 0) + " with i: " + i + " and x: " + i / (float)(AuxSize - 1));
        }
    }
}

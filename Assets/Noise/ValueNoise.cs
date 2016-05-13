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

    public TwoDAuxilaryFunction LatticeFunc;

    private float[,] _latticeArray;

    public ValueNoise(int seed = 0, int auxSize = 6) : base(seed, auxSize) {
        _latticeArray = new float[AuxSize, AuxSize];
        LatticeFunc = GetFromLatticeArray;
        if (seed == 0)
            FillLatticeArray2D();
        else
            FillLatticeArray2D(seed);
        TestValueNoise();
    }

    protected override float GetLatticeFuncFloat(int x, int y) {
        return LatticeFunc(x, y);
    }

    protected override float S(float x, float y) {
        int floorX = Mathf.FloorToInt(x),
            ceilX = Mathf.CeilToInt(x),
            floorY = Mathf.FloorToInt(y),
            ceilY = Mathf.CeilToInt(y);
        float tx = x - floorX,
            ty = y - floorY;

        float retVal = FadeFunction(1 - ty) *
                       (LatticeFunc(floorX, floorY) * FadeFunction(1 - tx) + FadeFunction(tx) * LatticeFunc(ceilX, floorY))
                       +
                       FadeFunction(ty) *
                       (LatticeFunc(floorX, ceilY)*FadeFunction(1 - tx) + FadeFunction(tx)*LatticeFunc(ceilX, ceilY));

        return retVal;
    }


    private float GetFromLatticeArray(int x, int y = 0) {
        return _latticeArray[x, y];
    }


    private void FillLatticeArray2D(int seed = 0) {
        if (seed != 0)
            Random.seed = seed;

        for (int i = 0; i <= _latticeArray.GetUpperBound(0); i++) {
            for (int j = 0; j <= _latticeArray.GetUpperBound(1); j++) {
                if (i == _latticeArray.GetUpperBound(0))
                    _latticeArray[i, j] = _latticeArray[0, j];
                else if (j == _latticeArray.GetUpperBound(1))
                    _latticeArray[i, j] = _latticeArray[i, 0];
                else if (i == _latticeArray.GetUpperBound(0)
                         && j == _latticeArray.GetUpperBound(1))
                    _latticeArray[i, j] = _latticeArray[0, 0];
                else
                    _latticeArray[i, j] = Random.value * 2 - 1;
            }
        }
    }


    private void TestValueNoise() {
        for (int i = 0; i < AuxSize; i++) {
            Assert.AreEqual(LatticeFunc(i, 0), S(i, 0));
            Assert.AreEqual(LatticeFunc(i, 0), S1F(i / (float)(AuxSize - 1), 0), "expected: " + LatticeFunc(i, 0) + " actual: " + S1F(i / (float)(AuxSize - 1), 0) + " with i: " + i + " and x: " + i / (float)(AuxSize - 1));
        }
    }
}

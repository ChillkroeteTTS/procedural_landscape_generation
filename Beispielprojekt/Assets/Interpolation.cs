using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Interpolation {

    public static float LinearInterpolation(float q1, float q2, float t) {
        return (1 - t)*q1 + t*q2;
    }


    public static float BillinearInterpolation(float q1, float q2, float q3, float q4, float tx, float ty) {
        return (q1 * (1 - tx) + tx * q2) * (1 - ty) + ty * (q3 * (1 - tx) + tx * q4);
    }


    public static void TestBillinearInterpolation() {
        Assert.IsTrue(Math.Abs(50 - BillinearInterpolation(0f, 100f, 0f, 100f, 0.5f, 0.5f)) < Mathf.Epsilon);
        Assert.IsTrue(Math.Abs(50 - BillinearInterpolation(0f, 100f, 0f, 100f, 0.5f, 0f)) < Mathf.Epsilon);
        Assert.IsTrue(Math.Abs(100 - BillinearInterpolation(0f, 100f, 0f, 100f, 1f, 0.5f)) < Mathf.Epsilon);
        Assert.IsTrue(Math.Abs(0 - BillinearInterpolation(0f, 100f, 0f, 100f, 0, 0f)) < Mathf.Epsilon);
        Assert.IsTrue(Math.Abs(100 - BillinearInterpolation(0f, 100f, 0f, 100f, 1, 1f)) < Mathf.Epsilon);
        Assert.IsTrue(Math.Abs(25 - BillinearInterpolation(0f, 100f, 0f, 100f, 0.25f, 0f)) < Mathf.Epsilon);
        Assert.IsTrue(Math.Abs(25 - BillinearInterpolation(100f, 0f, 0f, 100f, 1f, 0.25f)) < Mathf.Epsilon);
        Assert.IsTrue(Math.Abs(25 - BillinearInterpolation(0f, 100f, 0f, 100f, 0.25f, 0.25f)) < Mathf.Epsilon);
    }
}

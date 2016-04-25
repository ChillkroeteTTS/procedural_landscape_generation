using UnityEngine;
using System.Collections;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Noise : MonoBehaviour {

    public float MaxHeight {
        get { return _maxHeight; }
        set {
            _maxHeight = value;
            StartCoroutine("CalcHeightmap");
        }
    }

    int _heightMapWidth;

    private int _heightMapHeight;

    private float[,] _heightmap;
    [SerializeField]
    private float _maxHeight = .01f;

    // Use this for initialization
    void Start() {
        TestBillinearInterpolation();
        //CalcHeightmap();
    }

    // Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine("CalcHeightmap");

	}


    private IEnumerator CalcHeightmap() {
        _heightMapHeight = gameObject.GetComponent<Terrain>().terrainData.heightmapResolution = 32;
        _heightMapWidth = gameObject.GetComponent<Terrain>().terrainData.heightmapWidth;
        _heightMapHeight = gameObject.GetComponent<Terrain>().terrainData.heightmapHeight;
        _heightmap = new float[_heightMapWidth, _heightMapHeight];
        Debug.Log(_heightMapWidth);

        float[,] auxArr = new float[10, 10];
        FillRnd(ref auxArr, 10, 10, MaxHeight);

        int lastPercent = 0;
        for (int x = 0; x < _heightMapWidth; x++) {

            float processed = x / (float) _heightMapWidth * 100f;
            if (Mathf.FloorToInt(processed) > lastPercent) {
                lastPercent = Mathf.CeilToInt(processed);
                Debug.Log("processed "+processed+"%");
            }

            for (int y = 0; y < _heightMapHeight; y++) {
                Profiler.BeginSample("AuxFunc");
                _heightmap[x, y] = GetAuxFuncValue(auxArr, 10, 10, x / (float)_heightMapWidth, y / (float) _heightMapHeight);
                //Debug.Log(_heightmap[x, y]);
                Profiler.EndSample();
                
                yield return null;
               /* Profiler.BeginSample("AuxFuncOpt");
                _heightmap[x, y] = GetAuxFuncValueOpt(auxArr, 10, 10, x / _heightMapWidth, y / _heightMapHeight);
                Profiler.EndSample();*/
            }
        }
        gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, _heightmap);
    }


    private void FillRnd(ref float[,] arr, int width, int height, float range) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                arr[x, y] = Random.Range(0f, range);
            }
        }
    }


    private float GetAuxFuncValue(float[,] rndArr, int width, int height, float x, float y) {
        //TODO xand y is 0-1
        Assert.IsTrue(x <= 1f);
        Assert.IsTrue(y <= 1f);
        Assert.IsTrue(y >= 0);
        Assert.IsTrue(x >= 0);
        
        // Map x to 0 until width-1;  0 until height-1
        x = x*(width - 1);
        y = y*(height - 1);

        int floorX = Math.Max(0, Mathf.FloorToInt(x)),
            ceilX = Math.Min(width, Mathf.FloorToInt(x + 1f)),
            floorY = Math.Max(0, Mathf.FloorToInt(y)),
            ceilY = Math.Min(height, Mathf.FloorToInt(y + 1f));
        if (x >= (width-1))
            x = width - 1;
        if (y >= (height - 1))
            y = height - 1;
        float q1 = rndArr[floorX, floorY],
              q2 = rndArr[ceilX, floorY],
              q3 = rndArr[floorX,ceilY],
              q4 = rndArr[ceilX, ceilY];

        return BillinearInterpolation(q1, q2, q3, q4,
            (x - floorX) /(ceilX - floorX),
            (y - floorY) /(ceilY - floorY));
    }


    private float BillinearInterpolation(float q1, float q2, float q3, float q4, float tx, float ty) {
        return (q1*(1-tx) + tx*q2)*(1-ty) + ty*(q3*(1-tx) + tx*q4);
    }

    private void TestBillinearInterpolation() {
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

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
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

    public int Seed;

    public bool UseRandomSeed;

    public bool ProcessingOutput = false;

    public int RecursionDepth;

    public float Lacunarity = 2;

    public int AuxSize = 6;

    public bool RuntimeCalculation = false;

    int _heightMapWidth;

    private int _heightMapHeight;

    private float[,] _heightmap;
    [SerializeField]
    private float _maxHeight = .01f;

    [SerializeField] private List<NoiseWindow> _windows;
    private List<GameObject> _spheres;
    [SerializeField] private NoiseWindow _mainWindow;

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

    void FixedUpdate() {
        
    }


    private IEnumerator CalcHeightmap() {
        TerrainData _terrainData = gameObject.GetComponent<Terrain>().terrainData;
        //_heightMapHeight = _terrainData.heightmapResolution = 511;
        _heightMapHeight = _terrainData.heightmapResolution = 128;
        _heightMapWidth = _terrainData.heightmapWidth;
        _heightMapHeight = _terrainData.heightmapHeight;
        _heightmap = new float[_heightMapWidth, _heightMapHeight];
        _terrainData.SetHeights(0,0,_heightmap);
        Debug.Log(_heightMapWidth);
        int cnt = 0;

        _mainWindow.ValueList.Capacity = _heightMapHeight;

        //Calculate until Realtime is disabled or if not enabled at all just once
        while (RuntimeCalculation || cnt == 0) {
            /*AbstractNoise perlin = UseRandomSeed 
                                    ? new GradientNoise(auxSize:AuxSize) 
                                    : new GradientNoise(seed:Seed, auxSize: AuxSize);*/
            AbstractNoise perlin = UseRandomSeed 
                                ? new GradientNoise(auxSize:AuxSize) 
                                : new GradientNoise(seed:Seed, auxSize: AuxSize);

            //Destroy old NoiseWIndows
            foreach (NoiseWindow noiseWindow in _windows) {
                Destroy(noiseWindow);
            }

            // CLear window lists
            _windows.Clear();
            _mainWindow.ValueList.Clear();

            // Create new Windows and bind them to noise
            for (int i = 0; i <= RecursionDepth; i++) {
                NoiseWindow window = gameObject.AddComponent<NoiseWindow>();
                window.Id = i+1;
                window.windowRect.position += new Vector2(0, (window.windowRect.height+ window.windowRect.position.y)*(i+1));
                _windows.Add(window);
                perlin.ListenerLists.Add(window.ValueList);
                perlin.LatticeListener.Add(window.AuxList);
            }

            // Clear listener lists
            foreach (List<float> list in perlin.ListenerLists) {
                list.Clear();
                list.Capacity = _heightMapWidth;
            }
            foreach (List<float> list in perlin.LatticeListener) {
                list.Clear();
                list.Capacity = _heightMapWidth;
            }

            int lastPercent = 0;
            //Iterate through heighmap and fetch height values from noise function
            for (int x = 0; x < _heightMapWidth; x++) {

                // Processing output
                if (ProcessingOutput) {
                    float processed = x/(float) _heightMapWidth*100f;
                    if (Mathf.FloorToInt(processed) > lastPercent) {
                        lastPercent = Mathf.CeilToInt(processed);
                        Debug.Log("processed " + processed + "%");
                    }
                }

                for (int y = 0; y < _heightMapHeight; y++) {
                    Profiler.BeginSample("LatticeFunc");
                    float val = perlin.GetNoiseValue2D(x/(float) _heightMapWidth, y/(float) _heightMapHeight,
                        RecursionDepth, Lacunarity);
                    float[,] testArr = new float[1,1];

                    // Add perlin result to resulting noise window
                    if (y==0)
                        _mainWindow.ValueList.Add(val);

                    //_heightmap[x, y] = (val + 1)/2f * MaxHeight;
                    testArr[0, 0] = (val + 1) / 2f * MaxHeight;
                    _terrainData.SetHeights(x, y, testArr);
                    //Debug.Log(_heightmap[x, y]);
                    Profiler.EndSample();

                    /* Profiler.BeginSample("AuxFuncOpt");
                    _heightmap[x, y] = GetAuxFuncValueOpt(auxArr, 10, 10, x / _heightMapWidth, y / _heightMapHeight);
                    Profiler.EndSample();*/
                }
                yield return null;
            }
            //gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, _heightmap);
            cnt++;
            Seed = Random.seed;
        }
    }




    private void FillRnd(ref float[,] arr, int width, int height, float range) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                arr[x, y] = Random.Range(0f, range);
            }
        }
    }


    private float[,] CreateAuxArray() {
        float[,] auxArr = new float[10, 10];
        FillRnd(ref auxArr, 10, 10, MaxHeight);
        return auxArr;
    }


    private float GetAuxFuncValue(float[,] rndArr, int width, int height, float x, float y) {
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

        float q1 = rndArr[floorX, floorY],
              q2 = rndArr[ceilX, floorY],
              q3 = rndArr[floorX,ceilY],
              q4 = rndArr[ceilX, ceilY];

        return BillinearInterpolation(q1, q2, q3, q4,
            (x - floorX) /(ceilX - floorX),
            (y - floorY) /(ceilY - floorY));
    }


    private float BillinearInterpolation(float q1, float q2, float q3, float q4, float tx, float ty) {
        //return (q1*(1-tx) + tx*q2)*(1-ty) + ty*(q3*(1-tx) + tx*q4);
        return (q1 * (1 - tx) + tx * q2) * (1 - ty) + ty * (q3 * (1 - tx) + tx * q4);
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

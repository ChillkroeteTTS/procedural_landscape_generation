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

    public bool ValueNoise = true;

    public int Seed;

    public int CurrentSeed;

    public bool UseRandomSeed;

    public bool ProcessingOutput = false;

    public int RecursionDepth;

    public float Lacunarity = 2;

    public float H = 1;

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

    public Vector2 c1 = new Vector2(0.35f, 0.16f);
    public Vector2 c2 = new Vector2(-.07f, 0.13f);
    public Vector2 c3 = new Vector2(0.11f, 0.17f);
    public Vector2 r0 = new Vector2(1f, 0f);
    public Vector2 t0 = new Vector2(0f, 0f);

    // Use this for initialization
    void Start() {
        Interpolation.TestBillinearInterpolation();
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
        _heightMapHeight = _terrainData.heightmapResolution = 68;
        _heightMapWidth = _terrainData.heightmapWidth;
        _heightMapHeight = _terrainData.heightmapHeight;
        _heightmap = new float[_heightMapWidth, _heightMapHeight];
        _terrainData.SetHeights(0,0,_heightmap);
        Debug.Log(_heightMapWidth);
        int cnt = 0;

        _mainWindow.ValueList.Capacity = _heightMapHeight;

        //Calculate until Realtime is disabled or if not enabled at all just once
        while (RuntimeCalculation || cnt == 0) {
            AbstractNoise perlin;
            if (ValueNoise) {
                perlin = UseRandomSeed
                    ? new ValueNoise(auxSize: AuxSize)
                    : new ValueNoise(seed: Seed, auxSize: AuxSize);
            }
            else {
                perlin = UseRandomSeed 
                    ? new GradientNoise(auxSize:AuxSize) 
                    : new GradientNoise(seed:Seed, auxSize: AuxSize);

            }
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
                    float val = perlin.GetNoiseValue2D(x/(float) (_heightMapWidth-1), y/(float) (_heightMapHeight-1), RecursionDepth, Lacunarity, H, perlin.NoiseFuncPlain);
                    /*float val = perlin.GetNoiseValue2DDomainWarped(x / (float)_heightMapWidth, y / (float)_heightMapHeight,
                                                                   RecursionDepth, Lacunarity, H, c1, c2, c3, r0, t0);*/
                    float[,] testArr = new float[1,1];

                    // Add perlin result to resulting noise window
                    if (x==0)
                        _mainWindow.ValueList.Add(val);

                    //Heightmap[x, y] = (val + 1)/2f * MaxHeight;s
                    testArr[0, 0] = (val + 1) / 2f * MaxHeight;
                    _terrainData.SetHeights(x, y, testArr);
                    //Debug.Log(Heightmap[x, y]);
                    Profiler.EndSample();

                    /* Profiler.BeginSample("AuxFuncOpt");
                    Heightmap[x, y] = GetAuxFuncValueOpt(auxArr, 10, 10, x / _heightMapWidth, y / _heightMapHeight);
                    Profiler.EndSample();*/
                }
                yield return null;
            }
            //gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, Heightmap);
            cnt++;
            CurrentSeed = perlin.Seed;
        }
    }


    private void NormalizeHeightfield() {
        float min = _heightmap[0, 0], max = _heightmap[0, 0];
        foreach (float val in _heightmap) {
            min = val < min ? val : min;
            max = val > max ? val : max;
        }
        for (int x = 0; x < _heightMapWidth; x++) {
            for (int y = 0; y < _heightMapHeight; y++) {
                _heightmap[x, y] = (_heightmap[x, y] / (max - min) + 1) / 2f * MaxHeight;
            }
        }
        GetComponent<Terrain>().terrainData.SetHeights(0, 0, _heightmap);
    }
}

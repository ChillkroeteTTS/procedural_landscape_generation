using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Midpoint_Displacement : MonoBehaviour {

    delegate float RndDelegate();

    public float[,] Heightmap;

    private int _heightmapSize;

    public float MaxHeight = 0.1f;

    public float R = 2;

    public bool UseSteps;

    public float SecondsToWait;

    public Terrain Terrain;
    private bool _goOn = false;

    // Use this for initialization
	void Start () {
	    StartCoroutine("CalcTimer");
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.S))
	        StartCoroutine("CalcDiamondSquare");
        else if (Input.GetKeyDown(KeyCode.Space)) {
            _goOn = true;
        }
    }


    private IEnumerator CalcTimer() {
        while (true) {
            yield return new WaitForSeconds(SecondsToWait);
            _goOn = true;
        }
    }


    private IEnumerator CalcDiamondSquare() {
        _heightmapSize = Terrain.terrainData.heightmapResolution;
        Heightmap = new float[_heightmapSize, _heightmapSize];
        RndDelegate rnd = RndFunc;
        Heightmap[0, 0] = (rnd() + 1)/2;
        Heightmap[_heightmapSize-1, _heightmapSize-1] = (rnd() + 1) / 2;
        Heightmap[0, _heightmapSize-1] = (rnd() + 1) /2;
        Heightmap[_heightmapSize-1, 0] = (rnd() + 1) /2;
        UpdateTerrainHeightmap();
        yield return DiamondSquare(0, 0, Heightmap.GetUpperBound(0), Heightmap.GetUpperBound(1), RndFunc, R, 0);
        if (!UseSteps)
            UpdateTerrainHeightmap();
    }


    private IEnumerator DiamondSquare(int left, int top, int right, int bottom, RndDelegate rnd, float r, int recStep) {
        Assert.IsTrue(Heightmap.GetLength(0) % 2 == 1 && Heightmap.GetLength(1) % 2 == 1);
        
        //Diamond step
        int xCenter = left + (right - left)/2,
            yCenter = top + (bottom - top)/2;

        Heightmap[xCenter, yCenter] = Interpolation.BillinearInterpolation(Heightmap[left, top], Heightmap[right, top],
                                             Heightmap[left, bottom], Heightmap[right, bottom], 0.5f, 0.5f)
                                      + rnd() /Mathf.Pow(r, recStep+1);

        if (UseSteps) {
            _goOn = false;
            while (!_goOn) {
                yield return null;
            }
            UpdateTerrainHeightmap();
            _goOn = false;
        } else {
            yield return null;
        }

        //Square step
        Heightmap[left, yCenter] = Interpolation.LinearInterpolation(Heightmap[left, top], Heightmap[left, bottom], 0.5f) + rnd()  / Mathf.Pow(r, recStep+1);
        Heightmap[xCenter, top] = Interpolation.LinearInterpolation(Heightmap[left, top], Heightmap[right, top], 0.5f) + rnd() / Mathf.Pow(r, recStep+1);
        Heightmap[right, yCenter] = Interpolation.LinearInterpolation(Heightmap[right, top], Heightmap[right, bottom], 0.5f) + rnd() / Mathf.Pow(r, recStep+1);
        Heightmap[xCenter, bottom] = Interpolation.LinearInterpolation(Heightmap[left, bottom], Heightmap[right, bottom], 0.5f) + rnd() / Mathf.Pow(r, recStep+1);

        if (UseSteps) {
            _goOn = false;
            while (!_goOn) {
                yield return null;
            }
            UpdateTerrainHeightmap();
            _goOn = false;
        }
        
        if (right - left >= 2) {
            yield return DiamondSquare(left, top, xCenter, yCenter, rnd, r, recStep+1); // Top left
            yield return DiamondSquare(xCenter, top, right, yCenter, rnd, r, recStep+1); // Top right
            yield return DiamondSquare(left, yCenter, xCenter, bottom, rnd, r, recStep+1); // bottom left
            yield return DiamondSquare(xCenter, yCenter, right, bottom, rnd, r, recStep+1); // bottom left
            if (!UseSteps)
                UpdateTerrainHeightmap();
        }
    }

    private void UpdateTerrainHeightmap() {
        float[,] arr= new float[1,1];
        for (int x = 0; x < Heightmap.GetLength(0); x++) {
            for (int y = 0; y < Heightmap.GetLength(1); y++) {
                arr[0,0] = Heightmap[x, y];
                Terrain.terrainData.SetHeights(x, y, arr);
            }
        }
    }

    private float RndFunc() {
        return Random.value * 2 - 1;
    }
}

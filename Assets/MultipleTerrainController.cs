using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MultipleTerrainController : MonoBehaviour {

    private List<GameObject> _terrains = new List<GameObject>();

    [SerializeField]
    public GameObject Light;

    public GameObject Water;

    public int NoOfPatches;

    [SerializeField]
    private GameObject _prefab;

    private OwnTerrain _prefabTerr;

    [SerializeField]
    private Texture2D _latticeTex;

    [SerializeField] private int _latticeSize;

    [SerializeField]
    private int _height;

    [SerializeField]
    private string _matString = "ShaderUser";

    [SerializeField]
    private float _t;

    [SerializeField]
    private float _secScale;

    // Use this for initialization
    void Start () {

	    _prefabTerr = _prefab.GetComponent<OwnTerrain>();

	    int lattiSizePerPatch = _latticeSize/NoOfPatches;
        // Create patches
        for (int i = 0; i < NoOfPatches*NoOfPatches; i++) {
	        GameObject terrGo = Instantiate(_prefab);
	        OwnTerrain terr = terrGo.GetComponent<OwnTerrain>();
	        terr.IsControlled = true;
	        terr.Light = Light;
            terr.LatticeRangeX = new Vector2(i % NoOfPatches / (float)(NoOfPatches),
                                             i % NoOfPatches / (float)(NoOfPatches) + lattiSizePerPatch / (float)_latticeSize);
            terr.LatticeRangeY = new Vector2(i / NoOfPatches / (float)(NoOfPatches),
                                             i / NoOfPatches / (float)(NoOfPatches) + lattiSizePerPatch / (float)_latticeSize);
            _terrains.Add(terrGo);

	    }
        // top
        _terrains[6].transform.position = new Vector3(-_prefabTerr.Size, 0, _prefabTerr.Size);
        _terrains[7].transform.position = new Vector3(0, 0, _prefabTerr.Size);
        _terrains[8].transform.position = new Vector3(_prefabTerr.Size, 0, _prefabTerr.Size);
        //middle
        _terrains[3].transform.position = new Vector3(-_prefabTerr.Size, 0, 0);
        _terrains[4].transform.position = new Vector3(0, 0, 0);
        _terrains[5].transform.position = new Vector3(_prefabTerr.Size, 0, 0);
        //bottom
        _terrains[0].transform.position = new Vector3(-_prefabTerr.Size, 0, -_prefabTerr.Size);
        _terrains[1].transform.position = new Vector3(0, 0, -_prefabTerr.Size);
        _terrains[2].transform.position = new Vector3(_prefabTerr.Size, 0, -_prefabTerr.Size);
    }

    private void FillLatticeArray() {
        for (int i = 0; i < _latticeSize; i++) {
            for (int j = 0; j < _latticeSize; j++) {
                float val1 = Random.value,
                      val2 = Random.value;
                _latticeTex.SetPixel(i, j, new Color(val1, val2, 0));
            }    
        }
       _latticeTex.Apply();
    }

    // Update is called once per frame
	void Update () {
        _t = (_t+Time.deltaTime / _secScale) % 1.0f;
	    if (Input.GetKeyDown(KeyCode.R)) {
	        Water.transform.localScale = new Vector3(_prefabTerr.Size / 2 * NoOfPatches * 1.5f, 0, _prefabTerr.Size / 2 * NoOfPatches * 1.5f);
            Destroy(_latticeTex);
            _latticeTex = new Texture2D(_latticeSize, _latticeSize);
            _latticeTex.filterMode = FilterMode.Point;  
            FillLatticeArray();

	        foreach (GameObject terrain in _terrains) {
	            OwnTerrain terr = terrain.GetComponent<OwnTerrain>();
                terr.LatticeSize = _latticeSize;
                terr.LatticeTex = _latticeTex;
	            terr.MatString = _matString;
                terr.Build(_t);
	        }
	    }
	    foreach (GameObject terrain in _terrains) {
	        terrain.GetComponent<OwnTerrain>().UpdateTime(_t);
        }
	}
}

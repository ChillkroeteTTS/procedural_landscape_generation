using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleTerrainController : MonoBehaviour {

    private List<GameObject> _terrains = new List<GameObject>();

    [SerializeField]
    public GameObject Light;

    public int LatticeSizePerPatch;

    [SerializeField]
    private int LatticeSize;

    [SerializeField]
    private GameObject _prefab;

    private OwnTerrain _prefabTerr;

    [SerializeField]
    private Texture2D _latticeTex;

    // Use this for initialization
	void Start () {
        LatticeSize = (3 - 1) * LatticeSizePerPatch+1;
        _latticeTex = new Texture2D(LatticeSize, LatticeSize);
        _latticeTex.filterMode = FilterMode.Point;

	    _prefabTerr = _prefab.GetComponent<OwnTerrain>();
	    for (int i = 0; i < 9; i++) {
	        GameObject terrGo = Instantiate(_prefab);
	        OwnTerrain terr = terrGo.GetComponent<OwnTerrain>();
            terrGo.transform.position = new Vector3(-terr.Size/2, 0, -terr.Size/2);
	        terr.IsControlled = true;
	        terr.Light = Light;

            _terrains.Add(terrGo);

	    }
        // top
        _terrains[0].transform.position = new Vector3(-_prefabTerr.Size, 0, _prefabTerr.Size);
        _terrains[1].transform.position = new Vector3(0, 0, _prefabTerr.Size);
        _terrains[2].transform.position = new Vector3(_prefabTerr.Size, 0, _prefabTerr.Size);
        //middle
        _terrains[3].transform.position = new Vector3(-_prefabTerr.Size, 0, 0);
        _terrains[4].transform.position = new Vector3(0, 0, 0);
        _terrains[5].transform.position = new Vector3(_prefabTerr.Size, 0, 0);
        //bottom
        _terrains[6].transform.position = new Vector3(-_prefabTerr.Size, 0, -_prefabTerr.Size);
        _terrains[7].transform.position = new Vector3(0, 0, -_prefabTerr.Size);
        _terrains[8].transform.position = new Vector3(_prefabTerr.Size, 0, -_prefabTerr.Size);
    }

    private void FillLatticeArray() {
        for (int i = 0; i < LatticeSize; i++) {
            for (int j = 0; j < LatticeSize; j++) {
                float val1 = Random.value,
                      val2 = Random.value;
                _latticeTex.SetPixel(i, j, new Color(val1, val2, 0));
            }    
        }
       _latticeTex.Apply();
    }

    // Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.R)) {
            LatticeSize = (4 - 2) * LatticeSizePerPatch+2;
            FillLatticeArray();

	        for (int i = 0; i < _terrains.Count; i++) {
	            GameObject terrain = _terrains[i];

	            //set lattice map
	            Texture2D newLattice = new Texture2D(LatticeSizePerPatch, LatticeSizePerPatch);
	            newLattice.filterMode = FilterMode.Point;
	            for (int x = 0; x < LatticeSizePerPatch; x++) {
	                for (int z = 0; z < LatticeSizePerPatch; z++) {
	                    int arrPosX = i%3,
	                        arrPosY = i/3;
	                    newLattice.SetPixel(x, z, _latticeTex.GetPixel(arrPosX*(LatticeSizePerPatch - 2)+x,
	                                                                   arrPosY*(LatticeSizePerPatch - 2)+z));
	                }
	            }
                newLattice.Apply();
	            terrain.GetComponent<OwnTerrain>().LatticeTex = newLattice;
	            terrain.GetComponent<OwnTerrain>().Build();
	        }
	    }
	}
}

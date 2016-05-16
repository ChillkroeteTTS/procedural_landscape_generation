using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;

public class OwnTerrain : MonoBehaviour {

    public float Size;

    public int Resolution;

    public int LatticeSize = 3;

    public float[,] _heightmap;

    private Mesh _mesh;

    [SerializeField]
    private Vector3[] _vertices;

    [SerializeField]
    private Vector2[] _uv;

    [SerializeField]
    private int[] _triangles;

    [SerializeField]
    private Vector3[] _normals;

    private Material _terrainMat;

    private Texture2D _latticeTex;

    public float[,] Heightmap {
        get { return _heightmap; }
        set {
            _heightmap = value;
            Build();
        }
    }

    // Use this for initialization
    void Start () {
        _mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh = _mesh;
        gameObject.AddComponent<MeshRenderer>();

        _terrainMat = new Material(Shader.Find("Custom/OwnTerrain"));
        gameObject.GetComponent<MeshRenderer>().material = _terrainMat;
        _mesh.name = "MyTerrain";
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.R))
            Build();
	}


    private void Build() {
        //Set shader properties
        _terrainMat.SetFloat("_TerrainSize", Resolution);
        Destroy(_latticeTex);
        _latticeTex = new Texture2D(LatticeSize, LatticeSize);
        _latticeTex.filterMode = FilterMode.Point;
        FillLatticeTex();
        _terrainMat.SetTexture("_LatticeTex", _latticeTex);
        _terrainMat.SetColor("_Color", Color.gray);

        _heightmap = new float[Resolution,Resolution];
        _mesh.Clear();
        _vertices = new Vector3[Resolution*Resolution];
        _triangles = new int[(Resolution-1) * (Resolution-1) * 2 * 3];
        _uv = new Vector2[_vertices.Length];
        _normals = new Vector3[_vertices.Length];

        int cnt = 0;

        // Create Vertices
        for (int x = 0; x < _heightmap.GetLength(0); x++) {
            for (int z = 0; z < _heightmap.GetLength(1); z++) {
                _vertices[cnt++] = new Vector3(-Size/2f + Size/Resolution * x,
                                              _heightmap[x, z],
                                              - Size / 2f + Size / Resolution * z);
            }
        }


        //Create Triangles
        cnt = 0;
        for (int i = 0; i < (Resolution-1)*(Resolution-1); i++, cnt+=6) {
            //first triangle
            int resMinOne = Resolution - 1;
            _triangles[cnt]     = i % resMinOne + (i / resMinOne) * Resolution;
            _triangles[cnt + 1] = _triangles[cnt] + 1;
            _triangles[cnt + 2] = i % resMinOne + (i / resMinOne + 1) * Resolution;

            //second triangle
            _triangles[cnt + 3] = _triangles[cnt + 1];
            _triangles[cnt + 4] = _triangles[cnt + 2] + 1;
            _triangles[cnt + 5] = _triangles[cnt + 2];
        }

        //UV coordinates
        for (int i = 0; i < _uv.Length; i++) {
            _uv[i] = new Vector2(i % Resolution / (float)Resolution, (i / Resolution) / (float)Resolution);
        }

        // Normals
        for (int i = 0; i < _normals.Length; i++) {
            _normals[i] = Vector3.up;
        }

        _mesh.vertices = _vertices;
        //_mesh.vertices = new Vector3[] {new Vector3(-1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 0), };
        //_mesh.triangles = new int[] {0, 1, 2};
        _mesh.uv = _uv;
        _mesh.triangles = _triangles;
        _mesh.normals = _normals;
    }

    private void FillLatticeTex() {
        for (int x = 0; x < LatticeSize; x++) {
            for (int y = 0; y < LatticeSize; y++) {
                float val1 = Random.value,
                    val2 = Random.value;
                _latticeTex.SetPixel(x, y, new Color(val1, val2, 0));
            }
        }
        _latticeTex.Apply();
    }


    private void Reset() {
        _heightmap = new float[Resolution, Resolution];
    }
}

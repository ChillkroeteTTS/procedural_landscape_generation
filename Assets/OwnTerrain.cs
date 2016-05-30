using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;

public class OwnTerrain : MonoBehaviour {

    public float Size;

    public GameObject Light;

    public int Resolution;

    public int LatticeSize = 3;

    public float[,] _heightmap;

    public bool IsControlled = false;

    public string MatString = "ShaderUser";

    public Vector2 LatticeRangeX;
    public Vector2 LatticeRangeY;

    private Mesh _mesh;

    [SerializeField]
    private Vector3[] _vertices;

    [SerializeField]
    private Vector2[] _uv;

    [SerializeField] private Vector2[] _latticeUv;

    [SerializeField]
    private int[] _triangles;

    [SerializeField]
    private Vector3[] _normals;

    private Material _terrainMat;

    public Texture2D LatticeTex;

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
        _mesh.name = "MyTerrain";
    }
	
	// Update is called once per frame
	void Update () {
	    if (!IsControlled) {
	        if (Input.GetKeyDown(KeyCode.R))
	            Build();
	    }
	}


    public void Build(float time=0f) {
        Destroy(_terrainMat);
        _terrainMat = Instantiate(Resources.Load<Material>(MatString));
        gameObject.GetComponent<MeshRenderer>().material = _terrainMat;
        //Set shader properties
        _terrainMat.SetFloat("_TerrainSize", Resolution);
        if (!IsControlled) {
            Destroy(LatticeTex);
            LatticeTex = new Texture2D(LatticeSize, LatticeSize);
            FillLatticeTex();
        }
        LatticeTex.filterMode = FilterMode.Point;
        _terrainMat.SetTexture("_LatticeTex", LatticeTex);
        _terrainMat.SetFloat("_LatticeSize", LatticeSize);
        _terrainMat.SetVector("_LightPos", new Vector4(Light.transform.position.x, Light.transform.position.y, Light.transform.position.z, 0));
        _terrainMat.SetColor("_Color", Color.gray);
        _terrainMat.SetFloat("_Timer", time);

        _heightmap = new float[Resolution,Resolution];
        _mesh.Clear();
        _vertices = new Vector3[Resolution*Resolution];
        _triangles = new int[(Resolution-1) * (Resolution-1) * 2 * 3];
        _uv = new Vector2[_vertices.Length];
        _normals = new Vector3[_vertices.Length];

        int cnt = 0;

        // Create Vertices
        for (int z = 0; z < _heightmap.GetLength(0); z++) {
            for (int x = 0; x < _heightmap.GetLength(1); x++) {
                _vertices[cnt++] = new Vector3(-Size/2f + Size/(Resolution-1) * x,
                                              _heightmap[x, z],
                                              - Size / 2f + Size / (Resolution-1) * z);
            }
        }


        //Create Triangles
        cnt = 0;
        for (int i = 0; i < (Resolution-1)*(Resolution-1); i++, cnt+=6) {
            //first triangle
            int resMinOne = Resolution - 1;
            _triangles[cnt]     = i % resMinOne + (i / resMinOne) * Resolution;
            _triangles[cnt + 2] = _triangles[cnt] + 1;
            _triangles[cnt + 1] = i % resMinOne + (i / resMinOne + 1) * Resolution;

            //second triangle
            _triangles[cnt + 3] = _triangles[cnt + 1];
            _triangles[cnt + 4] = _triangles[cnt + 1] + 1;
            _triangles[cnt + 5] = _triangles[cnt + 2];
        }

        //UV coordinates
        for (int i = 0; i < _uv.Length; i++) {
            _uv[i] = new Vector2(i % Resolution / (float)(Resolution-1), (i / Resolution) / (float)(Resolution-1));
        }

        // Normals
        for (int i = 0; i < _normals.Length; i++) {
            _normals[i] = Vector3.up;
        }

        _mesh.vertices = _vertices;
        //_mesh.vertices = new Vector3[] {new Vector3(-1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 0), };
        //_mesh.triangles = new int[] {0, 1, 2};
        _mesh.uv = _uv;
        if (!IsControlled)
            _mesh.uv2 = _uv;
        else {
            // Determine specific lattice coordinates for this terrain and assign them
            _latticeUv = new Vector2[_vertices.Length];
            for (int i = 0; i < _latticeUv.Length; i++) {
                // Map uv coord for current pixel [0-1] to assigned range [LatticeRange[0] - LatticeRange[1]]
                _latticeUv[i] = new Vector2(LatticeRangeX[0] + (LatticeRangeX[1] - LatticeRangeX[0]) * _uv[i].x,
                                           LatticeRangeY[0] + (LatticeRangeY[1] - LatticeRangeY[0]) * _uv[i].y);
            }
            _mesh.uv2 = _latticeUv;
        }
        _mesh.triangles = _triangles;
        _mesh.normals = _normals;
    }


    public void UpdateTime(float time) {
        Material mat = gameObject.GetComponent<Renderer>().material;
        mat.SetFloat("_Timer", time);
        gameObject.GetComponent<Renderer>().material = mat;
    }

    private void FillLatticeTex() {
        for (int x = 0; x < LatticeSize; x++) {
            for (int y = 0; y < LatticeSize; y++) {
                float val1 = Random.value,
                    val2 = Random.value;
                LatticeTex.SetPixel(x, y, new Color(val1, val2, 0));
            }
        }
        LatticeTex.Apply();
    }


    private void Reset() {
        _heightmap = new float[Resolution, Resolution];
    }
}

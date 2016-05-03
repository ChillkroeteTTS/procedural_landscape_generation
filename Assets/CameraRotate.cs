using System;
using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour {

    [SerializeField]
    private float _radPos;

    [SerializeField]
    private float _radSpeedPS = 2f*Mathf.PI*0.02f;

    [SerializeField]
    private float _height = 30f;

    [SerializeField] private float _rad = 200f;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    _radPos = (_radPos + _radSpeedPS*Time.deltaTime) % (2f*Mathf.PI);
        gameObject.transform.position = new Vector3(Mathf.Cos(_radPos) *_rad, _height, Mathf.Sin(_radPos) * _rad);
        gameObject.transform.LookAt(Vector3.zero);
	}
}

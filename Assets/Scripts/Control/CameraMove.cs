using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    //相机偏移量
    public Vector3 offset;
    //跟随的目标
    GameObject target;
    
	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = target.transform.position + offset;		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    private GameObject target;
    public Vector3 offset;
    public float followSmoothing=4;
    void Start()
    {
        target = RaidManager.Instance.Player;
    }

    // Update is called once per frame
    void FixedUpdate() {
        //transform.position = target.transform.position + offset;
       transform.position= Vector3.Lerp(transform.position, target.transform.position + offset, Time.deltaTime * followSmoothing);
	}
   
}

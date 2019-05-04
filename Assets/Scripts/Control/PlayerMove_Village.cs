using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove_Village : MonoBehaviour {
    //速度控制
    public float speed = 3f;
   
    // Update is called once per frame
    void Update () {
        //charater move control 
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        Vector3 currentVelocity = GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = new Vector3(-h, currentVelocity.y, -v) * speed;
        
        //character rotation control
        if (Mathf.Abs(v) > 0.141f || Mathf.Abs(h) > 0.141f)
        {
            Vector3 lookat = new Vector3(-h, 0, -v);
             transform.rotation= Quaternion.LookRotation(lookat);
        }

    }
}

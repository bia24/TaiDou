using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleSelf : MonoBehaviour {
    public float disappearTime = 1f;
    private float timer;

    // Use this for initialization
    void Start () {
        timer = disappearTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (gameObject.activeInHierarchy)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                gameObject.SetActive(false);
                timer = disappearTime;
            }
        }
	}
}

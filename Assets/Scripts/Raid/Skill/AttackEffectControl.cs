using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectControl : MonoBehaviour {
    private Animation anim; //攻击弧线变大特效
    private ParticleSystem[] ps; //攻击粒子效果
    private  MeshRenderer r;//攻击弧线渲染；
    public float exitsTime = 0.3f; //攻击弧线存在时间
    private float timer;
    private bool isExist;
    // Use this for initialization
    void Start () {
        anim = transform.GetComponentInChildren<Animation>();
        ps = transform.GetComponentsInChildren<ParticleSystem>();
        r = transform.GetComponentInChildren<MeshRenderer>();
        //r.enabled = false;
        r.material.SetColor("_TintColor", new Color(0, 0, 0, 0));
        isExist = false;
        timer = exitsTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (isExist)
        {
            timer -= Time.deltaTime;
            r.material.SetColor("_TintColor", Color.Lerp(r.material.GetColor("_TintColor"), Color.red, Time.deltaTime));
            if (timer <= 0)
            {
                isExist = false;
                r.material.SetColor("_TintColor", new Color(0, 0, 0, 0));
            }
        }
	}

    public void Show()
    {
        timer = exitsTime;
        isExist = true;
        anim.Play();
        foreach (var t in ps)
        {
            t.Play();
        }
    }
}

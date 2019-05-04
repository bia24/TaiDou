using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour {
    //获得动画播放器
    private Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        float v = GetComponent<Rigidbody>().velocity.magnitude;
        if (v > 0.2f) //非自动控制
        {
            anim.SetBool("isMove",true);
        }
        else
        {
            if (GameControl.Instance.playerAutoMove.agent.isStopped)//如果当前不在自动寻路
            {
                anim.SetBool("isMove", false);
            }
        }
	}

    #region 逻辑方法
    public void SetAnimation_IdleToRun()
    {
        anim.SetBool("isMove", true);
    }
    public void SetAnimation_RunToIdle()
    {
        anim.SetBool("isMove", false);
    }
    #endregion

}

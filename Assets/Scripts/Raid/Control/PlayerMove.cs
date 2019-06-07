using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    public float velocity = 5f;
    private PlayerAnimation playerAnimation;
    [HideInInspector]
    public bool isLocalPlayer = false;
    private bool lastState = false;
    void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();  
    }

    void Update () {
        if (GetComponent<PlayerAttackManager>().hp <= 0||RaidManager.Instance.isEnd||!isLocalPlayer) return;
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        Vector3 nowVel = GetComponent<Rigidbody>().velocity;
        if (Mathf.Abs(v) > 0.05f || Mathf.Abs(h) > 0.05f)
        {
            if (playerAnimation.GetAnimatorState(1, "Empty")&&!playerAnimation.GetAnimatorState(0,"Hit"))
            {
                GetComponent<Rigidbody>().velocity = new Vector3(h * velocity, nowVel.y, v * velocity);
                playerAnimation.SetBool("Move", true);
                transform.LookAt(new Vector3(h, 0, v) + transform.position);
            }
            else
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0, nowVel.y, 0);
            }
        }
        else
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, nowVel.y, 0);
            playerAnimation.SetBool("Move", false);
        }
        bool currentState = playerAnimation.GetParamBool("Move");
        if (currentState != lastState)
        {
            RaidManager.Instance.SendLocalAnimationToServer("Move", TransationType.Bool, currentState);//同步本动画操作
            lastState = currentState;
        }
    }
}

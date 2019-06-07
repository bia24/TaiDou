using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    private Animator anim;
    

    private void Start()
    {
        anim = GetComponent<Animator>();
    }





    #region 逻辑方法
    public void SetBool(string name,bool state)
    {
        if (anim == null) return;
        anim.SetBool(name, state);
    }
    public void SetTrigger(string name)
    {
        if (anim == null) return;
        anim.SetTrigger(name);
    }
    public bool GetAnimatorState(int index,string name)
    {
        return anim.GetCurrentAnimatorStateInfo(index).IsName(name);
    }
    public bool GetParamBool(string name)
    {
        return anim.GetBool(name);
    }
   


    public void OnSkillBtnClick(PosType pos)
    {
        if (GetComponent<PlayerAttackManager>().hp <= 0) return;
        switch (pos)
        {
            case PosType.Basic:
                SetTrigger("AttackTrigger");
                RaidManager.Instance.SendLocalAnimationToServer("AttackTrigger", TransationType.Trigger, true);//同步本动画操作
                break;
            case PosType.One:
                SetTrigger("Skill1");
                RaidManager.Instance.SendLocalAnimationToServer("Skill1", TransationType.Trigger, true);//同步本动画操作
                break;
            case PosType.Two:
                SetTrigger("Skill2");
                RaidManager.Instance.SendLocalAnimationToServer("Skill2", TransationType.Trigger, true);//同步本动画操作
                break;
            case PosType.Three:
                SetTrigger("Skill3");
                RaidManager.Instance.SendLocalAnimationToServer("Skill3", TransationType.Trigger, true);//同步本动画操作
                break;


        }
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBtn : MonoBehaviour {

    public PosType posType=PosType.Basic;
    private PlayerAnimation playerAnimation;
    private UISprite mask;
    public float coldTime=4f;
    private float coldTimer=0;
    private bool isColding=false;

    private void Start()
    {
        playerAnimation = RaidManager.Instance.Player.GetComponent<PlayerAnimation>();
        mask = transform.Find(name + "_Mask").GetComponent<UISprite>();
        GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
        {
            if (RaidManager.Instance.isEnd) return;
            if (!isColding)
            {
                playerAnimation.OnSkillBtnClick(posType);
            }
            isColding = true;
        }));
        mask.fillAmount = 0;
        coldTimer = coldTime;
    }
    void  Update()
    {
        if (RaidManager.Instance.Player.GetComponent<PlayerAttackManager>().hp <= 0|| RaidManager.Instance.isEnd)
        {
            isColding = false;
            mask.fillAmount = 0;
            return;
        }
        if (isColding)
        {
            mask.fillAmount = coldTimer / coldTime;
            coldTimer -= Time.deltaTime;
            if (coldTimer <= 0.05f)
            {
                mask.fillAmount = 0;
                isColding = false;
                coldTimer = coldTime;
            }
        }
    }
}

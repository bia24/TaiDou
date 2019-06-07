using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFrame : MonoBehaviour {

    private PlayerInfo playerInfo;
    private UISprite headSpr;
    private UILabel levelLab;
    private UILabel nameLab;
    private UISlider hpBar;
    private UILabel hpLab;
    private UISlider physicalBar;
    private UILabel physicalLab;
    private RoleHandler roleHandler;
    

    private void Start()
    {
        //获取引用
        playerInfo = GameControl.Instance.playerInfo;
        headSpr = transform.Find("Spr_Head").GetComponent<UISprite>();
        levelLab = transform.Find("Lab_Level").GetComponent<UILabel>();
        nameLab = transform.Find("Lab_Name").GetComponent<UILabel>();
        hpBar = transform.Find("Spr_HP").GetComponent<UISlider>();
        hpLab = hpBar.transform.Find("Lab_Hp").GetComponent<UILabel>();
        physicalBar = transform.Find("Spr_Physical").GetComponent<UISlider>();
        physicalLab=physicalBar.transform.Find("Lab_Physical").GetComponent<UILabel>();
        roleHandler = GameObject.Find("UI Root").GetComponent<RoleHandler>();
        //初始化
        headSpr.spriteName = playerInfo.CharacterType == CharacterType.Man ? "Head_Man" : "Head_Woman";
        levelLab.text = playerInfo.CharacterLevel.ToString();
        nameLab.text = playerInfo.CharacterName.ToString();
        hpBar.value = (float)RaidManager.Instance.Player.GetComponent<PlayerAttackManager>().hp
            / RaidManager.Instance.Player.GetComponent<PlayerAttackManager>().hpSet;
        hpLab.text= RaidManager.Instance.Player.GetComponent<PlayerAttackManager>().hp+"/"
            + RaidManager.Instance.Player.GetComponent<PlayerAttackManager>().hpSet;
        physicalBar.value= (float)playerInfo.Physical / 100;
        physicalLab.text = playerInfo.Physical + "/100";
        //事件绑定
        playerInfo.OnPhysicalChange += OnPhysicalChange;
        RaidManager.Instance.Player.GetComponent<PlayerAttackManager>().OnHpChangeEvent += OnHpChange;
    }

    private void OnPhysicalChange(int v)
    {
        //ui改变
        physicalBar.value = (float) v / 100;
        physicalLab.text =v + "/100";
        //服务器同步
        roleHandler.UpdateRole(playerInfo);
    }
    private void OnHpChange(int v)
    {
        //ui改变
        hpBar.value = (float) v / RaidManager.Instance.Player.GetComponent<PlayerAttackManager>().hpSet;
        hpLab.text = v + "/" + RaidManager.Instance.Player.GetComponent<PlayerAttackManager>().hpSet;
    }

    private void OnDestroy()
    {
        //事件取消绑定
        if (playerInfo != null)
        {
            playerInfo.OnPhysicalChange -= OnPhysicalChange;
        }
        //hp事件，因为RaidManager.Instance.Player场景结束会自动销毁，所以不用主动取消了
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

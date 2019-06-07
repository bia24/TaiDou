using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RaidBtn : MonoBehaviour {
    public int id;
    public int needLevel;
    public string sceneName;
    public int needPhysics;
    private UIManagerBase ui;//获取gameControl里的uimanager引用
    private void Awake()
    {
        GetComponent<UIButton>().onClick.Add(new EventDelegate(OnBtnClick));
    }

    private void Start()
    {
        ui = GameControl.Instance.mUIManager;
        //向uimanager中注册自己
        ((UIManager_02NoviceVillage)ui).raids.Add(id,this);
    }
    public void OnBtnClick()
    {
        
        UIBehavior info = ui.GetUI("Map_Raid_Info_u");
        //info panel 显示出来
        info.ShowUI();
        info.transform.DOScale(1, 0.2f).From(0);
        //更新panel上的信息
        ui.GetUI("Raid_Des_u").GetComponent<UILabel>().text = "这里是名为 " + id + " 的地下城副本\n" + "你需要 " + needLevel + " 级才能进入";
        ui.GetUI("Raid_Physics_u").GetComponent<UILabel>().text = needPhysics.ToString();
        //进入 按钮显示
        if (GameControl.Instance.playerInfo.CharacterLevel < needLevel)
        {
            ui.GetUI("Raid_EnterPerson_Btn_u").DisableUIButton();
            ui.GetUI("Raid_EnterTeam_Btn_u").DisableUIButton();
        }
        else if(GameControl.Instance.playerInfo.Physical < needPhysics)
        {
            ui.GetUI("Raid_EnterPerson_Btn_u").DisableUIButton();
            ui.GetUI("Raid_EnterTeam_Btn_u").DisableUIButton();
        }
        else
        {
            ui.GetUI("Raid_EnterPerson_Btn_u").EnableUIButton();
            ui.GetUI("Raid_EnterTeam_Btn_u").EnableUIButton();
        }
        //给uimanager传递当前选择项
       ((UIManager_02NoviceVillage)GameControl.Instance.mUIManager).currentRaid = this;
        //禁用map上的所有按钮
        ui.SetChildBtnState(ui.GetUI("Map_u"), false);
    }
	
}

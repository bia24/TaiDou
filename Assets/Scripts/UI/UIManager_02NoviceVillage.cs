using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager_02NoviceVillage : UIManagerBase {
    public UIAtlas UIAtlas;
    private PlayerInfo playerInfo;
    private int currentKnapsackIndex;
    private ArticleType currentDressUpType;
    private int currentKnapsackCount;
    private int currentTaskIndex; 
        
   
       

    private void Start()
    {
        //初始化各UI控件
        InitUIValue();
        //给各个UI控件绑定对应事件--playerInfo自定义事件
        BindEventOfAllUI();
        //绑定NGUI自带的事件   
        BindNativeEventOfAllUI();
        //状态初始化，关闭不需要显示的控件
        InitHidePanel();


    }


    #region 初始化ui控件的值
    //初始化ui控件的值
    private void InitUIValue()
    {
        playerInfo = GameControl.Instance.playerInfo;
        //头像框部分
        GetUI("Lab_Name_u").GetComponent<UILabel>().text = playerInfo.CharacterName;
        GetUI("Lab_Level_u").GetComponent<UILabel>().text = playerInfo.CharacterLevel.ToString();
        GetUI("Lab_Physical_u").GetComponent<UILabel>().text = playerInfo.Physical.ToString() + "/100";
        GetUI("Spr_Physical_u").GetComponent<UISlider>().value = (float)playerInfo.Physical / 100;
        GetUI("Lab_Energy_u").GetComponent<UILabel>().text = playerInfo.Energy.ToString() + "/50";
        GetUI("Spr_Energy_u").GetComponent<UISlider>().value = (float)playerInfo.Energy / 50;
        GetUI("Spr_Head_u").GetComponent<UISprite>().atlas = UIAtlas;
        GetUI("Spr_Head_u").GetComponent<UISprite>().spriteName = "Head_" + playerInfo.CharacterType.ToString();
        //右上角金币钻石相关
        GetUI("Lab_Diamond_u").GetComponent<UILabel>().text = playerInfo.Diamond.ToString();
        GetUI("Lab_Gold_u").GetComponent<UILabel>().text = playerInfo.Gold.ToString();
        //"个人状态"面板
        GetUI("Spr_Status_Head_u").GetComponent<UISprite>().atlas = UIAtlas;
        GetUI("Spr_Status_Head_u").GetComponent<UISprite>().spriteName = "Head_" + playerInfo.CharacterType.ToString();
        GetUI("Lab_Status_Name_u").GetComponent<UILabel>().text = playerInfo.CharacterName;
        GetUI("Lab_Status_Level_u").GetComponent<UILabel>().text = playerInfo.CharacterLevel.ToString();
        GetUI("Lab_Status_Power_u").GetComponent<UILabel>().text = playerInfo.CharacterPower.ToString();
        GetUI("Lab_Status_Exp_u").GetComponent<UILabel>().text = playerInfo.CharacterExp.ToString() + "/" + playerInfo.LevelUpNeedExp().ToString();
        GetUI("Spr_Status_Exp_u").GetComponent<UISlider>().value = (float)playerInfo.CharacterExp / playerInfo.LevelUpNeedExp();
        GetUI("Lab_Status_Diamond_u").GetComponent<UILabel>().text = playerInfo.Diamond.ToString();
        GetUI("Lab_Status_Gold_u").GetComponent<UILabel>().text = playerInfo.Gold.ToString();
        GetUI("Lab_Status_Physical_u").GetComponent<UILabel>().text = playerInfo.Physical.ToString() + "/100";
        GetUI("Lab_Status_Energyl_u").GetComponent<UILabel>().text = playerInfo.Energy.ToString() + "/50";
        //背包 面板部分
        InitShowKnapsackItem();
        //装备穿戴面板
        InitShowDressedUpItem();
        //任务面板部分
        InitShowTask();
    }

    #endregion

    #region 绑定各个ui控件-playerInfo自定义事件
    //绑定各个ui控件
    private void BindEventOfAllUI()
    {
        /******角色类型更换******/
        playerInfo.OnTypeChange += (value) =>
        {
            //头像框部分
            GetUI("Spr_Head_u").GetComponent<UISprite>().atlas = UIAtlas;
            GetUI("Spr_Head_u").GetComponent<UISprite>().spriteName = "Head_" + value.ToString();
            //"个人状态"面板
            GetUI("Spr_Status_Head_u").GetComponent<UISprite>().atlas = UIAtlas;
            GetUI("Spr_Status_Head_u").GetComponent<UISprite>().spriteName = "Head_" + value.ToString();
        };

        /******等级变化******/
        playerInfo.OnLevelChange += (value) =>
        {
            //头像框部分
            GetUI("Lab_Level_u").GetComponent<UILabel>().text = value.ToString();
            //"个人状态"面板
            GetUI("Lab_Status_Level_u").GetComponent<UILabel>().text = value.ToString();
        };

        /******昵称更改******/
        playerInfo.OnNameChange += (value) =>
        {
            //头像框部分
            GetUI("Lab_Name_u").GetComponent<UILabel>().text = value;
            //"个人状态"面板
            GetUI("Lab_Status_Name_u").GetComponent<UILabel>().text = value;
        };

        /******体力变化******/
        playerInfo.OnPhysicalChange += (value) =>
        {
            //头像框部分
            GetUI("Lab_Physical_u").GetComponent<UILabel>().text = value.ToString() + "/100";
            GetUI("Spr_Physical_u").GetComponent<UISlider>().value = (float)value / 100;
            //"个人状态"面板
            GetUI("Lab_Status_Physical_u").GetComponent<UILabel>().text = value.ToString() + "/100";
        };

        /******精力变化******/
        playerInfo.OnEnergyChange += (value) =>
        {
            //头像框部分
            GetUI("Lab_Energy_u").GetComponent<UILabel>().text = value.ToString() + "/50";
            GetUI("Spr_Energy_u").GetComponent<UISlider>().value = (float)value / 50;
            //"个人状态"面板
            GetUI("Lab_Status_Energyl_u").GetComponent<UILabel>().text = value.ToString() + "/50";
        };

        /******经验变化******/
        playerInfo.OnExpChange += (value) =>
        {
            //"个人状态"面板
            GetUI("Lab_Status_Exp_u").GetComponent<UILabel>().text = value.ToString() + "/" + playerInfo.LevelUpNeedExp().ToString();
            GetUI("Spr_Status_Exp_u").GetComponent<UISlider>().value = (float)value / playerInfo.LevelUpNeedExp();
            //"角色装备穿戴面板"
            GetUI("Lab_EXP_u").GetComponent<UILabel>().text = value + " /" + playerInfo.LevelUpNeedExp();
            GetUI("Spr_EXP_BG_u").GetComponent<UISlider>().value = (float)value / playerInfo.LevelUpNeedExp();
        };

        /******战斗力变化******/
        playerInfo.OnPowerChange += (value) =>
        {//EventDelegate
            //"个人状态"面板
            GetUI("Lab_Status_Power_u").GetComponent<UILabel>().text = value.ToString();
        };

        /******金币变化******/
        playerInfo.OnGoldChange += (value) =>
        {
            //"个人状态"面板
            GetUI("Lab_Status_Gold_u").GetComponent<UILabel>().text = value.ToString();
            //右上角面板
            GetUI("Lab_Gold_u").GetComponent<UILabel>().text = value.ToString();
        };

        /******钻石变化******/
        playerInfo.OnDiamondChange += (value) =>
        {
            //"个人状态"面板
            GetUI("Lab_Status_Diamond_u").GetComponent<UILabel>().text = value.ToString();
            //右上角面板
            GetUI("Lab_Diamond_u").GetComponent<UILabel>().text = value.ToString();
        };

        /******体力计时器变化******/
        playerInfo.OnTimerPhysicalChange += (value) =>
        {
            int remainTime = Mathf.CeilToInt(value);
            int hours = remainTime / 3600;
            int minutes = remainTime / 60;
            int seconds = remainTime % 60;
            string showTime = (hours >= 10 ? hours.ToString() : "0" + hours.ToString())
            + ":" + (minutes >= 10 ? minutes.ToString() : "0" + minutes.ToString())
            + ":" + (seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString());
            if (playerInfo.Physical == 100)
            {
                showTime = "00:00:00";
            }
            GetUI("Lab_Status_PhysicalRecover_u").GetComponent<UILabel>().text = showTime;
            int remainAllTime = (100 - playerInfo.Physical - 1) * PlayerInfo.TIMERRECOVER + remainTime;
            hours = remainAllTime / 3600;
            minutes = remainAllTime / 60;
            seconds = remainAllTime % 60;
            showTime = (hours >= 10 ? hours.ToString() : "0" + hours.ToString())
           + ":" + (minutes >= 10 ? minutes.ToString() : "0" + minutes.ToString())
           + ":" + (seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString());
            if (playerInfo.Physical == 100)
            {
                showTime = "00:00:00";
            }
            GetUI("Lab_Status_PhysicalAllRecover_u").GetComponent<UILabel>().text = showTime;
        };

        /******精力计时器变化******/
        playerInfo.OnTimerEnergyChange += (value) =>
        {
            int remainTime = Mathf.CeilToInt(value);
            int hours = remainTime / 3600;
            int minutes = remainTime / 60;
            int seconds = remainTime % 60;
            string showTime = (hours >= 10 ? hours.ToString() : "0" + hours.ToString())
            + ":" + (minutes >= 10 ? minutes.ToString() : "0" + minutes.ToString())
            + ":" + (seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString());
            if (playerInfo.Energy == 50)
            {
                showTime = "00:00:00";
            }
            GetUI("Lab_Status_EnergyRecover_u").GetComponent<UILabel>().text = showTime;
            int remainAllTime = (50 - playerInfo.Energy - 1) * PlayerInfo.TIMERRECOVER + remainTime;
            hours = remainAllTime / 3600;
            minutes = remainAllTime / 60;
            seconds = remainAllTime % 60;
            showTime = (hours >= 10 ? hours.ToString() : "0" + hours.ToString())
           + ":" + (minutes >= 10 ? minutes.ToString() : "0" + minutes.ToString())
           + ":" + (seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString());
            if (playerInfo.Energy == 50)
            {
                showTime = "00:00:00";
            }
            GetUI("Lab_Status_EnergyAllRecover_u").GetComponent<UILabel>().text = showTime;
        };
    }
    #endregion

    #region 绑定原生按钮事件
    private void BindNativeEventOfAllUI()
    {
        /*头像框部分*/
        GetUI("Btn_HeadFrame_u").RegisterOnClick(new EventDelegate(OnHeadFrameClick));
        /*右上角部分*/
        /*右下角部分*/
        GetUI("Btn_Knapsack_u").RegisterOnClick(new EventDelegate(OnKnapsackClick));
        GetUI("Btn_Task_u").RegisterOnClick(new EventDelegate(OnTaskClick));
        /*角色信息栏部分*/
        GetUI("Btn_Status_Cancle_u").RegisterOnClick(new EventDelegate(OnPlayerStatusCancleClick));
        GetUI("Btn_Status_ChangeName_u").RegisterOnClick(new EventDelegate(OnPlayerStatusChangeNameClick));
        /*改名面板部分*/
        GetUI("Btn_ChangeName_Cancle_u").RegisterOnClick(new EventDelegate(OnChangeNameCancleClick));
        GetUI("Btn_ChangeName_Confirm_u").RegisterOnClick(new EventDelegate(OnChangeNameConfirmClick));
        /*背包item点击事件*/
        RegisterKnapsackItemClickEvent();
        GetUI("Knapsack_Cancel_u").RegisterOnClick(new EventDelegate(OnKnapsackCancelClick));
        GetUI("Btn_Knapsack_Sold_u").RegisterOnClick(new EventDelegate(OnKnapsackSoldClick));
        GetUI("Btn_Knapsack_Z_u").RegisterOnClick(new EventDelegate(OnKnapsackClearClick));
        /*角色装备item点击事件*/
        RegisterRoleItemClickEvent();
        GetUI("Spr_Role_cancle_u").RegisterOnClick(new EventDelegate(() => { GetUI("Role_u").HideUI(); }));
        /*左边装备详情按钮点击事件*/
        GetUI("Left_Cancel_u").RegisterOnClick(new EventDelegate(OnLeftEQCancleClick));
        GetUI("Btn_EQ_DressUp_u").RegisterOnClick(new EventDelegate(OnLeftEQDressUpClick));
        GetUI("Btn_EQ_LevelUp_u").RegisterOnClick(new EventDelegate(OnLeftEQLevelUpClick));
        /*右边装备详情按钮点击事件*/
        GetUI("Right_Cancel_u").RegisterOnClick(new EventDelegate(() => { GetUI("Right_EQ_Info_u").HideUI(); GetUI("Knapsack_u").ShowUI(); }));
        GetUI("Btn_EQ_DressDown_u").RegisterOnClick(new EventDelegate(OnRightEQDreesDownClick));
        GetUI("Btn_EQ_LevelUp_Right_u").RegisterOnClick(new EventDelegate(OnRightEQLevelUpClick));
        /*药品弹窗*/
        GetUI("Tool_Cancel_u").RegisterOnClick(new EventDelegate(() => { GetUI("Pan_Tool_u").HideUI(); }));
        GetUI("Btn_Tool_u").RegisterOnClick(new EventDelegate(OnUseClick));
        GetUI("Btn_Tool_All_u").RegisterOnClick(new EventDelegate(OnUseAllClick));
        /*任务栏部分*/
        GetUI("Task_Cancle_u").RegisterOnClick(new EventDelegate(OnTaskCancleClick));
        RegisterTaskBtnClickEvent();
        /*领取奖赏部分*/
        GetUI("Btn_Get_Reward_u").RegisterOnClick(new EventDelegate(OnRewardGetClick));
        /*任务对话框*/
        GetUI("TaskTalk_Btn_u").RegisterOnClick(new EventDelegate(OnTaskTalkBtnClick));
    }
    #endregion

    #region NativeEvent 回调函数
    /*头像框部分*/
    private void OnHeadFrameClick()
    {
        UIBehavior start = GetUI("PlayStatus_u");
        start.ShowUI();
        start.transform.DOScale(1, 0.2f).From(0);
        //子panel隐藏
        GetUI("Panel_ChangeName_u").HideUI();
    }
    /*右下角部分*/
    private void OnKnapsackClick()
    {
        SetBaseUIActive(false);
        GetUI("Role_u").ShowUI();
        GetUI("Knapsack_u").ShowUI();
    }
    private void OnTaskClick()
    {
        SetBaseUIActive(false);
        GetUI("TaskPanel_u").ShowUI();
        InitShowTask();
    }
    /*角色信息栏部分*/
    private void OnPlayerStatusCancleClick()
    {
        UIBehavior start = GetUI("PlayStatus_u");
        start.transform.DOScale(0, 0.2f).From(1);
        StartCoroutine(HideObjectDelay(start, 0.2f));
        //子panel直接关闭
        GetUI("Panel_ChangeName_u").HideUI();
    }
    private void OnPlayerStatusChangeNameClick()
    {
        UIBehavior start = GetUI("Panel_ChangeName_u");
        start.ShowUI();
        start.transform.DOScale(1, 0.2f).From(0);
    }
    /*改名面板部分*/
    private void OnChangeNameCancleClick()
    {
        UIBehavior start = GetUI("Panel_ChangeName_u");
        start.transform.DOScale(0, 0.2f).From(1);
        StartCoroutine(HideObjectDelay(start, 0.2f));
    }
    private void OnChangeNameConfirmClick()
    {
        UIBehavior input = GetUI("Input_ChangeName_u");
        string newName = input.GetComponent<UIInput>().value;
        //todo:名字校验
        if (newName == "")
        {
            return;
        }
        else
        {
            playerInfo.CharacterName = newName;
        }
        UIBehavior start = GetUI("Panel_ChangeName_u");
        start.transform.DOScale(0, 0.2f).From(1);
        HideObjectDelay(start, 0.2f);

    }
    /*背包部分*/
    private void OnKnapsackItemClick(string num, int index)
    {
        if (index >= playerInfo.knapsack.Count)
            return;
        string itemName = "Knapsack_Spr_" + num + "_u";
        if (GetUI(itemName).GetComponent<UISprite>().spriteName == "")
            return;
       ArticleInfo a = playerInfo.knapsack[index].ItemInfo;
        
        ArticleType type = a.Type;
        GetUI("Lab_Sold_Price_u").GetComponent<UILabel>().text = a.Price == 0 ? "" : a.Price.ToString();
        currentKnapsackIndex = index; //记录当前所指定的道具
        if (type == ArticleType.Box || type == ArticleType.Drug)
        {//道具部分
            GetUI("Pan_Tool_u").ShowUI();
            GetUI("Left_EQ_Info_u").HideUI();
            GetUI("Role_u").ShowUI();

            GetUI("Lab_Tool_Name_u").GetComponent<UILabel>().text = a.Name;
            GetUI("Spr_Tool_u").GetComponent<UISprite>().spriteName = a.Icon;
            GetUI("Des_Tool_u").GetComponent<UILabel>().text = a.Des;
        }
        else
        {
            GetUI("Left_EQ_Info_u").ShowUI();
            GetUI("Pan_Tool_u").HideUI();
            GetUI("Role_u").HideUI();

            GetUI("Spr_EQ_Info_u").GetComponent<UISprite>().spriteName = a.Icon;
            GetUI("Lab_EQ_Name_u").GetComponent<UILabel>().text = a.Name;
            GetUI("Lab_EQ_Quality_n_u").GetComponent<UILabel>().text = a.Quality.ToString();
            GetUI("Lab_EQ_Damage_n_u").GetComponent<UILabel>().text = a.Damage.ToString();
            GetUI("Lab_EQ_HP_n_u").GetComponent<UILabel>().text = playerInfo.knapsack[index].HPPlus.ToString();
            GetUI("Lab_EQ_Power_u").GetComponent<UILabel>().text = playerInfo.knapsack[index].PowerPlus.ToString();
            GetUI("Label_EQ_Des_u").GetComponent<UILabel>().text = a.Des;

        }
        

    }
    private void OnKnapsackCancelClick()
    {
        GetUI("Knapsack_u").HideUI();
        GetUI("Role_u").HideUI();
        GetUI("Left_EQ_Info_u").HideUI();
        GetUI("Right_EQ_Info_u").HideUI();
        GetUI("Pan_Tool_u").HideUI();
        SetBaseUIActive(true);
    }
    private void OnKnapsackSoldClick()
    {
        if (!GetUI("Left_EQ_Info_u").gameObject.activeInHierarchy)
            return;
        string sprName = "Knapsack_Spr_";
        if (currentKnapsackIndex + 1 < 10) { sprName = sprName + "0" + (currentKnapsackIndex + 1).ToString() + "_u"; }
        else { sprName = sprName + (currentKnapsackIndex + 1).ToString() + "_u"; }
        string labName = sprName.Replace("Spr", "Lab");
        GetUI(sprName).GetComponent<UISprite>().spriteName = "";
        GetUI(labName).GetComponent<UILabel>().text = "";
        playerInfo.Gold += playerInfo.knapsack[currentKnapsackIndex].ItemInfo.Price;
        //隐藏
        GetUI("Left_EQ_Info_u").HideUI();
        //显示
        GetUI("Role_u").ShowUI();
        //更改
        GetUI("Lab_Sold_Price_u").GetComponent<UILabel>().text="";
        currentKnapsackCount--;//背包当前数目减少1；
        GetUI("Lab_Knapsack_num_u").GetComponent<UILabel>().text = currentKnapsackCount + "/24";
    }
    private void OnKnapsackClearClick()
    {
        for (int i = 0, j=0; i < playerInfo.knapsack.Count;j++)
        {
            string sprName = "Knapsack_Spr_";
            if (j + 1 < 10) { sprName = sprName + "0" + (j + 1).ToString() + "_u"; }
            else { sprName = sprName + (j + 1).ToString() + "_u"; }
            if (GetUI(sprName).GetComponent<UISprite>().spriteName == "")
            {
                playerInfo.knapsack.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        InitShowKnapsackItem();
    }

    /*角色装备部分*/
    private void OnRoleItemClick(ArticleType type)
    {
        if (playerInfo.dressedEq[type].ItemInfo.Icon == "")
            return;
        //隐藏和显示
        GetUI("Pan_Tool_u").HideUI();
        GetUI("Knapsack_u").HideUI();
        GetUI("Right_EQ_Info_u").ShowUI();

        //数值部分
        GetUI("Spr_EQ_Info_Right_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[type].ItemInfo.Icon;
        GetUI("Lab_EQ_Name_Right_u").GetComponent<UILabel>().text = playerInfo.dressedEq[type].ItemInfo.Name;
        GetUI("Lab_EQ_Quality_n_Right_u").GetComponent<UILabel>().text = playerInfo.dressedEq[type].ItemInfo.Quality.ToString();
        GetUI("Lab_EQ_Damage_n_Right_u").GetComponent<UILabel>().text = playerInfo.dressedEq[type].ItemInfo.Damage.ToString() ;
        GetUI("Lab_EQ_HP_n_Right_u").GetComponent<UILabel>().text = playerInfo.dressedEq[type].ItemInfo.HPPlus.ToString();
        GetUI("Lab_EQ_Power_Right_u").GetComponent<UILabel>().text = playerInfo.dressedEq[type].PowerPlus.ToString();
        GetUI("Label_EQ_Des_Right_u").GetComponent<UILabel>().text = playerInfo.dressedEq[type].ItemInfo.Des;

        currentDressUpType = type;//设置当前点击的类型
        
    }

    /*左边装备详情按钮部分*/
    private void OnLeftEQCancleClick()
    {
        GetUI("Left_EQ_Info_u").HideUI();
        GetUI("Role_u").ShowUI();
    }
    private void OnLeftEQDressUpClick()
    {
        //exchange
        KnapsackItem temp;
        temp= playerInfo.knapsack[currentKnapsackIndex];
        playerInfo.knapsack[currentKnapsackIndex]= playerInfo.dressedEq[temp.ItemInfo.Type];
        playerInfo.dressedEq[temp.ItemInfo.Type] = temp;

        // 面板显示和隐藏
        GetUI("Left_EQ_Info_u").HideUI();
        GetUI("Role_u").ShowUI();
        //show the new item
        ArticleType type = temp.ItemInfo.Type;
        string dressName = "Eq_" + type.ToString() + "_u";
        GetUI(dressName).GetComponent<UISprite>().spriteName = playerInfo.dressedEq[type].ItemInfo.Icon;

        string sprName = "Knapsack_Spr_";
        if (currentKnapsackIndex + 1 < 10) { sprName = sprName + "0" + (currentKnapsackIndex + 1).ToString() + "_u"; }
        else { sprName = sprName + (currentKnapsackIndex + 1).ToString() + "_u"; }
        GetUI(sprName).GetComponent<UISprite>().spriteName = playerInfo.knapsack[currentKnapsackIndex].ItemInfo.Icon;
        //update role panel property
        UpdateRoleProperties();
        //update the count
        if (playerInfo.knapsack[currentKnapsackIndex].ItemInfo.Icon == "")
            currentKnapsackCount--;
    }
    private void OnLeftEQLevelUpClick()
    {
        KnapsackItem kItem= playerInfo.knapsack[currentKnapsackIndex];
        if (kItem.Level> 10) return;
        else
        {
            kItem.Level++;
            kItem.ItemInfo.Quality++;
            kItem.HPPlus += kItem.Level * 10;
            kItem.PowerPlus += kItem.Level * 5;
            kItem.ItemInfo.Damage += kItem.ItemInfo.Level * 5;
            
            GetUI("Lab_EQ_Quality_n_u").GetComponent<UILabel>().text = kItem.ItemInfo.Quality.ToString();
            GetUI("Lab_EQ_Damage_n_u").GetComponent<UILabel>().text = kItem.ItemInfo.Damage.ToString();
            GetUI("Lab_EQ_HP_n_u").GetComponent<UILabel>().text = kItem.HPPlus.ToString();
            GetUI("Lab_EQ_Power_u").GetComponent<UILabel>().text = kItem.PowerPlus.ToString();
        }
    }
    
    /*右边装备详情按钮部分*/
    private void OnRightEQDreesDownClick()
    {
        int i = playerInfo.knapsack.Count;
        if (i >= 24) return;
        ArticleInfo a = new ArticleInfo();
        a.Icon = "";
        a.Type = currentDressUpType;
        playerInfo.knapsack.Add(new KnapsackItem(a));
        //exchange
        KnapsackItem temp;
        temp = playerInfo.knapsack[i];
        playerInfo.knapsack[i] = playerInfo.dressedEq[temp.ItemInfo.Type];
        playerInfo.dressedEq[temp.ItemInfo.Type] = temp;
        //隐藏和显示面板
        GetUI("Knapsack_u").ShowUI();
        GetUI("Right_EQ_Info_u").HideUI();
        //更新背包图片
        string sprName = "Knapsack_Spr_";
        if (i+1 < 10) { sprName = sprName + "0" + (i+1).ToString() + "_u"; }
        else { sprName = sprName + (i + 1).ToString() + "_u"; }
        GetUI(sprName).GetComponent<UISprite>().spriteName = playerInfo.knapsack[i].ItemInfo.Icon;
        //更新role 图片
        GetUI("Eq_" + currentDressUpType.ToString() + "_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[currentDressUpType].ItemInfo.Icon;
        //当前背包数目更新
        currentKnapsackCount++;
        GetUI("Lab_Knapsack_num_u").GetComponent<UILabel>().text = currentKnapsackCount + "/24";
    }
    private void OnRightEQLevelUpClick()
    {
        KnapsackItem kItem = playerInfo.dressedEq[currentDressUpType];
        if (kItem.Level > 10) return;
        else
        {
            kItem.Level++;
            kItem.ItemInfo.Quality++;
            kItem.HPPlus += kItem.Level * 10;
            kItem.PowerPlus += kItem.Level * 5;
            kItem.ItemInfo.Damage += kItem.ItemInfo.Level * 5;

            GetUI("Lab_EQ_Quality_n_Right_u").GetComponent<UILabel>().text = kItem.ItemInfo.Quality.ToString();
            GetUI("Lab_EQ_Damage_n_Right_u").GetComponent<UILabel>().text = kItem.ItemInfo.Damage.ToString();
            GetUI("Lab_EQ_HP_n_Right_u").GetComponent<UILabel>().text = kItem.HPPlus.ToString();
            GetUI("Lab_EQ_Power_Right_u").GetComponent<UILabel>().text = kItem.PowerPlus.ToString();
            UpdateRoleProperties();
        }
    }

    /*药品弹窗部分*/
    private void OnUseClick()
    {
        ArticleInfo a = playerInfo.knapsack[currentKnapsackIndex].ItemInfo;
        if (a.Type == ArticleType.Box)
            return;
        else
        {
            if (a.AppType == AppType.Energy)
            {
                int value = a.ApplyValue;
                playerInfo.Energy += value;
                playerInfo.knapsack[currentKnapsackIndex].Count--;
            }
           
            string sprName = "Knapsack_Spr_";
            if (currentKnapsackIndex + 1 < 10) { sprName = sprName + "0" + (currentKnapsackIndex + 1).ToString() + "_u"; }
             else { sprName = sprName + (currentKnapsackIndex + 1).ToString() + "_u"; }
             string labName = sprName.Replace("Spr", "Lab");
             if(playerInfo.knapsack[currentKnapsackIndex].Count == 0)
            {
                GetUI(sprName).GetComponent<UISprite>().spriteName = "";
                GetUI(labName).GetComponent<UILabel>().text = "";

                currentKnapsackCount--;
                GetUI("Lab_Knapsack_num_u").GetComponent<UILabel>().text = currentKnapsackCount.ToString() + "/24";
            }
            else
            {
                GetUI(labName).GetComponent<UILabel>().text = playerInfo.knapsack[currentKnapsackIndex].Count.ToString();
            }
            GetUI("Pan_Tool_u").HideUI();
        }
    }
    private void OnUseAllClick()
    {
        ArticleInfo a = playerInfo.knapsack[currentKnapsackIndex].ItemInfo;
        if (a.Type == ArticleType.Box)
            return;
        else
        {
            if (a.AppType == AppType.Energy)
            {
                int value = a.ApplyValue;
                playerInfo.Energy += value*playerInfo.knapsack[currentKnapsackIndex].Count;
                playerInfo.knapsack[currentKnapsackIndex].Count=0;
            }

            string sprName = "Knapsack_Spr_";
            if (currentKnapsackIndex + 1 < 10) { sprName = sprName + "0" + (currentKnapsackIndex + 1).ToString() + "_u"; }
            else { sprName = sprName + (currentKnapsackIndex + 1).ToString() + "_u"; }
            string labName = sprName.Replace("Spr", "Lab");
           
            GetUI(sprName).GetComponent<UISprite>().spriteName = "";
            GetUI(labName).GetComponent<UILabel>().text = "";
           
            GetUI("Pan_Tool_u").HideUI();

            currentKnapsackCount--;
            GetUI("Lab_Knapsack_num_u").GetComponent<UILabel>().text=currentKnapsackCount.ToString()+"/24";
        }
     }

    /*任务栏部分*/
    private void OnTaskCancleClick()
    {
        GetUI("TaskPanel_u").HideUI();
        GetUI("Reward_u").HideUI();
        SetBaseUIActive(true);
    }
    private void OnTaskFinshBtnClick(int index)
    {
        TaskProgress progress = playerInfo.playerTask[index].taskProgress;
        switch (progress)
        {
            case TaskProgress.UnStart:
                playerInfo.GetTask(index);//接受任务，处理逻辑
                InitShowTask();
                OnTaskCancleClick();
                break;
            case TaskProgress.Accepted:
                playerInfo.GetTask(index);//接受任务，处理逻辑
                GetUI("TaskPanel_u").HideUI();
                OnTaskCancleClick();
                break;
            case TaskProgress.Reward:
                GetUI("Reward_u").ShowUI();
                GetUI("Reward_Gold_u").GetComponent<UILabel>().text = "X"+playerInfo.playerTask[index].gold;
                GetUI("Reward_Diamond_u").GetComponent<UILabel>().text = "X" + playerInfo.playerTask[index].diamond;
                currentTaskIndex = index;
                //禁用其他任务按钮
                SetTaskButtonUse(false);
                break;
        }
    }

    /*领取奖赏部分*/
    private void OnRewardGetClick()
    {
        playerInfo.RewardTask(currentTaskIndex);
        InitShowTask();
        GetUI("Reward_u").HideUI();
        //开启其他任务按钮
        SetTaskButtonUse(true);
    }

    /*任务对话框部分*/
    private void OnTaskTalkBtnClick()
    {
        UIBehavior start = GetUI("TaskTalk_u");
        start.transform.DOScale(0, 0.2f).From(1);
        StartCoroutine(HideObjectDelay(start, 0.2f));
        //开启基本ui面板
        SetBaseUIActive(true);
        //调用自动寻路到副本处
        GameObject raid= GameControl.Instance.npcManager.raidPosition;
        GameControl.Instance.playerAutoMove.OpenPlayerAutoMove(raid);
    }
    //在playerAutoMove中调用
    public void OnPlayerReachedNpc()
    {
        UIBehavior start = GetUI("TaskTalk_u");
        start.ShowUI();
        start.transform.DOScale(1, 0.2f).From(0);
        //显示当前任务的任务描述
        Task task = playerInfo.GetCurrentTask();
        GetUI("TaskTalk_Lab_u").GetComponent<UILabel>().text = task.talkWithNpc;
        //关闭基本ui
        SetBaseUIActive(false);
        //关闭任务面板
        GetUI("TaskPanel_u").HideUI();
    }

    #endregion

    #region 辅助方法
    //隐藏初始化时候的不需要的面板
    private void InitHidePanel()
    {
        GetUI("PlayStatus_u").HideUI();//个人信息
        GetUI("Knapsack_u").HideUI();//背包
        GetUI("Role_u").HideUI();//角色物品
        GetUI("Left_EQ_Info_u").HideUI();//左边物品详情
        GetUI("Right_EQ_Info_u").HideUI();//右边物品详情
        GetUI("Pan_Tool_u").HideUI();//道具弹框
        GetUI("TaskPanel_u").HideUI();//任务栏隐藏
        GetUI("Reward_u").HideUI();//奖赏领取
        GetUI("TaskTalk_u").HideUI();//任务对话框
        
    }
    //背包初始化值时候的循环方法
    private void InitShowKnapsackItem()
    {
        #region 背包物品部分
        for (int i = 0; i < 24; i++)
        {
            int num = playerInfo.knapsack.Count;
            string sprName = "Knapsack_Spr_";
            if (i + 1 < 10) { sprName = sprName + "0" + (i + 1).ToString() + "_u"; }
            else { sprName = sprName + (i + 1).ToString() + "_u"; }
            string labName = sprName.Replace("Spr","Lab");
            if (i + 1 <= num)
            {
                GetUI(sprName).GetComponent<UISprite>().spriteName = playerInfo.knapsack[i].ItemInfo.Icon;
                if (playerInfo.knapsack[i].ItemInfo.Type == ArticleType.Box || playerInfo.knapsack[i].ItemInfo.Type == ArticleType.Drug)
                {
                    GetUI(labName).GetComponent<UILabel>().text = playerInfo.knapsack[i].Count.ToString();
                }
                else
                {
                    GetUI(labName).GetComponent<UILabel>().text = "";
                }
            }
            else
            {
                GetUI(sprName).GetComponent<UISprite>().spriteName = "";
                GetUI(labName).GetComponent<UILabel>().text = "";
            }
        }
        #endregion
        #region 背包文字部分
        GetUI("Lab_Knapsack_num_u").GetComponent<UILabel>().text = playerInfo.knapsack.Count.ToString() + "/24";
        GetUI("Lab_Sold_Price_u").GetComponent<UILabel>().text = "";
        #endregion
        currentKnapsackCount = playerInfo.knapsack.Count;//用currentKnapsackIndexCount显示当前拥有的物品数目
    }
    //装备穿戴面板
    private void InitShowDressedUpItem()
    {
        #region 角色穿戴显示
        GetUI("Eq_Helmet_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[ArticleType.Helmet].ItemInfo.Icon;
        GetUI("Eq_Clothe_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[ArticleType.Clothe].ItemInfo.Icon;
        GetUI("Eq_Weapon_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[ArticleType.Weapon].ItemInfo.Icon;
        GetUI("Eq_Shoe_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[ArticleType.Shoe].ItemInfo.Icon;
        GetUI("Eq_Necklace_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[ArticleType.Necklace].ItemInfo.Icon;
        GetUI("Eq_Bracelet_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[ArticleType.Bracelet].ItemInfo.Icon;
        GetUI("Eq_Ring_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[ArticleType.Ring].ItemInfo.Icon;
        GetUI("Eq_Wing_u").GetComponent<UISprite>().spriteName = playerInfo.dressedEq[ArticleType.Wing].ItemInfo.Icon;
        #endregion
        #region 数值信息显示
        UpdateRoleProperties();
        #endregion
    }
    //背包item点击事件循环注册方法
    private void RegisterKnapsackItemClickEvent()
    {
        for(int i = 0; i < 24; i++)
        {
            string num = (i + 1) < 10 ? "0" + (i+1) : "" + (i+1);
            string boxName = "Knapsack_Item_"+num+"_u";
            int index = i;
            GetUI(boxName).RegisterOnClick(new EventDelegate(() => { OnKnapsackItemClick(num,index); }));
        }
    }
    //角色装备item点击事件注册方法
    private void RegisterRoleItemClickEvent()
    {
        GetUI("Eq_Helmet_Box_u").RegisterOnClick(new EventDelegate(()=> { OnRoleItemClick(ArticleType.Helmet); }));
        GetUI("Eq_Clothe_Box_u").RegisterOnClick(new EventDelegate(() => { OnRoleItemClick(ArticleType.Clothe); }));
        GetUI("Eq_Weapon_Box_u").RegisterOnClick(new EventDelegate(() => { OnRoleItemClick(ArticleType.Weapon); }));
        GetUI("Eq_Shoe_Box_u").RegisterOnClick(new EventDelegate(() => { OnRoleItemClick(ArticleType.Shoe); }));
        GetUI("Eq_Necklace_Box_u").RegisterOnClick(new EventDelegate(() => { OnRoleItemClick(ArticleType.Necklace); }));
        GetUI("Eq_Bracelet_Box_u").RegisterOnClick(new EventDelegate(() => { OnRoleItemClick(ArticleType.Bracelet); }));
        GetUI("Eq_Ring_Box_u").RegisterOnClick(new EventDelegate(() => { OnRoleItemClick(ArticleType.Ring); }));
        GetUI("Eq_Wing_Box_u").RegisterOnClick(new EventDelegate(() => { OnRoleItemClick(ArticleType.Wing); }));
    }
    //更新role面板属性值方法
    private void UpdateRoleProperties()
    {
        int hpUp = 0;
        int powUp = 0;
        int baseHp = playerInfo.CharacterLevel * 100 + 10;
        int basePow = playerInfo.CharacterLevel * 50 + 5;
        foreach (var t in playerInfo.dressedEq)
        {
            hpUp += t.Value.HPPlus;
            powUp += t.Value.PowerPlus;
        }
        GetUI("Lab_HP_u").GetComponent<UILabel>().text = hpUp + baseHp + "";
        GetUI("Lab_Damage_u").GetComponent<UILabel>().text = powUp + basePow + "";
        GetUI("Lab_EXP_u").GetComponent<UILabel>().text = playerInfo.CharacterExp + " /" + playerInfo.LevelUpNeedExp();
        GetUI("Spr_EXP_BG_u").GetComponent<UISlider>().value = (float)playerInfo.CharacterExp / playerInfo.LevelUpNeedExp();
    }
    //循环设置背景ui的button可用性
    private void SetBaseUIActive(bool isUse)
    {
        Transform t = GameObject.Find("HeadFrame").transform;
        UIButton[] childs = t.GetComponentsInChildren<UIButton>();
        foreach(var c in childs)
        {
            c.enabled = isUse;
        }
         t = GameObject.Find("TopBar").transform;
         childs = t.GetComponentsInChildren<UIButton>();
        foreach (var c in childs)
        {
            c.enabled = isUse;
        }
         t = GameObject.Find("BottonBar").transform;
         childs = t.GetComponentsInChildren<UIButton>();
        foreach (var c in childs)
        {
            c.enabled = isUse;
        }
    }
    //初始化任务面板
    private void InitShowTask()
    {
        List<Task> pTasks = GameControl.Instance.playerInfo.playerTask;
        int count = pTasks.Count;
        //pTasks[1].taskProgress = TaskProgress.Reward;///////////记得删除掉
        for (int i = 0; i < 4; i++) //4 is magic number!!ahaha
        {
            
            if (i < count)
            {
                string name="";
                switch (pTasks[i].type)
                {
                    case TaskType.Main:
                        name = "pic_主线";
                        break;
                    case TaskType.Daily:
                        name = "pic_日常";
                        break;
                    case TaskType.Reward:
                        name = "pic_奖赏";
                        break;
                }
                GetUI("Task_" + (i + 1).ToString() + "_Title_u").GetComponent<UISprite>().spriteName = name;
                GetUI("Task_" + (i + 1).ToString() + "_Icon_u").GetComponent<UISprite>().spriteName = pTasks[i].icon;
                GetUI("Task_" + (i + 1).ToString() + "_Name_u").GetComponent<UILabel>().text = pTasks[i].name;
                GetUI("Task_" + (i + 1).ToString() + "_Des_u").GetComponent<UILabel>().text = pTasks[i].des;
                GetUI("Task_" + (i + 1).ToString() + "_Gold_u").GetComponent<UILabel>().text = "X"+pTasks[i].gold;
                GetUI("Task_" + (i + 1).ToString() + "_Diamond_u").GetComponent<UILabel>().text = "X" + pTasks[i].diamond;
                string name1 = "";
                string name2 = "";
                string name3 = "";
                string name4 = "";
                Color color = Color.white;
                switch (pTasks[i].taskProgress)
                {
                    case TaskProgress.UnStart:
                        name1 = "公用四字按钮198x70_1";
                        name2 = "公用四字按钮198x70_2";
                        name3 = "公用四字按钮198x70_3";
                        name4 = "公用四字按钮198x70_4";
                        name = "接受任务";
                        break;
                    case TaskProgress.Accepted:
                        name1 = "公用四字按钮198x70_1";
                        name2 = "公用四字按钮198x70_2";
                        name3 = "公用四字按钮198x70_3";
                        name4 = "公用四字按钮198x70_4";
                        name = "任务进行中";
                        color = Color.yellow;
                        break;
                    case TaskProgress.Reward:
                        name1 = "btn_领取奖励1";
                        name2 = "btn_领取奖励2";
                        name3 = "btn_领取奖励3";
                        name4 = "btn_领取奖励4";
                        name = "领取奖励";
                        color = Color.red;
                        break;
                }
                GetUI("Btn_" + (i + 1).ToString() + "_u").GetComponent<UISprite>().spriteName = name1;
                GetUI("Btn_" + (i + 1).ToString() + "_u").GetComponent<UIButton>().normalSprite = name1;
                GetUI("Btn_" + (i + 1).ToString() + "_u").GetComponent<UIButton>().hoverSprite = name2;
                GetUI("Btn_" + (i + 1).ToString() + "_u").GetComponent<UIButton>().pressedSprite = name3;
                GetUI("Btn_" + (i + 1).ToString() + "_u").GetComponent<UIButton>().disabledSprite = name4;
                GetUI("Btn_Lab_" + (i + 1).ToString() + "_u").GetComponent<UILabel>().text = name;
                GetUI("Btn_Lab_" + (i + 1).ToString() + "_u").GetComponent<UILabel>().color = color;
            }
            else
            {
                GetUI("Task_" + (i + 1).ToString() + "_u").HideUI();    
            }
        }
    }
    //任务面板完成任务按钮循环注册
    private void RegisterTaskBtnClickEvent()
    {
        for(int i = 0; i < 4; i++)//i=4 is magic number!
        {
            string btnName = "Btn_" + (i + 1) + "_u";
            int index = i;
            GetUI(btnName).RegisterOnClick(new EventDelegate(() => { OnTaskFinshBtnClick(index); }));
        }
    }
    //任务面板任务完成按钮禁用与开启
    private void SetTaskButtonUse(bool isUse)
    {
        for(int i = 1; i < 5; i++) //i<5 is magic number!!
        {
            string name = "Btn_" + i + "_u";
            GetUI(name).GetComponent<UIButton>().enabled = isUse;
        }
    }
}
    #endregion

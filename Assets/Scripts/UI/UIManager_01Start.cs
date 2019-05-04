using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class UIManager_01Start : UIManagerBase {

    private void Start()
    {
        #region Start场景事件绑定
        GetUI("Btn_User_u").RegisterOnClick(new EventDelegate(OnUsernameClick));
        GetUI("Btn_Server_u").RegisterOnClick(new EventDelegate(OnServerClick));
        GetUI("Btn_Enter_u").RegisterOnClick(new EventDelegate(OnEnterGameClick));
        GetUI("Btn_Login_u").RegisterOnClick(new EventDelegate(OnSignInClick));
        GetUI("Btn_SignUp_u").RegisterOnClick(new EventDelegate(OnSignUpClick));
        GetUI("Btn_Cancle_Login_u").RegisterOnClick(new EventDelegate(OnLoginCancelClick));
        GetUI("Btn_Cancle_SignUp_u").RegisterOnClick(new EventDelegate(OnSignUpCancelClick));
        GetUI("Btn_Cancel_u").RegisterOnClick(new EventDelegate(OnSignUpCancelClick));
        GetUI("Btn_Confirm_u").RegisterOnClick(new EventDelegate(OnSignUpConfirmClick));
        RegisterAllServerItems();//注册所有服务器标签点击事件
        GetUI("Btn_ServerSelected_u").RegisterOnClick(new EventDelegate(OnServerSelectedClick));
        GetUI("Btn_ChangeCharacter_u").RegisterOnClick(new EventDelegate(OnChangeCharacterClick));
        GetUI("Btn_CharacteChooseReturn_u").RegisterOnClick(new EventDelegate(OnCharaterChooseReturnClick));
        GetUI("Btn_CharacterReturn_u").RegisterOnClick(new EventDelegate(OnCharacterNewReturnClick));
        RegisterAllCharacterItems();//注册所有角色的点击事件
        GetUI("Btn_CharacterConfirm_u").RegisterOnClick(new EventDelegate(OnCharacterConfirmClick));
        #endregion

        //完成绑定后，隐藏多余panel
        GetUI("Pan_Login_u").HideUI();
        GetUI("Pan_SignUp_u").HideUI();
        GetUI("Pan_Server_u").HideUI();
        GetUI("Pan_CharacterChoose_u").HideUI();
        GetUI("Pan_CharacterNew_u").HideUI();
    }


    #region 事件触发的方法
    /// <summary>
    /// "进入游戏"按钮点击回调方法
    /// </summary>
    private void OnEnterGameClick()
    {
        //TODO: 1.连接服务器，验证账号，初始化账号角色信息
        //2.进入游戏界面
        UIBehavior current = GetUI("Pan_Start_u");
        UIBehavior target = GetUI("Pan_CharacterChoose_u");
        current.transform.DOScale(0, 0.1f).From(1).SetEase(Ease.Linear);
        StartCoroutine(HideObjectDelay(current, 0.1f));
        target.ShowUI();
        target.transform.DOLocalMove(new Vector3(4, 16, 0), 0.2f).From(new Vector3(-1380, 16, 0)).SetEase(Ease.Linear);
        StartCoroutine(SetAchorOffsetDelay(target.transform, GameObject.Find("UI Root").transform, 0.1f));
    }
    /// <summary>
    /// "用户名"按钮点击回调方法
    /// </summary>
    private void OnUsernameClick()
    {
        ChangePanel("Pan_Start_u", "Pan_Login_u");
    }
    /// <summary>
    /// “服务器”按钮点击回调方法
    /// </summary>
    private void OnServerClick()
    {
        ChangePanel("Pan_Start_u","Pan_Server_u");
        ////ToDo:::::调用初始化服务器列表方法
        InitServerList();
    }
    /// <summary>
    /// "注册"按钮点击回调函数
    /// </summary>
    private void OnSignUpClick()
    {
        ClearLogin();
        ChangePanel("Pan_Login_u", "Pan_SignUp_u");
    }
    /// <summary>
    ///"登录"按钮点击
    /// </summary>
    private void OnSignInClick()
    {
        //ToDo:账号验证相关

        //面板切换
        string username = GetInputValue("Input_Username_Login_u");
        ClearLogin();
        ChangePanel("Pan_Login_u","Pan_Start_u");
        GetUI("Lab_User_u").GetComponent<UILabel>().text = username;
    }
    /// <summary>
    /// "取消"-登录界面
    /// </summary>
    private void OnLoginCancelClick()
    {
        ClearLogin();
        ChangePanel("Pan_Login_u", "Pan_Start_u");
    }
    /// <summary>
    /// "取消"-注册界面
    /// </summary>
    private void OnSignUpCancelClick()
    {
        ClearSignUp();
        ChangePanel("Pan_SignUp_u","Pan_Start_u");
    }
    /// <summary>
    /// "确认并登录"
    /// </summary>
    private void OnSignUpConfirmClick()
    {
        //ToDo: 正确性验证

        //面板切换
        string username = GetInputValue("Input_Username_SignUp_u");
        ClearSignUp();
        ChangePanel("Pan_SignUp_u", "Pan_Start_u");
        GetUI("Lab_User_u").GetComponent<UILabel>().text = username;
    }
    
    //“已选择服务器” 点击
    private void OnServerSelectedClick()
    {
        string serverName = GetUI("Lab_ServerSelected_u").GetComponent<UILabel>().text;
        ChangePanel("Pan_Server_u","Pan_Start_u");
        GetUI("Lab_Server_u").GetComponent<UILabel>().text = serverName;
    }
    //"更换角色" 点击
    private void OnChangeCharacterClick()
    {
        UIBehavior start = GetUI("Pan_CharacterChoose_u");
        UIBehavior end = GetUI("Pan_CharacterNew_u");
        start.GetComponent<UIWidget>().SetAnchor(start.gameObject,0,0,0,0); 
        start.transform.DOLocalMove(new Vector3(-1380, 16, 0), 0.2f).From(new Vector3(4, 16, 0)).SetEase(Ease.Linear);
        StartCoroutine(HideObjectDelay(start, 0.1f));
        end.ShowUI();
        end.transform.DOLocalMove(new Vector3(4,16,0),0.2f).From(new Vector3(1341,16,0)).SetEase(Ease.Linear);
        StartCoroutine(SetAchorOffsetDelay(end.transform,GameObject.Find("UI Root").transform, 0.1f));
    }
    //"返回"按钮点击【角色选择面板】
    private void OnCharaterChooseReturnClick()
    {
        UIBehavior start = GetUI("Pan_CharacterChoose_u");
        UIBehavior end = GetUI("Pan_Start_u");
        start.GetComponent<UIWidget>().SetAnchor(start.gameObject, 0, 0, 0, 0);
        start.transform.DOLocalMove(new Vector3(-1380, 16, 0), 0.2f).From(new Vector3(4, 16, 0)).SetEase(Ease.Linear);
        StartCoroutine(HideObjectDelay(start, 0.2f));
        end.ShowUI();
        end.transform.DOScale(1, 0.2f).From(0).SetEase(Ease.Linear);
    }

    //" 返回"按钮点击【角色创建面板】
    private void OnCharacterNewReturnClick()
    {
        UIBehavior start = GetUI("Pan_CharacterNew_u");
        UIBehavior end = GetUI("Pan_CharacterChoose_u");
        start.GetComponent<UIWidget>().SetAnchor(start.gameObject, 0, 0, 0, 0);
        start.transform.DOLocalMove(new Vector3(1346, 16, 0), 0.2f).From(new Vector3(4, 16, 0)).SetEase(Ease.Linear);
        StartCoroutine(HideObjectDelay(start, 0.1f));
        end.ShowUI();
        end.transform.DOLocalMove(new Vector3(4, 16, 0), 0.2f).From(new Vector3(-1380, 16, 0)).SetEase(Ease.Linear);
        StartCoroutine(SetAchorOffsetDelay(end.transform, GameObject.Find("UI Root").transform, 0.1f));
    }
    //待选“角色”被点击
    private void OnCharacterClick(CharacterType type)
    {
        GameControl.Instance.playerInfo.CharacterType = type;
        foreach(CharacterType t in GameControl.Instance.cList)
        {
            string name = "Character_" + t.ToString() + "_u";
            if (t == type)
            {
                GetUI(name).transform.DOScale(250,0.3f);
            }
            else
            {
                GetUI(name).transform.DOScale(200f, 0.3f);
            }
        }        
    }
    public GameObject[] characters;
    //角色“确认”按钮被点击
    private void OnCharacterConfirmClick()
    {
        //1.ToDo:昵称验证
        //2.Panel切换
        OnCharacterNewReturnClick();
        //3.角色信息更改显示
        GameControl.Instance.playerInfo.CharacterLevel = 1;   //默认等级为1级
        string nikName = GetUI("Input_NikiName_u").GetComponent<UIInput>().value;
        GameControl.Instance.playerInfo.CharacterName = nikName == "" ? "bia" : nikName;//默认名字显示bia
        GetUI("Lab_CharacterName_u").GetComponent<UILabel>().text = GameControl.Instance.playerInfo.CharacterName;
        GetUI("Lab_Level_u").GetComponent<UILabel>().text = "LV." + GameControl.Instance.playerInfo.CharacterLevel.ToString();
        //4.角色模型创建
        Transform createPos = GetUI("CharacterCreatePos_u").transform;
        if (createPos.childCount != 0)
        {
            GameObject.Destroy(createPos.GetChild(0).gameObject);
        }
        GameObject go= NGUITools.AddChild(createPos.gameObject, characters[(int)GameControl.Instance.playerInfo.CharacterType]);
        go.layer = 8;
        go.transform.localScale = new Vector3(200,200,200);
        go.transform.Rotate(new Vector3(0,180,0), Space.Self);
       //go.GetComponent<UIWidget>().SetAnchor(createPos.gameObject,)
    }


    #endregion

    #region 其他辅助方法
    
    /// <summary>
    /// 改变当前panel，并使用相关动画
    /// </summary>
    /// <param name="currentPanel"></param>
    /// <param name="targetPanel"></param>
    private void ChangePanel(string currentPanel,string targetPanel)
    {
        UIBehavior current = GetUI(currentPanel);
        UIBehavior target = GetUI(targetPanel);
        current.transform.DOScale(0, 0.2f).From(1).SetEase(Ease.Linear);
        StartCoroutine(HideObjectDelay(current, 0.2f));
        target.ShowUI();
        target.transform.DOScale(1, 0.2f).From(0).SetEase(Ease.Linear);
    }
    //获得Input输入的信息
    private string GetInputValue(string UIName)
    {
       return GetUI(UIName).GetComponent<UIInput>().value;
    }
    //清空登录panel信息
    private void ClearLogin()
    {
        GetUI("Input_Username_Login_u").GetComponent<UIInput>().value = "";
        GetUI("Input_Passward_Login_u").GetComponent<UIInput>().value = "";
    }
    //清空注册panel信息
    private void ClearSignUp()
    {
        GetUI("Input_Username_SignUp_u").GetComponent<UIInput>().value = "";
        GetUI("Input_Passward_SignUp_u").GetComponent<UIInput>().value = "";
        GetUI("Input_PasswardConfirm_SignUp_u").GetComponent<UIInput>().value = "";
    }
    //初始化服务器列表
    public UIAtlas atlas;
    private void InitServerList()
    {
      List<ServerItem> serverItems= GameControl.Instance.mServerlist.SList;
       for(int i=0;i< serverItems.Count; i++)
        {
            serverItems[i].ConnectCount = UnityEngine.Random.Range(0, 100); //模拟网络连接数TODO：改为服务器验证
            if (serverItems[i].ConnectCount < 50)
            {
                //free 状态修改
                SetServerState(i, serverItems[i].Name, true);
            }
            else
            {
                //busy 状态修改
                SetServerState(i, serverItems[i].Name, false);
            }
        }
    }
    //服务器状态ui改变
    private void SetServerState(int index,string name,bool isfree)
    {
        string uiBtnName = "Btn_Server_" + ((index + 1) / 10).ToString() + ((index + 1) % 10).ToString() + "_u";
        string uiLabName = "Lab_Server_" + ((index + 1) / 10).ToString() + ((index + 1) % 10).ToString() + "_u";
        UIBehavior btn = GetUI(uiBtnName);
        UIBehavior lab = GetUI(uiLabName);
        lab.GetComponent<UILabel>().text = name;
        if (isfree)
        {
            btn.GetComponent<UIButton>().normalSprite = "btn_流畅1";
            btn.GetComponent<UIButton>().hoverSprite = "btn_流畅2";
            btn.GetComponent<UIButton>().pressedSprite = "btn_流畅3";
            btn.GetComponent<UIButton>().disabledSprite = "btn_流畅4";
        }
        else
        {
            btn.GetComponent<UIButton>().normalSprite = "btn_火爆1";
            btn.GetComponent<UIButton>().hoverSprite = "btn_火爆2";
            btn.GetComponent<UIButton>().pressedSprite = "btn_火爆3";
            btn.GetComponent<UIButton>().disabledSprite = "btn_火爆4";
        }
    }

    ///服务器标签点击事件注册【封装了一个方法】
    private void RegisterAllServerItems()
    {
        int count = GameControl.Instance.mServerlist.SList.Count;
        for(int i = 0; i < count; i++)
        {
            string btnName= "Btn_Server_" + ((i + 1) / 10).ToString() + ((i + 1) % 10).ToString() + "_u"; 
            string labName= "Lab_Server_" + ((i + 1) / 10).ToString() + ((i + 1) % 10).ToString() + "_u";
            UIButton btn = GetUI(btnName).GetComponent<UIButton>();
            EventDelegate.Add(btn.onClick, () => { OnServerItemClick(btnName, labName); });
        }
    }

    //服务器标签点击回调方法
    private void OnServerItemClick(string btnName,string labName)
    {
        string targetBtnName = "Btn_ServerSelected_u";
        string targetLabName = "Lab_ServerSelected_u";
        UIBehavior tBtn = GetUI(targetBtnName);
        UIBehavior tLab = GetUI(targetLabName);
        UIBehavior cBtn = GetUI(btnName);
        UIBehavior cLab = GetUI(labName);
        tLab.GetComponent<UILabel>().text = cLab.GetComponent<UILabel>().text;
        tBtn.GetComponent<UIButton>().normalSprite = cBtn.GetComponent<UIButton>().normalSprite;
        tBtn.GetComponent<UIButton>().hoverSprite = cBtn.GetComponent<UIButton>().hoverSprite;
        tBtn.GetComponent<UIButton>().pressedSprite = cBtn.GetComponent<UIButton>().pressedSprite;
        tBtn.GetComponent<UIButton>().disabledSprite = cBtn.GetComponent<UIButton>().disabledSprite;
    }
    //定时开启obj的anchor设置
    IEnumerator SetAchorOffsetDelay(Transform obj,Transform  target,float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.GetComponent<UIWidget>().SetAnchor(target.gameObject,0,-31,0,32);
    }
    //注册所有角色的点击事件
    private void RegisterAllCharacterItems()
    {
        foreach (CharacterType t in GameControl.Instance.cList)
        {
            string name = "Character_" + t.ToString() + "_u";
            UIButton btn = GetUI(name).GetComponent<UIButton>();
            EventDelegate.Add(btn.onClick, () => { OnCharacterClick(t); });
        }
    }
   
    #endregion

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TaidouCommon.Model;
using UnityEngine.SceneManagement;


public class UIManager_01Start : UIManagerBase {

    private void Start()
    {
        //主动向manager登记自己
        GameControl.Instance.mUIManager = this;
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
        //进入游戏按钮，场景切换
        GetUI("Btn_CharacterEnter_u").RegisterOnClick(new EventDelegate(OnScene_StartToScene_Village));
        //手机登录界面
        GetUI("Btn_Phone_u").RegisterOnClick(new EventDelegate(OnSwitchPhoneLoginPanel));
        GetUI("Btn_Cancle_Phone_Enter_u").RegisterOnClick(new EventDelegate(OnReturnLoginPanel));
        GetUI("Btn_Send_u").RegisterOnClick(new EventDelegate(OnSendPhone));
        GetUI("Btn_Phone_Enter_u").RegisterOnClick(new EventDelegate(OnPhoneLogin));
        #endregion

        //完成绑定后，隐藏多余panel
        GetUI("Pan_Login_u").HideUI();
        GetUI("Pan_SignUp_u").HideUI();
        GetUI("Pan_Server_u").HideUI();
        GetUI("Pan_CharacterChoose_u").HideUI();
        GetUI("Pan_CharacterNew_u").HideUI();
        GetUI("Pan_Message_u").HideUI();
        GetUI("Loading_u").HideUI();
        GetUI("Pan_Phone_u").HideUI();


        //变量初始化
        isCharacterClicked = false;
    }


    #region 事件触发的方法
    /// <summary>
    /// "进入游戏"按钮点击回调方法
    /// </summary>
    private void OnEnterGameClick()
    {
        //进入游戏界面
        if (IsSignIn)
        {
            if (GameControl.Instance.playerInfo.CharacterName == null)
            {
                ShowMessagePanel("请创建角色",1f);
            }
            UIBehavior current = GetUI("Pan_Start_u");
            UIBehavior target = GetUI("Pan_CharacterChoose_u");
            current.transform.DOScale(0, 0.1f).From(1).SetEase(Ease.Linear);
            StartCoroutine(HideObjectDelay(current, 0.1f));
            target.ShowUI();
            target.transform.DOLocalMove(new Vector3(4, 16, 0), 0.2f).From(new Vector3(-1380, 16, 0)).SetEase(Ease.Linear);
            StartCoroutine(SetAchorOffsetDelay(target.transform, GameObject.Find("UI Root").transform, 0.1f));
        }
        else
        {
            ShowMessagePanel("账号验证失败，请输入正确账号", 1f);
        }
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
        //账号验证相关
        string username = GetInputValue("Input_Username_Login_u");
        string password = GetInputValue("Input_Passward_Login_u");
        if (username.Length <= 3)
        {
            ShowMessagePanel("非法用户名，长度要大于3", 1f);
            return;
        }
        if (password.Length <= 3)
        {
            ShowMessagePanel("非法密码，长度要大于3", 1f);
            return;
        }
        GetComponent<LoginHandler>().Login(username,password);//向服务器发出登录请求
    }
   
    /// <summary>
    /// "取消"-登录界面
    /// </summary>
    private void OnLoginCancelClick()
    {
        ClearLogin();
        ChangePanel("Pan_Login_u", "Pan_Start_u");
    }

    //切换到手机登录界面
    private void OnSwitchPhoneLoginPanel()
    {
        ClearLogin();
        ChangePanel("Pan_Login_u", "Pan_Phone_u");
    }
    //手机界面回到普通登录界面
    private void OnReturnLoginPanel()
    {
        GetUI("Input_Phone_u").GetComponent<UIInput>().value = "";
        GetUI("Input_Code_u").GetComponent<UIInput>().value = "";
        ChangePanel("Pan_Phone_u", "Pan_Login_u");
        code = -1;
        phone = "";
    }

    int code = -1;
    bool isSendPhone = false;
    string phone = "";
    //"发送手机号码"按钮被点击，获取验证码请求 发送
    private void OnSendPhone()
    {
        string phoneStr = GetUI("Input_Phone_u").GetComponent<UIInput>().value;
        if (string.IsNullOrEmpty(phoneStr))
        {
            ShowMessagePanel("请填入手机号码", 1f);
            return;
        }
        code = Sms.Send(phoneStr);
        GetUI("Btn_Send_u").DisableUIButton();
        isSendPhone = true;//开启计时器
        ShowMessagePanel("验证码已发送", 1f);
        phone = phoneStr;//后面验证手机号是否更改
    }
    //"手机登录"按钮被点击
    private void OnPhoneLogin()
    {
        string inputCode= GetUI("Input_Code_u").GetComponent<UIInput>().value;
        if (!code.ToString().Equals(inputCode))
        {
            ShowMessagePanel("验证码错误", 1f);
            GetUI("Input_Code_u").GetComponent<UIInput>().value = "";
            return;
        }
        if (!phone.Equals(GetUI("Input_Phone_u").GetComponent<UIInput>().value))
        {
            ShowMessagePanel("验证码错误",1f);
            GetUI("Input_Code_u").GetComponent<UIInput>().value = "";
            return;
        }
        //发送正确手机号进行登录
        GetComponent<LoginHandler>().Login(phone);
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
        string username = GetInputValue("Input_Username_SignUp_u");
        string password = GetInputValue("Input_Passward_SignUp_u");
        string passwordConfirm = GetInputValue("Input_PasswardConfirm_SignUp_u");
        if (username.Length<=3)
        {
            ShowMessagePanel("非法用户名，长度要大于3", 1f);
            return;
        }
        if (password.Length <= 3)
        {
            ShowMessagePanel("非法密码，长度要大于3", 1f);
            return;
        }
        if (password != passwordConfirm)
        {
            ShowMessagePanel("密码两次输入不一致，请确认",1f);
            return;
        }
        //向服务器发起注册请求 
        GetComponent<RegisterHandler>().Register(username, password);
     
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
        //重置点击变量
        isCharacterClicked = false;
        //先从数据库中更新一波player
        GetComponent<RoleHandler>().GetRole();

        UIBehavior start = GetUI("Pan_CharacterNew_u");
        UIBehavior end = GetUI("Pan_CharacterChoose_u");
        start.GetComponent<UIWidget>().SetAnchor(start.gameObject, 0, 0, 0, 0);
        start.transform.DOLocalMove(new Vector3(1346, 16, 0), 0.2f).From(new Vector3(4, 16, 0)).SetEase(Ease.Linear);
        StartCoroutine(HideObjectDelay(start, 0.1f));
        end.ShowUI();
        end.transform.DOLocalMove(new Vector3(4, 16, 0), 0.2f).From(new Vector3(-1380, 16, 0)).SetEase(Ease.Linear);
        StartCoroutine(SetAchorOffsetDelay(end.transform, GameObject.Find("UI Root").transform, 0.1f));

        //输入框文字清空
        GetUI("Input_NikiName_u").GetComponent<UIInput>().value = "";
        //角色大小重置
        ResetCharacterModel();
       
    }
    //待选“角色”被点击
    private void OnCharacterClick(CharacterType type)
    {
        //置角色选择变量为true
        isCharacterClicked = true;

        GameControl.Instance.playerInfo.CharacterType = type;
        foreach(CharacterType t in GameControl.Instance.cList)
        {
            string name = "Character_" + t.ToString() + "_u";
            if (t == type)
            {
                GetUI(name).transform.DOScale(200f,0.3f);
            }
            else
            {
                GetUI(name).transform.DOScale(180f, 0.3f);
            }
        }        
    }
    public GameObject[] characters;
    private bool isCharacterClicked = false;
    //角色“确认”按钮被点击
    private void OnCharacterConfirmClick()
    {
        if (isCharacterClicked == false)
        {
            ShowMessagePanel("请先选择一个角色",1f);
            return;
        }
        //角色昵称验证
        string nikName= GetUI("Input_NikiName_u").GetComponent<UIInput>().value;
        bool isman = GameControl.Instance.playerInfo.CharacterType == CharacterType.Man ? true : false;
        Role role = new Role() { user=null,name=nikName,isman=isman,
            //下面都是默认创建角色的值
            level =1,
            power=100,
            exp=0,
            diamond=666,
            gold=666,
            physical=0,
            energy=0
        };
        GetComponent<RoleHandler>().AddRole(role);
    }

    //"进入游戏"被点击，切换新手村场景
    //使用协程来完成，可以不用把判断放在update中了
    private void OnScene_StartToScene_Village()
    {
        StartCoroutine(LoadingToScene_Village());
    }
    IEnumerator LoadingToScene_Village()
    {
        //ui切换
        GetUI("Loading_u").ShowUI();
        GetUI("Pan_CharacterChoose_u").HideUI();
        UISlider slider = GetUI("Loading_Bar_bg_u").GetComponent<UISlider>();
        slider.value = 0;
        //异步加载场景
        AsyncOperation asy = SceneManager.LoadSceneAsync(1);
        asy.allowSceneActivation = false;
        while (true)
        {
            yield return null;
            slider.value = Mathf.Lerp(slider.value, asy.progress, Time.deltaTime);
            if (Mathf.Abs(slider.value - 0.9f) <= 0.02f)
            {
                while (true)
                {
                    yield return null;
                    slider.value = Mathf.Lerp(slider.value, 1f, Time.deltaTime);
                    if (Mathf.Abs(slider.value - 1f) <= 0.02f)
                    {
                        slider.value = 1f;
                        yield return new WaitForSeconds(1f);
                        asy.allowSceneActivation = true;
                        break;
                    }
                }
                break;
            }
        }
        yield return null;
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
    //public UIAtlas atlas;
    private void InitServerList()
    {
        List<Server> servers = GameControl.Instance.serverList;
        if (servers == null)
        {
            isGetServerList = false;
            GetUI("Lab_Server_u").GetComponent<UILabel>().text = "加载中...";
            GetUI("Btn_Server_u").DisableUIButton();
            for(int i = 0; i < SERVERCOUNT; i++)
            {
                string uiBtnName = "Btn_Server_" + ((i + 1) / 10).ToString() + ((i + 1) % 10).ToString() + "_u";
                UIBehavior btn = GetUI(uiBtnName);
                btn.DisableUIButton();
            }
            GetUI("Btn_ServerSelected_u").DisableUIButton();
        }
        else
        {
            isGetServerList = true;
            GetUI("Lab_Server_u").GetComponent<UILabel>().text = servers[0].name;
            GetUI("Btn_Server_u").EnableUIButton();
            for(int i = 0; i < servers.Count; i++)
            {
                bool state = servers[i].count < 50;
                SetServerState(i, servers[i].name,state);//循环设置状态
            }
            UIBehavior btn0 = GetUI("Btn_Server_01_u");
            UIBehavior lab0 = GetUI("Lab_Server_01_u");
            GetUI("Lab_ServerSelected_u").GetComponent<UILabel>().text = lab0.GetComponent<UILabel>().text;
            GetUI("Btn_ServerSelected_u").GetComponent<UIButton>().normalSprite = btn0.GetComponent<UIButton>().normalSprite;
            GetUI("Btn_ServerSelected_u").GetComponent<UIButton>().hoverSprite = btn0.GetComponent<UIButton>().hoverSprite;
            GetUI("Btn_ServerSelected_u").GetComponent<UIButton>().pressedSprite = btn0.GetComponent<UIButton>().pressedSprite;
            GetUI("Btn_ServerSelected_u").GetComponent<UIButton>().disabledSprite = btn0.GetComponent<UIButton>().disabledSprite;
            GetUI("Btn_ServerSelected_u").EnableUIButton();
        }
    }
    //服务器状态ui改变
    private void SetServerState(int index,string name,bool isfree)
    {
        string uiBtnName = "Btn_Server_" + ((index + 1) / 10).ToString() + ((index + 1) % 10).ToString() + "_u";
        string uiLabName = "Lab_Server_" + ((index + 1) / 10).ToString() + ((index + 1) % 10).ToString() + "_u";
        UIBehavior btn = GetUI(uiBtnName);
        UIBehavior lab = GetUI(uiLabName);
        btn.EnableUIButton();
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

    private const int SERVERCOUNT = 10;
    ///服务器标签点击事件注册【封装了一个方法】
    private void RegisterAllServerItems()
    {
        int count = SERVERCOUNT;
        for (int i = 0; i < count; i++)
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
    private bool isSignIn = false; //是否成功登录，影响”进入游戏“按钮
    public bool IsSignIn { get { return isSignIn; } }
    //登录成功进行的操作
    public void SignInSuccess()
    {
        //界面切换
        isSignIn = true;//已经验证成功
        string username = GetInputValue("Input_Username_Login_u");
        ClearLogin();
        ChangePanel("Pan_Login_u", "Pan_Start_u");
        GetUI("Lab_User_u").GetComponent<UILabel>().text = username;
        ShowMessagePanel("登录成功", 1f);
        //todo：加载玩家信息，从服务器获取角色信息
        GetComponent<RoleHandler>().GetRole();
    }
    //注册成功的操作
    public void RegisterSuccess()
    {
        //界面切换
        isSignIn = true;//已经验证成功
        string username = GetInputValue("Input_Username_SignUp_u");
        ClearSignUp();
        ChangePanel("Pan_SignUp_u", "Pan_Start_u");
        GetUI("Lab_User_u").GetComponent<UILabel>().text = username;
        ShowMessagePanel("登录成功",1f);
        //todo：加载玩家信息
        GetComponent<RoleHandler>().GetRole();
    }
    //手机号登录成功的回调
    public void PhoneSuccess()
    {
        //界面切换
        isSignIn = true;//已经验证成功
        GetUI("Lab_User_u").GetComponent<UILabel>().text = phone;
        GetUI("Input_Phone_u").GetComponent<UIInput>().value = "";
        GetUI("Input_Code_u").GetComponent<UIInput>().value = "";
        code = -1;
        phone = "";
        ChangePanel("Pan_Phone_u", "Pan_Start_u");
        ShowMessagePanel("登录成功", 1f);
        //todo：加载玩家信息，从服务器获取角色信息
        GetComponent<RoleHandler>().GetRole();
    }

    public void LoadingRole()
    {
        PlayerInfo player = GameControl.Instance.playerInfo;
        //获取角色模型创建点
        Transform createPos = GetUI("CharacterCreatePos_u").transform;
        //该用户没有角色的情况
        if (player.CharacterName == null)
        {
            GetUI("Lab_CharacterName_u").GetComponent<UILabel>().text = "";
            GetUI("Lab_Level_u").GetComponent<UILabel>().text = "";
            if (createPos.childCount != 0)
            {
                Destroy(createPos.GetChild(0).gameObject);
            }
            return;
        }
        //等级和角色名显示
        GetUI("Lab_CharacterName_u").GetComponent<UILabel>().text = player.CharacterName;
        GetUI("Lab_Level_u").GetComponent<UILabel>().text = "LV." + player.CharacterLevel;
        //模型创建
        if (createPos.childCount != 0)
        {
            Destroy(createPos.GetChild(0).gameObject);
        }
        GameObject go = NGUITools.AddChild(createPos.gameObject, characters[(int)GameControl.Instance.playerInfo.CharacterType]);
        go.layer = 8;
        go.transform.localScale = new Vector3(200, 200, 200);
        go.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
    }

    public void AddRoleSuccess()
    { 
        //面板切换
        OnCharacterNewReturnClick();
        //把模型重置
        ResetCharacterModel();
        //角色信息加载
        LoadingRole();
    }

    private void ResetCharacterModel()
    {
        string man = "Character_Man_u";
        string woman = "Character_Woman_u";
        GetUI(man).transform.DOScale(180, 0.2f);
        GetUI(woman).transform.DOScale(180, 0.2f);
    }

    //提示信息面板显示信息
    public void ShowMessagePanel(string message,float time)
    {
        UIBehavior start = GetUI("Pan_Message_u");
        start.GetComponent<UILabel>().text = message;
        start.ShowUI();
        StartCoroutine(HideObjectDelay(start, time));
    }
    private bool isGetServerList = false;
    public bool IsGetServerList { get { return isGetServerList; } }

    float phoneTimer = 60f;
    private void Update()
    {
        //获取服务器列表
        if (!isGetServerList) InitServerList();
        //验证码请求发送后60s倒计时
        if (isSendPhone)
        {
            if (phoneTimer >= 0)
            {
                GetUI("Lab_Send_u").GetComponent<UILabel>().text = ((int)phoneTimer).ToString() + " s";
                phoneTimer -= Time.deltaTime;
            }
            else
            {
                isSendPhone = false;
                phoneTimer = 60f;
                GetUI("Lab_Send_u").GetComponent<UILabel>().text = "发送验证码";
                GetUI("Btn_Send_u").EnableUIButton();
            }
        }
    }
    #endregion

}

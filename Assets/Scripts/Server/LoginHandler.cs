using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouCommon.ParamTools;

public class LoginHandler : ClientHandlerBase
{

    public override void Start()
    {
        code = OperationCode.Login;
        TaidouClient.Instance.RegistClientHandler(code, this);
    }

    public override void HandlerResponseMessage(OperationResponse response)
    {
        if (gameObject.tag == "01Start")
        {
            UIManager_01Start manager = (UIManager_01Start)GameControl.Instance.mUIManager;
            switch ((ReturnCode)response.ReturnCode)
            {
                case ReturnCode.Fail://登录失败显示信息
                    manager.ShowMessagePanel(response.DebugMessage, 1f);
                    break;
                case ReturnCode.Success://登录成功
                    manager.SignInSuccess();
                    break;
                case ReturnCode.PhoneSuccess://手机登录成功
                    manager.PhoneSuccess();
                    break;
                case ReturnCode.Exception://服务器端参数获取失败
                    manager.ShowMessagePanel("数据传输异常", 1f);
                    Debug.Log(response.DebugMessage);
                    break;
            }
        }
        else
        {
            switch ((ReturnCode)response.ReturnCode)
            {
                case ReturnCode.Fail://登录失败显示信息
                    Debug.Log("重新登录失败 : "+response.DebugMessage);
                    break;
                case ReturnCode.Success://登录成功
                    Debug.Log("重新登录成功");
                    break;
                case ReturnCode.Exception://服务器端参数获取失败
                    Debug.Log("重新登录异常 : " + response.DebugMessage);
                    break;
            }
        }
    }

    public void Login(string username,string password)
    {
        User user = new User() { username = username, password = password };
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.Login, user);
        TaidouClient.Instance.SendOperationRequest(code,para);
        GameControl.Instance.Username = username;
        GameControl.Instance.Password = password;
    }

    public void Login(string phone)
    {
        User user = new User() { username = phone, password = "0" };
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.PhoneLogin, user);
        TaidouClient.Instance.SendOperationRequest(code, para);
    }



    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }

   
}

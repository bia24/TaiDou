using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouCommon.ParamTools;

public class RegisterHandler : ClientHandlerBase
{
    public override void Start()
    {
        code = OperationCode.Register;
        TaidouClient.Instance.RegistClientHandler(code, this);
    }

    public override void HandlerResponseMessage(OperationResponse response)
    {
        UIManager_01Start manager = (UIManager_01Start)GameControl.Instance.mUIManager;
        switch ((ReturnCode)response.ReturnCode)
        {
            case ReturnCode.Success:
                manager.RegisterSuccess();
                break;
            case ReturnCode.Fail:
                manager.ShowMessagePanel(response.DebugMessage, 1f);
                break;
            case ReturnCode.Exception:
                manager.ShowMessagePanel("数据传输异常！", 1f);
                Debug.Log(response.DebugMessage);
                break;
        }
    }

    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }

    public void Register(string username,string password)
    {
        User user = new User() { username = username, password = password };
       Dictionary<byte,object>para= ParamTool.ConstructParam<User>(ParamCode.Register, user);
       TaidouClient.Instance.SendOperationRequest(code,para);
    }
}

using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;
using LitJson;
using TaidouCommon.Model;
using TaidouCommon.ParamTools;

public class GetServerHandler : ClientHandlerBase
{
     public override void Start()
    {
        code = OperationCode.GetServerList;
        TaidouClient.Instance.RegistClientHandler(code, this);
        TaidouClient.Instance.OnConnectToServer += GetServerList;
    }
    public override void HandlerResponseMessage(OperationResponse response)
    {
        if ((OperationCode)response.OperationCode != code)
        {
            Debug.Log("*********Wrong response code matach. reponse code: " + (OperationCode)response.OperationCode
                + " handler code: " + code + " *********");
            return;
        }
        else
        {
            List<Server> servers= ParamTool.GetParam<List<Server>>(ParamCode.ServerList, response.Parameters);
            GameControl.Instance.serverList = servers;//刷新服务器列表
        }
    }
    //向服务器发起请求的方法
    public void GetServerList()
    {
        TaidouClient.Instance.SendOperationRequest(code, new Dictionary<byte, object>());
    }

    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
        TaidouClient.Instance.OnConnectToServer -= GetServerList;
    }
}

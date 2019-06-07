using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouCommon.ParamTools;

public class SyncPlayerHandler : ClientHandlerBase
{

    public override void Start()
    {
        code = OperationCode.Sync;
        TaidouClient.Instance.RegistClientHandler(code, this);
    }

    public override void HandlerResponseMessage(OperationResponse response)
    {
        ReturnCode returnCode =(ReturnCode) response.ReturnCode;
        switch (returnCode)
        {
            case ReturnCode.Success:
                break;
            case ReturnCode.Fail:
                Debug.LogError(response.DebugMessage);
                break;
            case ReturnCode.Exception:
                Debug.LogError(response.DebugMessage);
                break;
        }
    }

    public void SycnToServer(SynPlayer players)
    {
        Dictionary<byte,object>para= ParamTool.ConstructParam(ParamCode.SysnPlayer, players);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Sync, para);
    }

    public void SycnToServer(AmModel model)
    {
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.SysnAnimation, model);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Sync, para);
    }


    public override void HandlerEvent(EventData eventData)
    {
        if (eventData.Parameters.ContainsKey((byte)ParamCode.SysnPlayer))
        {//同步玩家位置和旋转
            List<SynPlayer> serverPlayers = ParamTool.GetParam<List<SynPlayer>>(ParamCode.SysnPlayer, eventData.Parameters);
            if (serverPlayers == null)
            {
                Debug.LogError("服务器传回players实时信息解析失败");
                return;
            }
            else
            {
                RaidManager.Instance.SyncForLocalPlayerData(serverPlayers);
            }
        }
        else if(eventData.Parameters.ContainsKey((byte)ParamCode.SysnAnimation))
        {//同步玩家的动画操作
            AmModel model = ParamTool.GetParam<AmModel>(ParamCode.SysnAnimation, eventData.Parameters);
            if (model == null)
            {
                Debug.LogError("服务器传回的animation实时数据解析失败");
                return;
            }
            else
            {
                RaidManager.Instance.SyncAnimationToLocal(model);
            }
        }
    }







    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }

  
}

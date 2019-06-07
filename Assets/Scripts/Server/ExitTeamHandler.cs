using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;

public class ExitTeamHandler : ClientHandlerBase
{
    private void Awake()
    {
        code = OperationCode.ExitTeam;
        TaidouClient.Instance.RegistClientHandler(code, this);
    }
    public override void Start()
    {
    }

    public override void HandlerResponseMessage(OperationResponse response)
    {
        ReturnCode returnCode = (ReturnCode)response.ReturnCode;
        switch (returnCode)
        {
            case ReturnCode.EndGame:
                Debug.Log("队伍解散成功");
                break;
            case ReturnCode.Fail:
                Debug.Log("解散队伍"+response.DebugMessage);
                break;
            case ReturnCode.Exception:
                Debug.Log(response.DebugMessage);
                break;
        }
    }

    public void SendExitTeamRequest()
    {
        Dictionary<byte, object> para = new Dictionary<byte, object>();
        para.Add((byte)ParamCode.EndGame, "endgame");
        TaidouClient.Instance.SendOperationRequest(OperationCode.ExitTeam, para);
    }

    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }

}

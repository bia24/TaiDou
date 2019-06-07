using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;
using TaidouCommon.ParamTools;
using TaidouCommon.Model;
using System.Text;

public class TeamHandler : ClientHandlerBase
{
    private UIManager_02NoviceVillage ui;

    public override void Start()
    {
        code = OperationCode.Team;
        TaidouClient.Instance.RegistClientHandler(code, this);
        ui = (UIManager_02NoviceVillage)GameControl.Instance.mUIManager;
    }
    public override void HandlerResponseMessage(OperationResponse response)
    {
        ReturnCode returnCode = (ReturnCode)response.ReturnCode;
        switch (returnCode)
        {
            case ReturnCode.WaitTeam:
                Debug.Log("等待组队中...");
                ui.ShowMessage("等待组队中...");
                break;
            case ReturnCode.CreateTeamSuccess:
                Debug.Log("队伍创建成功");
                ui.ShowMessage("队伍创建成功");
                break;
            case ReturnCode.CancelTeamSuccess:
                Debug.Log("取消队伍成功");
                ui.ShowMessage("取消队伍成功");
                break;
            case ReturnCode.Exception:
                Debug.Log("组队请求服务器端解析异常");
                ui.ShowMessage("组队请求服务器端解析异常");
                break;
        }
    }

    public override void HandlerEvent(EventData eventData)
    {
        List<string> playerNames=ParamTool.GetParam<List<string>>(ParamCode.TeamName, eventData.Parameters);
        List<bool> playerSexes = ParamTool.GetParam<List<bool>>(ParamCode.TeamSex, eventData.Parameters);
        if (playerNames!=null&&playerSexes!=null)
        {
            GameControl.Instance.playerNames = playerNames;
            GameControl.Instance.playerSexes = playerSexes;
            Debug.Log("队友名称传回接收成功");
            //更改ui显示 btn可以按
            ui.OnTeamSuccess(GameControl.Instance.playerNames,GameControl.Instance.playerSexes);
            //提示信息显示
            if (GameControl.Instance.playerNames[0] != GameControl.Instance.playerInfo.CharacterName)
            {
                ui.ShowMessage("队伍创建成功");
            }
        }
        else
        {
            Debug.Log("队友名称传回参数解析失败");
        }
    }


    public void SendTeamRequest(int raidID)
    {
        Dictionary<byte, object> para = new Dictionary<byte, object>();
        para.Add((byte)ParamCode.GetTeam, raidID);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Team, para);
    }

    public void CancelTeamRequest()
    {
        Dictionary<byte, object> para = new Dictionary<byte, object>();
        para.Add((byte)ParamCode.CancelTeam, "cancle");
        TaidouClient.Instance.SendOperationRequest(OperationCode.Team, para);
    }


    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }

    
}

using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouCommon.ParamTools;
using LitJson;
using System.Text.RegularExpressions;

public class RoleHandler : ClientHandlerBase
{

    public override void Start()
    {
        code = OperationCode.Role;
        TaidouClient.Instance.RegistClientHandler(code,this);
    }

    public override void HandlerResponseMessage(OperationResponse response)
    {
        if (gameObject.tag == "01Start")
        {
            UIManager_01Start manager = (UIManager_01Start)GameControl.Instance.mUIManager;
            ReturnCode returnCode = (ReturnCode)response.ReturnCode;
            Role role = null;
            switch (returnCode)
            {
                case ReturnCode.Exception:
                    manager.ShowMessagePanel("角色信息异常", 1f);
                    break;
                case ReturnCode.EmptyRole:
                    //若查无角色信息，将当前player的nikname=null，用来做空角色的标志
                    GameControl.Instance.playerInfo.CharacterName = null;
                    //加载ui
                    manager.LoadingRole();
                    break;
                case ReturnCode.HasRole:
                    role = ParamTool.GetParam<Role>(ParamCode.Role, response.Parameters);
                    //为GameControl的player信息赋值
                    UpdatePlayerInfo(role);
                    //加载ui显示角色信息
                    manager.LoadingRole();
                    break;
                case ReturnCode.Fail:
                    manager.ShowMessagePanel(response.DebugMessage, 1f);
                    break;
                case ReturnCode.Success://添加角色成功
                    role = ParamTool.GetParam<Role>(ParamCode.Role, response.Parameters);
                    //为GameControl的player信息赋值
                    UpdatePlayerInfo(role);
                    //调用成功创建的方法
                    manager.AddRoleSuccess();
                    break;
            }
        }
        else if (gameObject.tag == "02NoviceVillage")
        {
            UIManager_02NoviceVillage manager = (UIManager_02NoviceVillage)GameControl.Instance.mUIManager;
            ReturnCode returnCode = (ReturnCode)response.ReturnCode;
            switch (returnCode)
            {
                case ReturnCode.Exception:
                    Debug.Log(response.DebugMessage);
                    break;
                case ReturnCode.UpdateRoleSuccess:
                    //更新角色信息成功
                    Debug.Log("角色更新成功");
                    break;
            }
        }
        else
        {
            ReturnCode returnCode = (ReturnCode)response.ReturnCode;
            switch (returnCode)
            {
                case ReturnCode.Exception:
                    Debug.Log(response.DebugMessage);
                    break;
                case ReturnCode.UpdateRoleSuccess:
                    //更新角色信息成功
                    Debug.Log("角色更新成功");
                    break;
            }
        }
    }

    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }

    //当前必须已经登录验证，客户端通过ui类中bool进行过滤
    //登录成功后，服务器端会保留有user的引用
    public void GetRole()
    {
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.Role, SubOpCode.GetRole);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Role, para);
    }
    //增加角色
    public void AddRole(Role role)
    {
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.Role, SubOpCode.AddRole);
        string json = Regex.Unescape(JsonMapper.ToJson(role));
        para.Add((byte)ParamCode.AddRole, json);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Role, para);
    }
    //向服务器端发送更新role角色请求
    public void UpdateRole(PlayerInfo player)
    {
        Role role = new Role();
        role.id = 0;
        role.user = null;
        role.name = player.CharacterName;
        role.isman = (player.CharacterType == CharacterType.Man);
        role.level = player.CharacterLevel;
        role.power = player.CharacterPower;
        role.exp = player.CharacterExp;
        role.diamond = player.Diamond;
        role.gold = player.Gold;
        role.physical = player.Physical;
        role.energy = player.Energy;
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.Role, SubOpCode.UpdateRole);
        string json = Regex.Unescape(JsonMapper.ToJson(role));
        para.Add((byte)ParamCode.UpdateRole, json);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Role,para);
    }

    private void UpdatePlayerInfo(Role role)
    {
        PlayerInfo player = GameControl.Instance.playerInfo;
        player.CharacterName = role.name;
        player.CharacterType = role.isman ? CharacterType.Man : CharacterType.Woman;
        player.CharacterLevel = role.level;
        player.CharacterPower = role.power;
        player.CharacterExp = role.exp;
        player.Diamond = role.diamond;
        player.Gold = role.gold;
        player.Physical = role.physical;
        player.Energy = role.energy;
    }
}
    


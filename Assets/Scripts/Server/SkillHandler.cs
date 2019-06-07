using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using TaidouCommon;
using UnityEngine;
using TaidouCommon.ParamTools;
using TaidouCommon.Model;
using UnityEngine.SceneManagement;


public class SkillHandler : ClientHandlerBase
{
    public override void Start()
    {
        code = OperationCode.Skill;
        TaidouClient.Instance.RegistClientHandler(code, this);
    }

    public override void HandlerResponseMessage(OperationResponse response)
    {
        switch ((ReturnCode)response.ReturnCode)
        {
            case ReturnCode.EmptySkill:
                //空背包的情况，向数据库中添加默认技能等级
                Debug.Log("数据库中没有skill，进行添加默认skill列表");
                AddSkill();
                break;
            case ReturnCode.HasSkill:
                //数据库中已经拥有skill，给赋值
                Debug.Log("取到了skill等级，进行本地化修改");
                ModifySkillLevel(response);
                break;
            case ReturnCode.Success:
                Debug.Log("添加技能/更新技能至数据库成功");
                break;
            case ReturnCode.Fail:
                Debug.Log(response.DebugMessage);
                break;
            case ReturnCode.Exception:
                Debug.Log(response.DebugMessage);
                break;
        }
    }



    //获取数据库中的skill
    public void GetSkill()
    {
        Dictionary<byte, object> para = new Dictionary<byte, object>();
        para.Add((byte)ParamCode.GetSkill, null);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Skill, para);
    }
    //用于初始化数据库中的skill项
    public void AddSkill()
    {
        List<TaidouCommon.Model.Skill> skills = new List<TaidouCommon.Model.Skill>();
        for (int i = 1; i <= 3; i++)
        {
            TaidouCommon.Model.Skill skill = new TaidouCommon.Model.Skill();
            skill.id = 0;
            skill.level = 1;
            skill.role = null;
            skill.pos = i;
            skills.Add(skill);
        }
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.AddSkill, skills);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Skill, para);
    }
    //用于更新数据库中的skill等级
    public void UpdateSkill(PosType posType, int level)
    {
        int pos = 0;
        switch (posType)
        {
            case PosType.One:
                pos = 1;
                break;
            case PosType.Two:
                pos = 2;
                break;
            case PosType.Three:
                pos = 3;
                break;
        }
        TaidouCommon.Model.Skill skill = new TaidouCommon.Model.Skill();
        skill.id = 0;
        skill.level = level;
        skill.role = null;
        skill.pos = pos;
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.UpdateSkill, skill);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Skill, para);
    }
    //修改本地的技能等级
    private void ModifySkillLevel(OperationResponse response)
    {
        List<TaidouCommon.Model.Skill> skills = ParamTool.GetParam<List<TaidouCommon.Model.Skill>>(ParamCode.GetSkill, response.Parameters);
        foreach (var s in skills)
        {
            PosType pos = PosType.One;
            switch (s.pos)
            {
                case 1:
                    pos = PosType.One;
                    break;
                case 2:
                    pos = PosType.Two;
                    break;
                case 3:
                    pos = PosType.Three;
                    break;
            }
            GameControl.Instance.skillManager.GetSkill(GameControl.Instance.playerInfo.CharacterType, pos).Level = s.level;
        }
    }

    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }


}

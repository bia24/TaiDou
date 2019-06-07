using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouServer.DataBase.Utils;
using TaidouCommon.ParamTools;

namespace TaidouServer.Handler
{
    class SkillHandler:HandlerBase
    {
        SkillUtils skillUtil;
        RoleUtil roleUtil;
        public SkillHandler()
        {
            skillUtil = new SkillUtils();
            roleUtil = new RoleUtil();
        }

        public override OperationResponse HandlerRequestMessage(OperationRequest request, TaidouPeer peer)
        {
            OperationResponse res = new OperationResponse();
            res.OperationCode = request.OperationCode;
            Dictionary<byte, object> para = request.Parameters;
            Role role = roleUtil.GetRoleByUser(peer.user)[0];
            if (para.ContainsKey((byte)ParamCode.GetSkill))
            {//获取操作
                IList<Skill> skillsDB= skillUtil.GetSkillByRole(role);
                if (skillsDB.Count == 0)//不存在skill
                {
                    res.ReturnCode = (short)ReturnCode.EmptySkill;
                    return res;
                }
                else
                {
                    List<Skill> skills=(List<Skill>)skillsDB;
                    foreach(var t in skills){
                        t.role=null;
                    }
                    Dictionary<byte, object> param = ParamTool.ConstructParam(ParamCode.GetSkill, skills);
                    res.Parameters = param;
                    res.ReturnCode = (short)ReturnCode.HasSkill;
                    return res;
                }
            }
            else if(para.ContainsKey((byte)ParamCode.AddSkill)){
                List<Skill> skills = ParamTool.GetParam<List<Skill>>(ParamCode.AddSkill, para);
                foreach (var t in skills)
                {
                    t.role = role;
                    skillUtil.AddSkill(t);
                }
                res.ReturnCode = (short)ReturnCode.Success;
                return res;
            }
            else if (para.ContainsKey((byte)ParamCode.UpdateSkill))
            {
                Skill s = ParamTool.GetParam<Skill>(ParamCode.UpdateSkill, para);
                IList<Skill> sks= skillUtil.GetOneSkill(role, s.pos);
                if (sks.Count == 0)
                {
                    res.ReturnCode = (short)ReturnCode.Fail;
                    res.DebugMessage = "找不到对应的skill";
                    return res;
                }
                Skill dbs = sks[0];
                //更新等级
                dbs.level = s.level;
                skillUtil.UpdateSkill(dbs);
                res.ReturnCode = (short)ReturnCode.Success;
                return res;
            }
            else
            {
                res.ReturnCode = (short)ReturnCode.Exception;
                res.DebugMessage = "无法处理的情况";
                return res;
            }
        }
    }
}

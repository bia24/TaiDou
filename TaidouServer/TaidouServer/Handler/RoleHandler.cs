using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon.ParamTools;
using TaidouServer.DataBase.Utils;
using TaidouCommon;
using TaidouCommon.Model;

namespace TaidouServer.Handler
{
    class RoleHandler:HandlerBase
    {
        public RoleUtil util;
        public RoleHandler()
        {
            util = new RoleUtil();
        }
        public override OperationResponse HandlerRequestMessage(OperationRequest request,TaidouPeer peer)
        {
            OperationResponse response = new OperationResponse();
            response.OperationCode = request.OperationCode;
            SubOpCode subCode= ParamTool.GetParam<SubOpCode>(ParamCode.Role, request.Parameters);
            if (subCode == SubOpCode.Exception)
            {
                response.ReturnCode = (short)ReturnCode.Exception;
                response.DebugMessage = "不存在SubOpCode，异常错误";
                return response;
            }
            switch(subCode){
                case SubOpCode.GetRole:
                    TaidouApplication.log.Debug("*********获取角色操作");
                    Role role = GetRole(peer);
                    if (role == null)
                    {
                        response.ReturnCode = (short)ReturnCode.EmptyRole;
                    }
                    else
                    {
                        response.ReturnCode = (short)ReturnCode.HasRole;
                        response.Parameters = ParamTool.ConstructParam(ParamCode.Role, role);
                    }
                    break;
                case SubOpCode.AddRole:
                    TaidouApplication.log.Debug("*********增加角色操作");
                    Role roleAdd = ParamTool.GetParam<Role>(ParamCode.AddRole, request.Parameters);
                    if (roleAdd == null)
                    {
                        response.ReturnCode = (short)ReturnCode.Exception;
                        response.DebugMessage = "需要添加的role 解析错误，异常";
                        return response;
                    }
                    if (AddRole(peer, roleAdd))
                    {
                        response.Parameters = ParamTool.ConstructParam(ParamCode.Role, roleAdd);
                        response.ReturnCode = (short)ReturnCode.Success;
                    }
                    else
                    {
                        response.ReturnCode = (short)ReturnCode.Fail;
                        response.DebugMessage = "角色用户名已存在";
                    }
                    break;
                case SubOpCode.UpdateRole:
                    TaidouApplication.log.Debug("*********更新角色数值");
                    Role roleUp = ParamTool.GetParam<Role>(ParamCode.UpdateRole, request.Parameters);
                    if (roleUp == null)
                    {
                        response.ReturnCode = (short)ReturnCode.Exception;
                        response.DebugMessage = "需要更新的role 解析错误，异常";
                        return response;
                    }
                    UpdateRole(peer,roleUp);
                    response.ReturnCode = (short)ReturnCode.UpdateRoleSuccess;
                    break;
            }
            return response;
        }

        /// <summary>
        /// 返回的Role里没有user信息
        /// </summary>
        /// <param name="peer"></param>
        /// <returns></returns>
        private Role GetRole(TaidouPeer peer)
        {
            User user = peer.user;
            IList<Role> roles= util.GetRoleByUser(user);
            if (roles.Count == 0)
            {
                return null;
            }
            else
            {
                Role role = roles[0];
                role.user = null;
                return role;
            }
        }

        private bool AddRole(TaidouPeer peer,Role role)
        {
            //角色名唯一性验证
            IList<Role> roles = util.GetRoleByName(role.name);
            if (roles.Count != 0)
            {
                TaidouApplication.log.Debug("*******角色名字重复");
                return false;
            }

            User user = peer.user;
            roles = util.GetRoleByUser(user);
            if (roles.Count == 0)
            {
               //该用户没有创建角色
                role.user = user;
                util.AddRole(role);
            }
            else
            {
                //更新用户信息
                //Role roleDb = roles[0];
                //role.id = roleDb.id;
                //role.user = user;
                //util.UpdateRole(role);
                
                /*更新方法，删除原角色信息，新建一个role*/
                Role roleDb = roles[0];
                role.id = roleDb.id;
                role.user = user;
                util.Delete(role);
                role.id=0;
                util.AddRole(role);
            }
            return true;
        }

        //不删除原role，在原role上更新role的相关信息
        //用在游戏内部，例如更新数值或者金币等
        private void UpdateRole(TaidouPeer peer, Role role)
        {
            User user = peer.user;
            IList<Role> roles = util.GetRoleByUser(user);
            //更新db中的role
            Role roleDB = roles[0];
            role.id = roleDB.id;
            role.user = user;
            util.UpdateRole(role);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouServer.DataBase.Utils;
using TaidouCommon.ParamTools;
using TaidouCommon.Model;
using TaidouCommon;
using TaidouServer.Tools;

namespace TaidouServer.Handler
{
    class RegisterHandler:HandlerBase
    {
        private UserUtil userUtil = null;
        public RegisterHandler()
        {
            userUtil = new UserUtil();
        }
        public override OperationResponse HandlerRequestMessage(OperationRequest request,TaidouPeer peer)
        {
            OperationResponse response = new OperationResponse();
            response.OperationCode = request.OperationCode;
            User user = ParamTool.GetParam<User>(ParamCode.Register, request.Parameters);
            if (user!=null)
            {
                IList<User> userDb = userUtil.GetUserByUsername(user.username);
                if (userDb.Count != 0)
                {
                    response.ReturnCode = (short)ReturnCode.Fail;
                    response.DebugMessage = "用户名已存在.";
                }
                else
                {
                    user.password = MD5Tool.GetMD5(user.password);
                    user.id=userUtil.AddUser(user);//添加用户
                    response.ReturnCode = (short)ReturnCode.Success;
                    peer.user = user;//完成验证的用户进行赋值
                }
            }
            else{
                response.ReturnCode = (short)ReturnCode.Exception;
                response.DebugMessage = "请求参数代码 " + ParamCode.Register + " 无法获取到参数";
            }
            return response;
        }
    }
}

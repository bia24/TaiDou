using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon.Model;
using TaidouCommon;
using LitJson;
using TaidouServer.DataBase.Utils;
using TaidouServer.Tools;
using TaidouCommon.ParamTools;

namespace TaidouServer.Handler
{
    class LoginHandler : HandlerBase
    {
        UserUtil userUtil = null;
        public LoginHandler()
        {
            userUtil = new UserUtil();
        }
        public override OperationResponse HandlerRequestMessage(OperationRequest request,TaidouPeer peer)
        {
            OperationResponse response = new OperationResponse();
            response.OperationCode = request.OperationCode;
            User user = ParamTool.GetParam<User>(ParamCode.Login,request.Parameters);
            if(user!=null)
            {
                IList<User> userDb= userUtil.GetUserByUsername(user.username);
                
                if (userDb.Count==0)
                {
                    response.ReturnCode = (short)ReturnCode.Fail;
                    response.DebugMessage = "用户名或密码错误";
                }
                else
                {
                    if (MD5Tool.GetMD5(user.password) != userDb[0].password)
                    {
                        response.ReturnCode = (short)ReturnCode.Fail;
                        response.DebugMessage = "用户名或密码错误";
                    }
                    else
                    {
                        response.ReturnCode = (short)ReturnCode.Success;
                        peer.user = userDb[0];//给完成验证的用户赋值
                    }
                }
            }
            else
            {
                response.ReturnCode = (short)ReturnCode.Exception;
                response.DebugMessage="请求参数代码 "+ParamCode.Login+" 无法获取到参数";
            }
            return response;
        }
    }
}

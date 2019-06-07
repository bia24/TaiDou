using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouServer.DataBase.Utils;
using TaidouCommon.Model;
using TaidouCommon;
using TaidouCommon.ParamTools;

namespace TaidouServer.Handler
{
    class ServerHandler:HandlerBase
    {
        private ServerUtil util;
        public ServerHandler()
        {
            util = new ServerUtil();
        }
        public override OperationResponse HandlerRequestMessage(OperationRequest request,TaidouPeer peer)
        {
            OperationResponse response = new OperationResponse();
            response.OperationCode = request.OperationCode;
            IList<Server> serverList =util.GetAllServer();
            Dictionary<byte, object> responseParam = ParamTool.ConstructParam(ParamCode.ServerList, serverList);
            response.Parameters = responseParam;
            return response;
        }
    }
}

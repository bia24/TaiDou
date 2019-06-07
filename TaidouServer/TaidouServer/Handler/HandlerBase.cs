using ExitGames.Logging;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TaidouServer.Handler
{
    public abstract class HandlerBase
    {
        public abstract OperationResponse HandlerRequestMessage(OperationRequest request,TaidouPeer peer);
    }
}

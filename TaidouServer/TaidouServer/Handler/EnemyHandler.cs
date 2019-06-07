using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon;

namespace TaidouServer.Handler
{
    class EnemyHandler:HandlerBase
    {
        public Dictionary<string, List<TaidouPeer>> peerTeams = null;
        public EnemyHandler()
        {
            peerTeams = TaidouApplication.Instance.peerTeams;
        }
        public override OperationResponse HandlerRequestMessage(OperationRequest request, TaidouPeer peer)
        {
            OperationResponse res = new OperationResponse();
            res.OperationCode = request.OperationCode;
            if (request.Parameters.ContainsKey((byte)ParamCode.EnemyCreate))
            {
                BroadcastEnemy(request.Parameters, peer);
                res.ReturnCode = (short)ReturnCode.Success;
            }
            else if (request.Parameters.ContainsKey((byte)ParamCode.EnemyMove))
            {
                BroadcastEnemy(request.Parameters, peer);
                res.ReturnCode = (short)ReturnCode.Success;
            }
            else if (request.Parameters.ContainsKey((byte)ParamCode.EnemyAm))
            {
                BroadcastEnemy(request.Parameters, peer);
                res.ReturnCode = (short)ReturnCode.Success;
            }
            else if (request.Parameters.ContainsKey((byte)ParamCode.EnemyTakeDamage))
            {
                BroadcastEnemy(request.Parameters, peer);
                res.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                res.ReturnCode = (short)ReturnCode.Exception;
                res.DebugMessage = "敌人同步：未知操作";
            }
            return res;
        }

        private void BroadcastEnemy(Dictionary<byte,object>para,TaidouPeer peer)
        {
            EventData data = new EventData();
            data.Code = (byte)OperationCode.EnemySync;
            data.Parameters = para;
            foreach(var temp in peerTeams[peer.leaderName])
            {
                if (temp != peer)//非本peer对应的客户端
                {
                    if (temp.Connected)
                    {
                        temp.SendEvent(data, new SendParameters());
                    }
                }   
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon;
using TaidouCommon.Model;

namespace TaidouServer.Handler
{
    class ExitTeamHandler:HandlerBase
    {
        public override OperationResponse HandlerRequestMessage(OperationRequest request, TaidouPeer peer)
        {
            OperationResponse res = new OperationResponse();
            res.OperationCode = request.OperationCode;
            if(request.Parameters.ContainsKey((byte)ParamCode.EndGame)){
                TaidouApplication.log.Debug("get endgame:" + peer.leaderName);
                //游戏结束的请求
                if (peer.leaderName != null)
                {
                    Dictionary<string, List<TaidouPeer>> peerTeams = TaidouApplication.Instance.peerTeams;
                    if (peerTeams.ContainsKey(peer.leaderName))
                    {
                        peerTeams.Remove(peer.leaderName);
                        TaidouApplication.log.Debug("End Game,remove peerTeams:"+peer.leaderName);
                    }
                    Dictionary<string, List<SynPlayer>> serverPlayers = TaidouApplication.Instance.serverPlayers;
                    if (serverPlayers.ContainsKey(peer.leaderName))
                    {
                        serverPlayers.Remove(peer.leaderName);
                        TaidouApplication.log.Debug("End Game,remove SynPlayer:" + peer.leaderName);
                    }
                    res.ReturnCode = (short)ReturnCode.EndGame;
                }
                else
                {
                    TaidouApplication.log.Debug("End Game,null leaderName");
                    res.DebugMessage = "leaderName为空,不存在队伍:"+peer.leaderName;
                    res.ReturnCode = (short)ReturnCode.Fail;
                }
                return res;
            }
            else{
                res.DebugMessage = "EndGame请求参数错误";
                res.ReturnCode=(short)ReturnCode.Exception;
                return res;
            }
        }
    }
}

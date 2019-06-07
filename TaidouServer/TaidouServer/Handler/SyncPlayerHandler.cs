using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon.Model;
using TaidouCommon.ParamTools;
using TaidouCommon;

namespace TaidouServer.Handler
{
    class SyncPlayerHandler:HandlerBase
    {
        public Dictionary<string, List<SynPlayer>> serverPlayers = null;
        public Dictionary<string, List<TaidouPeer>> peerTeams = null;
        public SyncPlayerHandler()
        {
            serverPlayers = TaidouApplication.Instance.serverPlayers;
            peerTeams = TaidouApplication.Instance.peerTeams;
        }
        public override OperationResponse HandlerRequestMessage(OperationRequest request, TaidouPeer peer)
        {
            OperationResponse response=new OperationResponse();
            response.OperationCode=request.OperationCode;
            if (request.Parameters.ContainsKey((byte)ParamCode.SysnPlayer))
            {
                //取出玩家数据
                SynPlayer remotePlayerDate = ParamTool.GetParam<SynPlayer>(ParamCode.SysnPlayer, request.Parameters);
                if (remotePlayerDate != null)
                {
                    string masterName = peer.leaderName;
                    if (!serverPlayers.ContainsKey(masterName))
                    {
                        serverPlayers.Add(masterName, new List<SynPlayer>());//初始化
                    }
                    //将远程数据与数据库中的player数据进行同步
                    SyncPlayerData(serverPlayers[masterName], remotePlayerDate);
                    response.DebugMessage = "服务器player数据同步成功";
                    response.ReturnCode = (short)ReturnCode.Success;
                    //向所有玩家广播当前各个玩家的状态
                    SendServerPlayerDataToClient(peer);
                }
                else
                {
                    response.DebugMessage = "服务器player数据解析失败";
                    response.ReturnCode = (short)ReturnCode.Fail;
                }
                return response;
            }
            else if (request.Parameters.ContainsKey((byte)ParamCode.SysnAnimation))
            {
                //向所有玩家广播某客户端的动画操作
                BroadcastAnimation(request.Parameters,peer);
                response.DebugMessage = "服务器广播动画操作成功";
                response.ReturnCode = (short)ReturnCode.Success;
                return response;
            }
            else
            {
                response.DebugMessage = "sync 未知操作";
                response.ReturnCode = (short)ReturnCode.Exception;
                return response;
            }
        }

        private void SyncPlayerData(List<SynPlayer> serverData, SynPlayer remoteData)
        {
            bool isExist=false;
            foreach (var temp in serverData)
            {
                if (temp.Name == remoteData.Name)
                {
                    isExist = true;
                    //更新位置和旋转信息
                    temp.Position = remoteData.Position;
                    temp.Euler = remoteData.Euler;
                }
            }
            if (!isExist)
            {
                SynPlayer player = new SynPlayer();
                //更新新加入的player的相关信息
                player.Name = remoteData.Name;
                player.isConnect = true;
                player.Position = remoteData.Position;
                player.Euler = remoteData.Euler;
                serverData.Add(player);
            }
        }

        private void SendServerPlayerDataToClient(TaidouPeer peer)
        {
            if (!serverPlayers.ContainsKey(peer.leaderName)) return;
            if (!peerTeams.ContainsKey(peer.leaderName)) return;
            List<SynPlayer> playerData = serverPlayers[peer.leaderName];
            Dictionary<byte,object>para=ParamTool.ConstructParam(ParamCode.SysnPlayer,playerData);
            EventData data=new EventData();
            data.Code=(byte)OperationCode.Sync;
            data.Parameters=para;
            foreach (var temp in peerTeams[peer.leaderName])
            {
                if (temp.Connected)
                {
                    temp.SendEvent(data, new SendParameters());
                }
            }
        }
        private void BroadcastAnimation(Dictionary<byte,object>para,TaidouPeer peer)
        {
            if (!serverPlayers.ContainsKey(peer.leaderName)) return;
            if (!peerTeams.ContainsKey(peer.leaderName)) return;
            EventData data = new EventData();
            data.Code = (byte)OperationCode.Sync;
            data.Parameters = para;
            foreach (var temp in peerTeams[peer.leaderName])
            {
                if (temp.Connected)
                {
                    temp.SendEvent(data, new SendParameters());
                }
            }
        }

    }
}

using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon;
using TaidouServer.Handler;
using TaidouCommon.Model;
using TaidouServer.DataBase.Utils;

namespace TaidouServer
{
    public class TaidouPeer : PeerBase
    {
        //用来存储已经完成验证的用户账号
        public User user = null;
        //玩家选择的团队副本主机角色名字
        public string leaderName = null;
        //玩家选择的团队副本id
        public int raidID = 0;
        
        public TaidouPeer(InitRequest initRequest)
            : base(initRequest)
        {
            TaidouApplication.log.Debug("========Create a new ServerPeer!========");
        }

        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            RoleUtil util=new RoleUtil();
            IList<Role> roles = util.GetRoleByUser(user);
            Role role = null;
            if (roles.Count == 0)
            {
                return;
            }
            else
            {
                role = roles[0];
            }
            //组队中断线，将peer从组队队列中移除
            if (TaidouApplication.Instance.raidTeam.ContainsKey(raidID))
            {
                if(TaidouApplication.Instance.raidTeam[raidID].Contains(this)){
                    List<TaidouPeer> temp = new List<TaidouPeer>(TaidouApplication.Instance.raidTeam[raidID].ToArray<TaidouPeer>());
                    temp.Remove(this);
                    TaidouApplication.Instance.raidTeam[raidID].Clear();
                    foreach (var p in temp)
                    {
                        TaidouApplication.Instance.raidTeam[raidID].Enqueue(p);
                    }
                    TaidouApplication.log.Debug("OnDisconnect to delete one peer from raidTeam: rolename: " + role.name + ",raidID: " + this.raidID);
                }
            }
            if (leaderName == null) return;
            //游戏中掉线，peer字典中的对象移除
            Dictionary<string, List<TaidouPeer>> peerTeams = TaidouApplication.Instance.peerTeams;
            if (peerTeams.ContainsKey(leaderName))
            {
                if (peerTeams[leaderName].Contains(this))
                {
                    peerTeams[leaderName].Remove(this);
                        
                    //因为peer副本队伍每次建立都会new出来所以没有必要清空
                }
            }
            //游戏中掉线，同步玩家字典中的对象在线状态更改为false
            Dictionary<string, List<SynPlayer>> serverPlayers = TaidouApplication.Instance.serverPlayers;
            if (serverPlayers.ContainsKey(leaderName))
            {
                foreach (var temp in serverPlayers[leaderName])
                {
                    if (temp.Name == role.name)
                    {
                        temp.isConnect = false;
                        TaidouApplication.log.Debug("rolename: " + role.name+" 玩家下线了");
                    }
                }
                //如果玩家同步列表中的对象都不在线了，重置该列表
                bool isClear = true;
                foreach (var temp in serverPlayers[leaderName])
                {
                    if (temp.isConnect == true)
                    {
                        isClear = false;
                    }
                }
                if (isClear)
                {
                    serverPlayers[leaderName].Clear();
                    TaidouApplication.log.Debug(leaderName + " 的副本中已经没有队员了，本副本serverPlayers清空");
                }
            }

        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            OperationCode code = (OperationCode)operationRequest.OperationCode;
            HandlerBase handle = TaidouApplication.Instance.GetHandler(code);
            OperationResponse response = handle.HandlerRequestMessage(operationRequest,this);
            SendOperationResponse(response, sendParameters);
            if (code == OperationCode.Sync || code == OperationCode.EnemySync) return;
            else
            {
                TaidouApplication.log.Debug("Receive a request: "+code.ToString());
            }
        }

    }
}

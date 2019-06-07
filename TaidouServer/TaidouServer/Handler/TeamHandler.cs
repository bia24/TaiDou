using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon;
using TaidouServer.DataBase.Utils;
using TaidouCommon.ParamTools;
using LitJson;
using System.Text.RegularExpressions;

namespace TaidouServer.Handler
{
    class TeamHandler:HandlerBase
    {
        RoleUtil roleUtil;
        Dictionary<int, Queue<TaidouPeer>> raidTeam;
        Dictionary<string, List<TaidouPeer>> peerTeams;
        public TeamHandler()
        {
            roleUtil = new RoleUtil();
            raidTeam = TaidouApplication.Instance.raidTeam;
            peerTeams = TaidouApplication.Instance.peerTeams;
        }
        public override OperationResponse HandlerRequestMessage(OperationRequest request, TaidouPeer peer)
        {
            OperationResponse response = new OperationResponse();
            response.OperationCode = request.OperationCode;
            if (request.Parameters.ContainsKey((byte)ParamCode.GetTeam))
            {//组队请求
                int raidID = (int)request.Parameters[(byte)ParamCode.GetTeam];
                //给peer赋值raidID
                peer.raidID = raidID;
                if (raidTeam.ContainsKey(raidID))
                {//副本id=raidid的副本已存在队列
                    if (raidTeam[raidID].Count < TaidouApplication.RAIDNEEDPLAYER - 1)
                    {//人数不够，继续添加
                        raidTeam[raidID].Enqueue(peer);
                        response.ReturnCode = (short)ReturnCode.WaitTeam;
                    }
                    else
                    {//人数够了，将元素取出，组成一支副本队伍，将队伍集合返回至客户端
                        GetOneTeam(raidTeam[raidID],peer);
                        response.ReturnCode = (short)ReturnCode.CreateTeamSuccess;
                    }
                }
                else
                {
                    //不存在队列,添加队列，并且加入队列
                    raidTeam.Add(raidID, new Queue<TaidouPeer>());
                    raidTeam[raidID].Enqueue(peer);
                    response.ReturnCode = (short)ReturnCode.WaitTeam;
                }
                TaidouApplication.log.Debug("Add one peer to raidTeam: username: " + peer.user.username + ",raidID: " + peer.raidID);
            }
            else if(request.Parameters.ContainsKey((byte)ParamCode.CancelTeam))
            {
                //取消组队操作
                int raidID = peer.raidID;
                if (raidTeam.ContainsKey(raidID))
                {
                    if (raidTeam[raidID].Contains(peer))
                    {
                        TaidouApplication.log.Debug("取消时候的队列情况："+raidTeam[raidID].Count);
                        List<TaidouPeer> temp= new List<TaidouPeer>(raidTeam[raidID].ToArray<TaidouPeer>());
                        temp.Remove(peer);
                        raidTeam[raidID].Clear();
                        foreach (var p in temp)
                        {
                            raidTeam[raidID].Enqueue(p);
                        }
                        TaidouApplication.log.Debug("Delete one peer from raidTeam: username: " + peer.user.username + ",raidID: " + peer.raidID);
                    }
                }
                response.ReturnCode = (short)ReturnCode.CancelTeamSuccess;
            }
            else
            {
                //未知操作
                response.ReturnCode = (short)ReturnCode.Exception;
            }
            return response;
        }

        private void GetOneTeam(Queue<TaidouPeer> teamQ,TaidouPeer master)
        {
            List<string> playerNames = new List<string>();
            List<bool> playerSexes = new List<bool>();
            string masterName = roleUtil.GetRoleByUser(master.user)[0].name;//获得主机的角色名字
            bool isMan = roleUtil.GetRoleByUser(master.user)[0].isman;//获得主机角色的性别
            master.leaderName = masterName;
            playerNames.Add(masterName);//第一个元素为主机的角色名字
            playerSexes.Add(isMan);//第一个元素为主机的性别
            //完成参数赋值
            TaidouPeer[] team = teamQ.ToArray<TaidouPeer>();
            foreach (TaidouPeer p in team)
            {
                p.leaderName = masterName;
                playerNames.Add(roleUtil.GetRoleByUser(p.user)[0].name);
                playerSexes.Add(roleUtil.GetRoleByUser(p.user)[0].isman);
            }
            //传回参数
            EventData data = new EventData();
            data.Code = (byte)OperationCode.Team;
            Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.TeamName,playerNames);
            para.Add((byte)ParamCode.TeamSex, Regex.Unescape(JsonMapper.ToJson(playerSexes)));
            data.Parameters=para;
            master.SendEvent(data, new SendParameters());//主机参数传回
            foreach (TaidouPeer p in team)
            {
                p.SendEvent(data, new SendParameters());//从机参数传回
            }
            //peer对象字典创建
            if (peerTeams.ContainsKey(masterName))
            {
                peerTeams[masterName] = new List<TaidouPeer>();//重新构建
            }
            else
            {
                peerTeams.Add(masterName, new List<TaidouPeer>());
            }
            peerTeams[masterName].Add(master);
            foreach (var t in team)
            {
                peerTeams[masterName].Add(t);
            }
            // 请求队列清空
            teamQ.Clear();
            TaidouApplication.log.Debug("队列已经清空,数目："+teamQ.Count+",并成功创建peer字典:"+masterName);
        }
    }
}

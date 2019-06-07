using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouServer.DataBase.Utils;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouCommon.ParamTools;

namespace TaidouServer.Handler
{
    class KnapsackHandler:HandlerBase
    {
        RoleUtil roleUtil;
        KnapsackUtil kutil;
        public KnapsackHandler()
        {
            roleUtil = new RoleUtil();
            kutil = new KnapsackUtil();
        }
        public override OperationResponse HandlerRequestMessage(OperationRequest request, TaidouPeer peer)
        {
            OperationResponse res = new OperationResponse();
            res.OperationCode=request.OperationCode;
            Dictionary<byte, object> param = request.Parameters;
            if (param.ContainsKey((byte)ParamCode.GetKnapsack))
            {
                //获取背包数据请求
                Role role = roleUtil.GetRoleByUser(peer.user)[0];
                List<Knapsack> knapDB = GetFromDB(role);
                if (knapDB.Count == 0)
                {
                    res.ReturnCode = (short)ReturnCode.EmptyKnapsack;
                }
                else
                {
                    res.ReturnCode = (short)ReturnCode.GetKnapsack;
                    res.Parameters = ParamTool.ConstructParam(ParamCode.GetKnapsack, knapDB);
                }
            }
            else if (param.ContainsKey((byte)ParamCode.UpdateKnapsack))
            {
                //更新（添加）背包数据
                List<Knapsack> knaps = ParamTool.GetParam<List<Knapsack>>(ParamCode.UpdateKnapsack, param);
                if (knaps == null)
                {
                    res.ReturnCode = (short)ReturnCode.Exception;
                    res.DebugMessage = "dic参数解析失败";
                }
                else
                {
                    Role role = roleUtil.GetRoleByUser(peer.user)[0];
                    UpdateToDB(knaps, role);
                    res.ReturnCode = (short)ReturnCode.UpdateKnapsack;
                }
            }
            else
            {
                res.ReturnCode = (short)ReturnCode.Exception;
                res.DebugMessage = "dic参数不存在";
            }
            return res;
        }

        private List<Knapsack> GetFromDB(Role role)
        {
            IList<Knapsack> dbdate = kutil.GetKnapsackByRole(role);
            foreach (var t in dbdate)
            {
                t.id = 0;
                t.role = null;
            }
            return (List<Knapsack>)dbdate;
        }

        private void UpdateToDB(List<Knapsack> knaps,Role role)
        {
            kutil.DeleteAllKnapsackByRole(role);
            foreach (var temp in knaps)
            {
                temp.role = role;
            }
            kutil.AddKnapsack(knaps);
        }

    }
}

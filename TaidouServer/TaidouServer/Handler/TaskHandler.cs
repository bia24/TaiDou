using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouServer.DataBase.Utils;
using TaidouCommon;
using TaidouCommon.ParamTools;
using TaidouCommon.Model;

namespace TaidouServer.Handler
{
    class TaskHandler:HandlerBase
    {
        TaskUtil utilTask;
        RoleUtil utilRole;
        public TaskHandler()
        {
            utilTask = new TaskUtil();
            utilRole = new RoleUtil();
        }
        public override OperationResponse HandlerRequestMessage(OperationRequest request, TaidouPeer peer)
        {
            OperationResponse response = new OperationResponse();
            response.OperationCode = request.OperationCode;
            List<TaskDB> taskDbs= ParamTool.GetParam<List<TaskDB>>(ParamCode.AddTaskList,request.Parameters);
            if (taskDbs != null)
            {
                Role role= (utilRole.GetRoleByUser(peer.user))[0];
                //处理添加任务集合到数据库的操作
                foreach(var t in taskDbs){
                    t.role = role;
                }
                utilTask.AddTask(taskDbs);
                response.ReturnCode = (short)ReturnCode.Success;
                return response;
            }
            object para = null;
            request.Parameters.TryGetValue((byte)ParamCode.GetTaskList, out para);
            if (para != null)
            {
                //处理获取任务集合的操作
                Role role = (utilRole.GetRoleByUser(peer.user))[0];
                IList<TaskDB> temp= utilTask.GetTaskByRole(role);
                if (temp.Count == 0)
                {//空任务列表
                    response.ReturnCode = (short)ReturnCode.Fail;
                    return response;
                }
                List<TaskDB> temp2 = new List<TaskDB>();
                foreach (var t in temp)
                {
                    t.role = null;
                    temp2.Add(t);
                }
                Dictionary<byte, object> respara = ParamTool.ConstructParam(ParamCode.GetTaskList, temp2);
                response.ReturnCode = (short)ReturnCode.GetTask;
                response.Parameters = respara;
                return response;
            }
            TaskDB tdb = ParamTool.GetParam<TaskDB>(ParamCode.UpdateTask, request.Parameters);
            if (tdb != null)
            {
                //处理任务更新操作
                Role role = (utilRole.GetRoleByUser(peer.user))[0];
                TaskDB task = (utilTask.GetOneTask(role, tdb.taskid))[0];
                task.createtime = DateTime.Now;
                task.taskprogress = tdb.taskprogress;
                utilTask.UpdateTask(task);
                response.ReturnCode = (short)ReturnCode.UpdateTask;
                return response;
            }
            response.ReturnCode = (short)ReturnCode.Exception;
            return response;
        }
    }
}

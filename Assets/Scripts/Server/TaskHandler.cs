using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;
using TaidouCommon.ParamTools;
using TaidouCommon.Model;
using System;

public class TaskHandler : ClientHandlerBase
{
    private void Awake()
    {
        code = OperationCode.Task;
        TaidouClient.Instance.RegistClientHandler(code, this);
    }
    public override void Start()
    {
    }

    public override void HandlerResponseMessage(OperationResponse response)
    {
        ReturnCode returnCode =(ReturnCode) response.ReturnCode;
        switch (returnCode)
        {
            case ReturnCode.Exception:
                Debug.Log("未知操作");
                break;
            case ReturnCode.Success:
                Debug.Log("任务列表增加成功");
                break;
            case ReturnCode.GetTask:
                Debug.Log("任务列表获取成功");
                UpdatePlayerTaskList(response);
                ((UIManager_02NoviceVillage)GameControl.Instance.mUIManager).InitShowTask();
                break;
            case ReturnCode.UpdateTask:
                Debug.Log("任务更新成功");
                break;
            case ReturnCode.Fail:
                Debug.Log("空任务列表");
                AddTaskList(GameControl.Instance.playerInfo.playerTask);
                break;
        }
    }


    public void AddTaskList(List<Task> tasks)
    {
        List<TaskDB> taskDBList = new List<TaskDB>();
        foreach (var t in tasks)
        {
            taskDBList.Add(ConvertTaskToTaskDB(t));
        }
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.AddTaskList, taskDBList);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Task,para);
    }

    public void  GetTaskList()
    {
        Dictionary<byte, object> para = new Dictionary<byte, object>();
        para.Add((byte)ParamCode.GetTaskList, "get");
        TaidouClient.Instance.SendOperationRequest(OperationCode.Task, para);
    }

    public void UpdateOneTask(Task task)
    {
        TaskDB taskDB = ConvertTaskToTaskDB(task);
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.UpdateTask, taskDB);
        TaidouClient.Instance.SendOperationRequest(OperationCode.Task, para);
    }


    private TaskDB ConvertTaskToTaskDB(Task task)
    {
        TaskDB taskDB = new TaskDB();
        taskDB.taskid = task.id;
        taskDB.type = (TaidouCommon.Model.TaskType)task.type;
        taskDB.name = task.name;
        taskDB.icon = task.icon;
        taskDB.des = task.des;
        taskDB.gold = task.gold;
        taskDB.diamond = task.diamond;
        taskDB.talkwithnpc = task.talkWithNpc;
        taskDB.idnpc = task.idNpc;
        taskDB.idraid = task.idRaid;
        taskDB.taskprogress = (TaidouCommon.Model.TaskProgress)task.taskProgress;
        return taskDB;
    }

    private void UpdatePlayerTaskList(OperationResponse response)
    {
        List<TaskDB> taskDBs = ParamTool.GetParam<List<TaskDB>>(ParamCode.GetTaskList, response.Parameters);
        List<Task> tasks = new List<Task>();
        foreach(var t in taskDBs)
        {
            Task temp = ConvertTaskDBToTask(t);
            if ((DateTime.Now - t.createtime).TotalSeconds >= 3600)
            {
                temp.taskProgress = TaskProgress.UnStart;
                //更新数据库
                UpdateOneTask(temp);
            }
            tasks.Add(temp);
        }
        //更新player的任务列表
        GameControl.Instance.playerInfo.playerTask = tasks;
    }

    private Task ConvertTaskDBToTask(TaskDB taskDB)
    {
        Task temp = new Task();
        temp.id = taskDB.taskid;
        temp.type = (TaskType)taskDB.type;
        temp.name = taskDB.name;
        temp.icon = taskDB.icon;
        temp.des = taskDB.des;
        temp.gold = taskDB.gold;
        temp.diamond = taskDB.diamond;
        temp.talkWithNpc = taskDB.talkwithnpc;
        temp.idNpc = taskDB.idnpc;
        temp.idRaid = taskDB.idraid;
        temp.taskProgress = (TaskProgress)taskDB.taskprogress;
        return temp;
    }


    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }

}

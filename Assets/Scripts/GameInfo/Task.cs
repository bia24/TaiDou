using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType
{
    Main,//主线任务
    Reward,//赏金任务
    Daily//日常任务
}

public enum TaskProgress
{
    UnStart,//未开启
    Accepted,//接受
    Complete,//完成
    Reward//领赏
}


public class Task
{
    public int id;
    public TaskType type;
    public string name;
    public string icon;
    public string des;
    public int gold;
    public int diamond;
    public string talkWithNpc;
    public int idNpc;
    public int idRaid;
    public TaskProgress taskProgress;
}

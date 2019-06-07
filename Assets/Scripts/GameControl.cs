using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TaidouCommon.Model;

public enum RaidType
{
    None,
    Person,
    Team
}


public class GameControl : MonoBehaviour {
    private static GameControl _instance;
    public static GameControl Instance
    {
        get { return _instance; }
    }
#region 持有的相关引用
    //UI管理类
    public UIManagerBase mUIManager { get; set; }
    //服务器列表类
    public List<Server> serverList;
    //持有玩家信息类
    public PlayerInfo playerInfo { get;  set; }
    //持有可选角色类型
    public List<CharacterType> cList { get; private set; }
    //持有装备道具类
    public ArticleList articleList;
    //持有任务信息类
    public List<Task> tasks;
    //持有Npc管理类
    public NPCManager npcManager { get; set; }// todo:记得在场景二加载完成时候赋值
    //持有player自动寻路组件
    public PlayerAutoMove playerAutoMove { get; set; }//todo:记得在场景二加载完成时候赋值
    //持有技能管理类
    public SkillManager skillManager { get; set; }
    //副本类型
    public RaidType raidType = RaidType.None;
    //将要完成的副本id，默认为0
    public int raidIdToFinished = 0;
    //用户账号信息，用来重新连接
    public string Username { get; set; }
    public string Password { get; set; }
    //进入副本的玩家名字和性别，索引对应，0号为主机
    [HideInInspector]
    public List<string> playerNames;
    [HideInInspector]
    public List<bool> playerSexes;
    #endregion


    /// <summary>
    /// awake,初始化
    /// </summary>
    private void Awake()
    {
        //单例
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {//避免重复创建gamecontrol
            Destroy(gameObject);
            return;
        }
        //UI管理类，由各自的脚本主动赋值
        mUIManager = null;
        //本单例不随场景摧毁
        GameControl.DontDestroyOnLoad(this.gameObject);
        //服务器列表
        serverList = null;
        //角色可选列表初始化
        cList = new List<CharacterType>();
        cList.Add(CharacterType.Man);
        cList.Add(CharacterType.Woman);
        //任务信息加载
        LoadTaskInfo();
        //装备道具信息初始化
        articleList = new ArticleList();
        //玩家相关信息初始化
        playerInfo = new PlayerInfo();

        //技能信息初始化
        skillManager = new SkillManager();
        //副本类型赋值
        raidType = RaidType.None;
        //将要完成的raidid赋值
        raidIdToFinished = 0;
   }

    

    #region 逻辑方法
    private void LoadTaskInfo()
    {
        tasks = new List<Task>();
        string task = Resources.Load<TextAsset>("TaskInfo").text;
        string[] lines = task.Split(new char[] { '\r','\n' },StringSplitOptions.RemoveEmptyEntries);
        foreach(string line in lines)
        {
            string[] s = line.Split('|');
            Task t = new Task();
            t.id = int.Parse(s[0]);
            t.type = (TaskType)Enum.Parse(typeof(TaskType), s[1]);
            t.name = s[2];
            t.icon = s[3];
            t.des = s[4];
            t.gold = int.Parse(s[5]);
            t.diamond = int.Parse(s[6]);
            t.talkWithNpc = s[7];
            t.idNpc = int.Parse(s[8]);
            t.idRaid = int.Parse(s[9]);
            t.taskProgress = TaskProgress.UnStart;
            tasks.Add(t);
        }
    }
    #endregion

}

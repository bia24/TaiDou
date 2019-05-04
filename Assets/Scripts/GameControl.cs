using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameControl : MonoBehaviour {
    private static GameControl _instance;
    public static GameControl Instance
    {
        get { return _instance; }
    }
#region 持有的相关引用
    //UI管理类
    public UIManagerBase mUIManager { get; private set; }
    //服务器列表类
    public ServerList mServerlist { get; private set; }
    //持有玩家信息类
    public PlayerInfo playerInfo { get; private set; }
    //持有可选角色类型
    public List<CharacterType> cList { get; private set; }
    //持有装备道具类
    public ArticleList articleList;
    //持有任务信息类
    public List<Task> tasks;
    //持有Npc管理类
    public NPCManager npcManager;// todo:记得在场景二加载完成时候赋值
    //持有player自动寻路组件
    public PlayerAutoMove playerAutoMove;//todo:记得在场景二加载完成时候赋值
    #endregion


    /// <summary>
    /// awake,初始化
    /// </summary>
    private void Awake()
    {
        //单例
        _instance = this;
        //UI管理类
        FindRefrence_UIManager();
        //本单例不随场景摧毁
        GameControl.DontDestroyOnLoad(this.gameObject);
        //服务器列表
        mServerlist = new ServerList();
        //角色可选列表初始化
        cList = new List<CharacterType>();
        cList.Add(CharacterType.Man);
        cList.Add(CharacterType.Women);
        //任务信息加载
        LoadTaskInfo();
        //装备道具信息初始化
        articleList = new ArticleList();
        //玩家相关信息初始化
        playerInfo = new PlayerInfo();

        ///*这个步骤后面不能放在这里*///
        FindRefrence_02NoviceVillageScene();
      
    }

    private void Update()
    {
        //玩家体力活力自动回复
        playerInfo.UpdateEnergyAndPhysical();
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
    public void FindRefrence_02NoviceVillageScene()
    {
        FindRefrence_UIManager();
        FindRefrence_NPCManager();
        FindRefrence_PlayerAutoMove();
    }
    private void FindRefrence_UIManager()
    {
        mUIManager = GameObject.Find("UI Root").GetComponent<UIManagerBase>();
        if (mUIManager == null)
        {
            Debug.LogError("UI Root 上找不到UIManager");
        }
    }
    private void FindRefrence_NPCManager()
    {
        npcManager= GameObject.Find("NPC").GetComponent<NPCManager>();
        if (npcManager == null)
        {
            Debug.LogError("NPC 上找不到npcManager");
        }
    }
    private void FindRefrence_PlayerAutoMove()
    {
        try
        {
            playerAutoMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAutoMove>();
        }catch (Exception e)
        {
            Debug.LogError("找不到playerAutoMove : "+e.Message);
        }
    }
    #endregion

}

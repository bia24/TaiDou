using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInfo {
    #region 字段
    private string username;
    private string passward;
    private string characterName;
    private CharacterType characterType;
    private int characterLevel;
    private int characterPower;
    private int characterExp;
    private int diamond;
    private int gold;
    private int physical;
    private int energy;
    public const int TIMERRECOVER = 2;
    private float timerEnergy;
    private float timerPhysical;
    public  List<KnapsackItem> knapsack;
    public Dictionary<ArticleType, KnapsackItem> dressedEq;
    public List<Task> playerTask;
    private int currentTaskIndex;
    #endregion
    #region 属性
    //用户名
    public string Username { get { return username; } set { username = value; } }
    //密码
    public string Passward { get { return username; } set { username = value; } }
    //角色名
    public string CharacterName
    {
        get { return characterName; }
        set
        {
            characterName = value;
            if (OnNameChange != null)
            {
                OnNameChange(value);
            }
        }
    }
    //角色类型
    public CharacterType CharacterType
    {
        get { return characterType; }
        set
        {
            characterType = value;
            if (OnTypeChange != null)
            {
                OnTypeChange(value);
            }
        }
    }
    //等级
    public int CharacterLevel
    {
        get { return characterLevel; }
        set
        {
            characterLevel = value;
            if (OnLevelChange != null)
            {
                OnLevelChange(value);
            }
        }
    }
    //战斗力
    public int CharacterPower
    {
        get { return characterPower; }
        set
        {
            characterPower = value;
            if (OnPowerChange != null)
            {
                OnPowerChange(value);
            }
        }
    }
    //经验值
    public int CharacterExp
    {
        get { return characterExp; }
        set
        {
            characterExp = value;
            if (OnExpChange != null)
            {
                OnExpChange(value);
            }
        }
    }
    //钻石数
    public int Diamond
    {
        get { return diamond; }
        set
        {
            diamond = value;
            if (OnDiamondChange != null)
            {
                OnDiamondChange(value);
            }
        }
    }
    //金币数
    public int Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            if (OnGoldChange != null)
            {
                OnGoldChange(value);
            }
        }
    }
    //体力值
    public int Physical
    {
        get { return physical; }
        set
        {
            physical = value;
            if (OnPhysicalChange != null)
            {
                OnPhysicalChange(value);
            }
        }
    }
    //精力值
    public int Energy
    {
        get { return energy; }
        set
        {
            if (value <= 50)
            {
                energy = value;
            }
            else
            {
                energy = 50;
            }
            if (OnEnergyChange != null)
            {
                OnEnergyChange(value>50?50:value);
            }
        }
    }

    //体力计时器
    public float TimerPhysical
    {
        private get { return timerPhysical; }
        set
        {
            timerPhysical = value;
            if (OnTimerPhysicalChange != null)
            {
                OnTimerPhysicalChange(value);
            }
        }
    }

    //活力计时器
    public float TimerEnergy
    {
        private get { return timerEnergy; }
        set
        {
            timerEnergy = value;
            if (OnTimerEnergyChange != null)
            {
                OnTimerEnergyChange(value);
            }
        }
    }

   
    #endregion
    #region 事件定义
    public event Action<string> OnNameChange;
    public event Action <CharacterType>OnTypeChange;
    public event Action <int>OnLevelChange;
    public event Action <int>OnPowerChange;
    public event Action <int>OnExpChange;
    public event Action <int>OnDiamondChange;
    public event Action <int>OnGoldChange;
    public event Action <int>OnPhysicalChange;
    public event Action <int>OnEnergyChange;
    public event Action<float> OnTimerPhysicalChange;
    public event Action<float> OnTimerEnergyChange;
    #endregion

    public PlayerInfo(){
        InitBaseInfo();
        InitKnapsackInfo();
        InitDressedEq();
        InitPlayerTask();
    }

    #region 逻辑方法
    //玩家基本信息初始化
    private void InitBaseInfo()
    {
        Username = null;
        Passward = null;
        CharacterName = "北门一条狗";
        CharacterType = CharacterType.Women;
        CharacterLevel = 24;
        CharacterPower = 998;
        CharacterExp = 531;
        Diamond = 111;
        Gold = 222;
        Physical = 77;
        Energy = 15;
        TimerEnergy = TIMERRECOVER;
        TimerPhysical = TIMERRECOVER;
    }
    //背包物品初始化
    private void InitKnapsackInfo()
    {
        //TODO: 使用账号信息链接服务器获得背包信息状态

        knapsack = new List<KnapsackItem>();
        //模拟初始化背包
        for (int i = 0; i < 20; i++)
        {
            int id = UnityEngine.Random.Range(1001, 1020);
            ArticleInfo ari = GameControl.Instance.articleList.Get(id);
            if (ari.Type == ArticleType.Box || ari.Type == ArticleType.Drug)
            {
                bool isExist = false;
                for (int j = 0; j < knapsack.Count; j++)
                {
                    if (knapsack[j].ItemInfo.ID == ari.ID)
                    {
                        knapsack[j].Count++;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    knapsack.Add(new KnapsackItem(ari));
                }
            }
            else
            {
                //装备
                knapsack.Add(new KnapsackItem(ari));
            }
        }
    }
    //身上穿戴装备初始化
    private void InitDressedEq()
    {
        //todo:连接数据库 获得玩家已有的装备
        //模拟：
        dressedEq = new Dictionary<ArticleType, KnapsackItem>();
       for(int i = 1001; i < 1020; i++)
        {
            ArticleInfo ari = GameControl.Instance.articleList.Get(i);
            if (ari.Type != ArticleType.Box && ari.Type != ArticleType.Drug)
            {
                if (!dressedEq.ContainsKey(ari.Type))
                {
                    dressedEq.Add(ari.Type, new KnapsackItem(ari));
                }
            }
        }
    }
    //玩家任务初始化
    private void InitPlayerTask()
    {
        playerTask = new List<Task>();
        foreach(var t in GameControl.Instance.tasks)
        {
            Task n = new Task();
            n.id = t.id;
            n.name = t.name;
            n.type = t.type;
            n.des = t.des;
            n.icon = t.icon;
            n.idNpc = t.idNpc;
            n.idRaid = t.idRaid;
            n.gold = t.gold;
            n.diamond = t.diamond;
            n.talkWithNpc = t.talkWithNpc;
            n.taskProgress = t.taskProgress;
            playerTask.Add(n);
        }
    }



    //玩家体力活力自动回复
    public void UpdateEnergyAndPhysical()
    {
        if (Energy < 50)
        {
            TimerEnergy -= Time.deltaTime;
            if (TimerEnergy < 0)
            {
                Energy += 1;
                TimerEnergy = TIMERRECOVER;
            }
        }
        if (Physical < 100)
        {
            TimerPhysical -= Time.deltaTime;
            if (TimerPhysical < 0)
            {
                Physical += 1;
                TimerPhysical = TIMERRECOVER;
            }
        }
    }

    //获得当前等级升级需要的经验
    public int LevelUpNeedExp()
    {
        int currentLevel = CharacterLevel;
        if(currentLevel==1)
        {
            return 50;
        }
        else
        {
            return 5 * currentLevel * currentLevel + 85 * currentLevel - 90;
        }
        
    }

    public void GetTask(int taskIndex)
    {
        playerTask[taskIndex].taskProgress = TaskProgress.Accepted;

        currentTaskIndex = taskIndex;//当前任务index赋值

        //自动寻路
        int npcId = playerTask[taskIndex].idNpc;
        GameObject npc = GameControl.Instance.npcManager.GetNpcById(npcId);
        GameControl.Instance.playerAutoMove.OpenPlayerAutoMove(npc);
    }
    public void FinishTask(int taskIndex)//todo:副本里调用
    {
    }   
    public void RewardTask(int taskIndex)
    {
        Task task = playerTask[taskIndex];
        task.taskProgress = TaskProgress.Complete;
        Gold += task.gold;
        Diamond += task.diamond;
        playerTask.RemoveAt(taskIndex);
    }
    public Task GetCurrentTask()
    {
        return playerTask[currentTaskIndex];
    }
    #endregion
}

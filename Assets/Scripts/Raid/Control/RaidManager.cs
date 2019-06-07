using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TaidouCommon.Model;

public class RaidManager : MonoBehaviour {

    private static RaidManager instance;
    public static RaidManager Instance { get { return instance; } }
    //本地player引用持有
    private GameObject player;
    public GameObject Player { get { return player; } }
    //所有参加游戏的player引用集合
    public Dictionary<string, GameObject> playersAll;
    //本客户端是否为主机
    [HideInInspector]
    public bool isMaster=false;
    //raid敌人引用持有，客户端检测攻击使用
    public List<GameObject> enemys=new List<GameObject>();
    //非主机客户端用来同步敌人位置和旋转使用
    public Dictionary<string, GameObject> enemysDic = new Dictionary<string, GameObject>();
    //主机客户端用来上传服务器需要同步 移动 的敌人
    public List<GameObject> enemysNeedSync = new List<GameObject>();
    //主机客户端用来上传服务器需要同步 动画 的敌人
    public List<GameObject> enemysNeedSyncAm = new List<GameObject>();
    //主机客户端用来上传敌人被攻击需要的对象
    public List<EnemyTakeAttackModel> enemysTakeAttack = new List<EnemyTakeAttackModel>();
    //持有player生成点的引用
    public Transform playerSpawnPos;
    //游戏是否胜利
    [HideInInspector]
    public bool isEnd = false;
    //ui引用
    private GameObject uiroot;
    private GameObject loadingUI;
    private UISlider slider;
    
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //加载本地能控制的player,其他player在后面加载
        LoadLocalPlayer(GameControl.Instance.playerInfo);
        //相关引用赋值
        player= GameObject.FindWithTag("Player");
        player.GetComponent<PlayerMove>().isLocalPlayer = true;
        if (GameControl.Instance.raidType == RaidType.Team)
        {
            if (playersAll == null)
            {
                playersAll = new Dictionary<string, GameObject>();
                //将本地player先加入
                playersAll.Add(GameControl.Instance.playerInfo.CharacterName, player);
            }
            isMaster = (GameControl.Instance.playerNames[0] == GameControl.Instance.playerInfo.CharacterName);//本客户端是否为主机
        }
        //ui赋值
        uiroot = GameObject.Find("UI Root");
        loadingUI = uiroot.transform.Find("Loading_u").gameObject;
        slider = loadingUI.transform.Find("Loading_Bar_bg_u").GetComponent<UISlider>();
        loadingUI.SetActive(false);
    }

    private void Start()
    {
        //战斗结果面板“返回主城”按钮点击事件注册
        GameResultPanel.Instance.ReturnBtn.onClick.Add(new EventDelegate(OnReturnBtnClick));
        //团队副本开启同步
        if (GameControl.Instance.raidType == RaidType.Team)
        {
            InvokeRepeating("SendLocalPlayerDataToServer", 0, 1f/50);
            InvokeRepeating("SendLocalEnemyDataToServer", 0, 1f/50);
            InvokeRepeating("SendLocalEnemyTakeAttackToServer", 0, 1f / 50);
        }
    }



    private void LoadLocalPlayer(PlayerInfo playerInfo)
    {
        string prefabName = null;
        switch (playerInfo.CharacterType)
        {
            case CharacterType.Man:
                prefabName = "Player_Man_Raid";
                break;
            case CharacterType.Woman:
                prefabName = "Player_Man_Raid";//todo::::::::modify
                break;
        }
        //加载资源
        GameObject go = Resources.Load(prefabName) as GameObject;
        //实例化
        Instantiate(go, playerSpawnPos.position, Quaternion.identity);
    }

    private void OnReturnBtnClick()
    {
        //加载到场景2
        StartCoroutine(LoadingScene("02.NoviceVillage"));
    }

    IEnumerator LoadingScene(string sceneName)
    {
        //loding 图显示
        loadingUI.SetActive(true);
        //异步加载
        slider.value = 0;
        yield return new WaitForEndOfFrame();
        AsyncOperation asy = SceneManager.LoadSceneAsync(sceneName);
        asy.allowSceneActivation = false;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            slider.value = Mathf.Lerp(slider.value, asy.progress, Time.deltaTime);
            if (Mathf.Abs(slider.value - 0.9f) <= 0.02f)
            {
                while (true)
                {
                    yield return new WaitForEndOfFrame();
                    slider.value = Mathf.Lerp(slider.value, 1f, Time.deltaTime);
                    if (Mathf.Abs(slider.value - 1f) <= 0.02f)
                    {
                        slider.value = 1f;
                        yield return new WaitForSeconds(1f);
                        asy.allowSceneActivation = true;
                        break;
                    }
                }
                break;
            }
        }
        yield return null;
    }



    //游戏胜利
    public void OnGameEnd(bool win)
    {
        //游戏结束标志
        isEnd = true;
        if (win)// 游戏胜利
        {
            //调用游戏显示结果面板
            GameResultPanel.Instance.Show("恭喜！通关地下城！", Color.green);
            //更新玩家的任务列表
            foreach (Task task in GameControl.Instance.playerInfo.playerTask)
            {
                if (task.idRaid == GameControl.Instance.raidIdToFinished && task.taskProgress == TaskProgress.Accepted)
                {
                    task.taskProgress = TaskProgress.Reward;
                    uiroot.GetComponent<TaskHandler>().UpdateOneTask(task);
                }
            }
        }
        else
        {//游戏失败
            //调用游戏显示结果面板
            GameResultPanel.Instance.Show("挑战失败，别灰心！", Color.red);
        }
    }


    private void Update()
    {
        //自动回复体力精力
        GameControl.Instance.playerInfo.UpdateEnergyAndPhysical();
    }


    #region 玩家移动和旋转同步
    //向服务器发送本客户端玩家移动信息
    private void SendLocalPlayerDataToServer()
    {
        //创建当前客户端的player实时信息
        SynPlayer localPlayer = new SynPlayer();
        localPlayer.Name = GameControl.Instance.playerInfo.CharacterName;
        localPlayer.isConnect = true;
        Vector3 pos = player.transform.position;
        Vector3 eula = player.transform.eulerAngles;
        localPlayer.Position = new Position(pos.x, pos.y, pos.z);
        localPlayer.Euler = new Euler(eula.x, eula.y, eula.z);
        //发送到服务器
        uiroot.GetComponent<SyncPlayerHandler>().SycnToServer(localPlayer);
    }
    //同步本地客户端玩家移动信息
    public void SyncForLocalPlayerData(List<SynPlayer>serverPlayers)
    {
        foreach(var remoteDate in serverPlayers)
        {
            //断线判断
            if (remoteDate.isConnect == false)
            {
                GameObject localPlayer = null;
                if (playersAll.TryGetValue(remoteDate.Name, out localPlayer))
                {
                    playersAll.Remove(remoteDate.Name);
                    Destroy(localPlayer.GetComponent<PlayerAttackManager>().hpBar);
                    Destroy(localPlayer.GetComponent<PlayerAttackManager>().hudText);
                    Destroy(localPlayer);
                    Debug.Log("玩家 " + remoteDate.Name + "断开了连接");
                }
            }
            else
            {
                if (remoteDate.Name != GameControl.Instance.playerInfo.CharacterName)
                {//不是本客户端的player，需要进行同步到本地
                    if (!playersAll.ContainsKey(remoteDate.Name))
                    {//不包含，也就是需要创建新的gameobject
                        GameObject go = LoadOtherPlayer(remoteDate.Name);
                        //加入本地玩家列表
                        playersAll.Add(remoteDate.Name, go);
                    }
                    //包含了，对它的属性进行同步
                    GameObject localGo = playersAll[remoteDate.Name];
                    RemoteDataToLocal(remoteDate, localGo);
                }
            }
        }
    }
    //加载非本客户端的玩家
    private GameObject LoadOtherPlayer(string name)
    {
        int index=GameControl.Instance.playerNames.IndexOf(name);
        bool isMan = GameControl.Instance.playerSexes[index];
        string prefabName = null;
        switch (isMan)
        {
            case true:
                prefabName = "Player_Man_Raid";
                break;
            case false:
                prefabName = "Player_Man_Raid";//todo::::::::modify
                break;
        }
        //加载资源
        GameObject go = Resources.Load(prefabName) as GameObject;
        //实例化
        go= Instantiate(go, playerSpawnPos.position, Quaternion.identity);
        go.GetComponent<PlayerMove>().isLocalPlayer = false;
        return go;
    }
    //位置与旋转同步
    private void RemoteDataToLocal(SynPlayer remoteData,GameObject go)
    {
        go.transform.position = new Vector3((float)remoteData.Position.x, (float)remoteData.Position.y, (float)remoteData.Position.z);
        go.transform.eulerAngles=new Vector3((float)remoteData.Euler.x, (float)remoteData.Euler.y, (float)remoteData.Euler.z);
    }
    #endregion

    #region 玩家动画同步
    //向服务器发送本客户端玩家的动画信息
    public void SendLocalAnimationToServer(string transationName, TransationType ttype, bool state)
    {
        if (GameControl.Instance.raidType == RaidType.Person) return;
        AmModel model = new AmModel()
        {
            playerName = GameControl.Instance.playerInfo.CharacterName,
            transationName = transationName,
            type = ttype,
            state = state
        };
        uiroot.GetComponent<SyncPlayerHandler>().SycnToServer(model);
    }
    //服务器传回动画实时信息同步
    public void SyncAnimationToLocal(AmModel model)
    {
        string playerName = model.playerName;
        if (playerName == GameControl.Instance.playerInfo.CharacterName) return;//如果是本客户端的游戏对象，不进行同步
        if (!playersAll.ContainsKey(playerName))
        {
            Debug.Log("动画同步，不存在玩家：" + playerName);
            return;
        }
        else
        {
            GameObject go = playersAll[playerName];
            if (go.GetComponent<PlayerAnimation>() == null) return;//go游戏对象还没有初始化好，先不同步了
            if (model.type == TransationType.Bool)
            {
                go.GetComponent<PlayerAnimation>().SetBool(model.transationName, model.state);
            }
            else if (model.type == TransationType.Trigger)
            {
                go.GetComponent<PlayerAnimation>().SetTrigger(model.transationName);
            }
            else
            {
                Debug.Log("动画同步，不存在的动画参数设置类型：" + model.type.ToString());
                return;
            }
        }
    }
    #endregion

    #region 敌人同步
    //主机生成敌人信息，发送到服务器
    public void SendEnemyCreateToServer(List<EnemyCreateModel>enemies)
    {
        if (!isMaster) return;//双重保险
        uiroot.GetComponent<EnemyHandler>().CreateEnemysRequest(enemies);
    }
    //服务器传回敌人创建信息同步
    public void SyncCreateEnemy(List<EnemyCreateModel>enemies)
    {
        if (isMaster) return;//双重保险
        foreach(EnemyCreateModel model in enemies)
        {
            string path = "RaidEnemy/" + model.Name;
            GameObject go= Resources.Load(path) as GameObject;
            GameObject e= Instantiate(go, model.GetPosition(), Quaternion.identity);
            if (e.tag == "Boss")
            {
                e.GetComponent<Boss>().guid = model.Guid;
            }
            else
            {
                e.GetComponent<Enemy>().guid = model.Guid;
            }
            enemys.Add(e);
            enemysDic.Add(model.Guid, e);
        }
    }
    //定时发送需要同步的敌人位置和旋转到服务器
    public void SendLocalEnemyDataToServer()
    {
        if (!isMaster) return;
        if (enemysNeedSync.Count != 0)
        {
            List<EnemyMoveModel> models = new List<EnemyMoveModel>();
            foreach (var go in enemysNeedSync)
            {
                EnemyMoveModel model = new EnemyMoveModel();
                if (go.tag == "Boss")
                {
                    model.Guid = go.GetComponent<Boss>().guid;  
                }
                else
                {
                    model.Guid = go.GetComponent<Enemy>().guid;
                }
                model.Position = new Position(go.transform.position.x, go.transform.position.y, go.transform.position.z);
                model.Euler = new Euler(go.transform.eulerAngles.x, go.transform.eulerAngles.y, go.transform.eulerAngles.z);
                models.Add(model);
            }
            uiroot.GetComponent<EnemyHandler>().EnemysMoveRequest(models);
            enemysNeedSync.Clear();//清空需要同步的列表
        }
        if (enemysNeedSyncAm.Count != 0)
        {
            List<EnemyAmModel> models = new List<EnemyAmModel>();
            foreach(var go in enemysNeedSyncAm)
            {
                EnemyAmModel model = new EnemyAmModel();
                if (go.tag == "Boss")
                {
                    Boss boss = go.GetComponent<Boss>();
                    model.Guid = boss.guid;
                    model.IsIdle = boss.isIdle;
                    model.IsWalk = boss.isWalk;
                    model.IsAttack01 = boss.isAttack01;
                    model.IsAttack02 = boss.isAttack02;
                    model.IsAttack03 = boss.isAttack03;
                }
                else
                {
                    Enemy enemy = go.GetComponent<Enemy>();
                    model.Guid = enemy.guid;
                    model.IsWalk = enemy.isWalk;
                    model.IsIdle = enemy.isIdle;
                    model.IsAttack01 = enemy.isAttack01;
                }
                models.Add(model);
            }
            uiroot.GetComponent<EnemyHandler>().EnemysAmRequest(models);
            enemysNeedSyncAm.Clear();
        }
    }
    //服务器传回敌人移动信息的同步
    public void SyncMoveEnemy(List<EnemyMoveModel>enemies)
    {
        if (isMaster) return;
        foreach (EnemyMoveModel model in enemies)
        {
            GameObject go = null;
            if(enemysDic.TryGetValue(model.Guid,out go))
            {
                go.transform.position = model.GetPosition();
                go.transform.eulerAngles = model.GetEuler();
            }
        }
    }
    //服务器传回敌人动画信息的同步
    public void SyncAmEnemy(List<EnemyAmModel>enemies)
    {
        if (isMaster) return;
        foreach(EnemyAmModel model in enemies)
        {
            GameObject go = null;
            if(enemysDic.TryGetValue(model.Guid,out go))
            {
                Animation anim = go.GetComponent<Animation>();
                if (model.IsIdle)
                {
                    anim.Play("idle");
                }
                if (model.IsWalk)
                {
                    anim.Play("walk");
                }
                if (model.IsAttack01)
                {
                    anim.Play("attack01");
                }
                if (go.tag == "Boss")
                {
                    if (model.IsAttack02)
                    {
                        anim.Play("attack02");
                    }
                    if (model.IsAttack03)
                    {
                        anim.Play("attack03");
                    }
                }
            }
        }
    }
    //主机发送敌人受到攻击请求
    public void SendLocalEnemyTakeAttackToServer()
    {
        if (!isMaster) return;
        if (enemysTakeAttack.Count != 0)
        {
            uiroot.GetComponent<EnemyHandler>().EnemysTakeDamageRequest(enemysTakeAttack);
            enemysTakeAttack.Clear();
        }
    }
    //服务器传回敌人被攻击消息的同步
    public void SyncTakeAttackEnemy(List<EnemyTakeAttackModel> enemies)
    {
        if (isMaster) return;
        foreach (var model in enemies)
        {
            GameObject go = null;
            if(enemysDic.TryGetValue(model.Guid,out go))
            {
                if (go.tag == "Boss")
                {
                    go.GetComponent<Boss>().SyncGetDamaged(model.Damage);
                }
                else
                {
                    go.GetComponent<Enemy>().SyncGetDamaged(model.Damage, (float)model.backOffset, (float)model.upOffset);
                }
            }
        }
    }

    #endregion

}

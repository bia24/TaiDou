using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouCommon.ParamTools;

public class KnapsackHandler : ClientHandlerBase
{
    private void Awake()
    {
        code = OperationCode.Knapsack;
        TaidouClient.Instance.RegistClientHandler(code, this);
    }
    public override void Start()
    {
    }

    public override void HandlerResponseMessage(OperationResponse response)
    {
        ReturnCode returnCode = (ReturnCode)response.ReturnCode;
        switch (returnCode)
        {
            case ReturnCode.EmptyKnapsack:
                Debug.Log("背包为空");
                //当数据库中背包项目为空时候，不操作
                break;
            case ReturnCode.GetKnapsack:
                Debug.Log("获得背包数据，对player信息进行更新");
                UpdatePlayerKnapsack(response);
                ((UIManager_02NoviceVillage)GameControl.Instance.mUIManager).InitShowKnapsackItem();
                ((UIManager_02NoviceVillage)GameControl.Instance.mUIManager).InitShowDressedUpItem();
                break;
            case ReturnCode.Exception:
                Debug.Log(response.DebugMessage);
                break;
            case ReturnCode.UpdateKnapsack:
                Debug.Log("数据库背包数据更新成功");
                break;
        }
    }

    private void UpdatePlayerKnapsack(OperationResponse response)
    {
        List<Knapsack> knapsacks = ParamTool.GetParam<List<Knapsack>>(ParamCode.GetKnapsack, response.Parameters);
        List<KnapsackItem> knaps = new List<KnapsackItem>();//player背包系统
        Dictionary<ArticleType, KnapsackItem> dressedEq = new Dictionary<ArticleType, KnapsackItem>();//player穿戴系统
        foreach (var t in knapsacks)
        {
            ArticleInfo articleInfo = GameControl.Instance.articleList.Get(t.toolid);
            KnapsackItem item = new KnapsackItem(articleInfo);
            if (t.isdressd == false)
            {
                //背包里
                item.Level = t.level;
                item.HPPlus = t.hpadd;
                item.PowerPlus = t.poweradd;
                knaps.Add(item);
            }
            else
            {
                //穿戴着的
                item.Level = t.level;
                item.HPPlus = t.hpadd;
                item.PowerPlus = t.poweradd;
                if (!dressedEq.ContainsKey(articleInfo.Type))
                {
                    dressedEq.Add(articleInfo.Type, item);
                }
            }
        }
        GameControl.Instance.playerInfo.dressedEq = dressedEq;
        GameControl.Instance.playerInfo.knapsack = knaps;
    }




    public void GetKnapsackList()
    {
        Dictionary<byte, object> para = new Dictionary<byte, object>();
        para.Add((byte)ParamCode.GetKnapsack, "get");
        TaidouClient.Instance.SendOperationRequest(code,para);
    }

    public void UpdateKnapsackList()
    {
        List<KnapsackItem> knaps = GameControl.Instance.playerInfo.knapsack;
        Dictionary<ArticleType, KnapsackItem> dressedEq = GameControl.Instance.playerInfo.dressedEq;
        List<Knapsack> transdata = new List<Knapsack>();
        foreach(var t in knaps)//背包赋值
        {
            Knapsack temp = new Knapsack();
            temp.id = 0;
            temp.role = null;
            temp.toolid = t.ItemInfo.ID;
            temp.isdressd = false;
            temp.level = t.Level;
            temp.poweradd = t.PowerPlus;
            temp.hpadd = t.HPPlus;
            temp.count = t.Count;
            transdata.Add(temp);
        }
        //穿戴的装备赋值
        List<ArticleType> types = new List<ArticleType>() {
            ArticleType.Bracelet,
            ArticleType.Clothe,
            ArticleType.Helmet,
            ArticleType.Necklace,
            ArticleType.Ring,
            ArticleType.Shoe,
            ArticleType.Weapon,
            ArticleType.Wing
        };
        foreach(var t in types)
        {
            KnapsackItem k = null;
            if(dressedEq.TryGetValue(t,out k))
            {
                Knapsack temp = new Knapsack();
                temp.id = 0;
                temp.role = null;
                temp.toolid = k.ItemInfo.ID;
                temp.isdressd = true;
                temp.level = k.Level;
                temp.poweradd = k.PowerPlus;
                temp.hpadd = k.HPPlus;
                temp.count = k.Count;
                transdata.Add(temp);
            }
        }
        Dictionary<byte, object> para = ParamTool.ConstructParam(ParamCode.UpdateKnapsack, transdata);
        TaidouClient.Instance.SendOperationRequest(code,para);
    }




    public override void OnDestroy()
    {
        TaidouClient.Instance.DisRegisterCllientHandler(code);
    }


    public void AddKnapsackItem(int toolid)
    { //增加物品的接口，可以优化，药品箱子数量叠加
        PlayerInfo playerInfo = GameControl.Instance.playerInfo;
        ArticleInfo a = GameControl.Instance.articleList.Get(toolid);
        KnapsackItem item = new KnapsackItem(a);
        playerInfo.knapsack.Add(item);
        ((UIManager_02NoviceVillage)GameControl.Instance.mUIManager).InitShowKnapsackItem();
        UpdateKnapsackList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int i = Random.Range(1001, 1020);
            AddKnapsackItem(i);
        }
    }
}

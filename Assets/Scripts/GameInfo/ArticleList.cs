using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ArticleList {
    private Dictionary<int, ArticleInfo> articleList;
    
	public ArticleList()
    {
        articleList = new Dictionary<int, ArticleInfo>();
        string  input= (Resources.Load("ArticleList") as TextAsset).text;
        string[] lines = input.Split(new char[] { '\r','\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            string[] items = line.Split(new char[] { '|' });
            ArticleInfo info = new ArticleInfo();
            try
            {
                info.ID = Convert.ToInt32(items[0]);
                info.Name = items[1];
                info.Icon = items[2];
                info.Type = (ArticleType)Enum.Parse(typeof(ArticleType), items[3]);
                info.Price = items[4] == "" ? 0 : Convert.ToInt32(items[4]);
                info.Level = items[5] == "" ? 0 : Convert.ToInt32(items[5]);
                info.Quality = items[6] == "" ? 0 : Convert.ToInt32(items[6]);
                info.Damage = items[7] == "" ? 0 : Convert.ToInt32(items[7]);
                info.HPPlus = items[8] == "" ? 0 : Convert.ToInt32(items[8]);
                info.PowerPlus = items[9] == "" ? 0 : Convert.ToInt32(items[9]);
                info.AppType = items[10] == "" ? AppType.NULL : (AppType)Enum.Parse(typeof(AppType), items[10]);
                info.ApplyValue = items[11] == "" ? 0 : Convert.ToInt32(items[11]);
                info.Des = items[12];
                articleList.Add(info.ID, info);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }


    }

    public ArticleInfo Get(int id )
    {
        ArticleInfo a = null;
        if(!articleList.TryGetValue(id,out a))
        {
            return null;
        }
        return a;
    }


 }

public class KnapsackItem
{
    private ArticleInfo itemInfo;
    private int count;
    private int level;
    private int hpadd;
    private int poweradd;

    public event Action <int>OnCountChange;
    public event Action<int> OnLevelChange;
    public event Action<int> OnHPAddChange;
    public event Action<int> OnPowerAddChange;
  
    #region 属性
    public int Count {
        get { return count; }
        set {
            count = value;
            if (OnCountChange != null)
            {
                OnCountChange(value);
            }
        }
    }
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            if (OnLevelChange != null)
            {
                OnLevelChange(value);
            }
        }
    }
    public int HPPlus
    {
        get { return hpadd; }
        set
        {
            hpadd = value;
            if (OnHPAddChange != null)
            {
                OnHPAddChange(value);
            }
        }
    }
    public int PowerPlus
    {
        get { return poweradd; }
        set
        {
            poweradd = value;
            if (OnPowerAddChange != null)
            {
                OnPowerAddChange(value);
            }
        }
    }
    public ArticleInfo ItemInfo
    {
        get { return itemInfo; }
        set { itemInfo = value; }
    }

    #endregion


    public KnapsackItem(ArticleInfo info)
    {
        itemInfo = info;
        Count = 1;
        Level = itemInfo.Level;
        HPPlus = itemInfo.HPPlus;
        PowerPlus = itemInfo.PowerPlus;
    }
}






public class ArticleInfo
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public ArticleType Type { get; set; }
    public int Price { get; set; }
    public int Level { get; set; }
    public int Quality { get; set; }
    public int Damage { get; set; }
    public int HPPlus { get; set; }
    public int PowerPlus { get; set; }
    public AppType AppType { get; set; }
    public int ApplyValue { get; set; }
    public string Des { get; set; }
   
}
public enum ArticleType
{
    Helmet,//头盔
    Clothe,//衣服
    Weapon,//武器
    Shoe,//鞋子
    Necklace,//项链
    Bracelet,//手镯
    Ring,//戒指
    Wing,//翅膀
    Drug,//药品
    Box//宝箱
}

public enum AppType
{
    NULL,
    HP,//作用在hp上
    Phisical,//作用在体力上
    Energy//作用在精力上
}
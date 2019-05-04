using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerList {
    private List<ServerItem> sList = null;
    public List<ServerItem> SList { get { return sList; } }
    public ServerList()
    {
        sList = new List<ServerItem>();
        sList.Add(new ServerItem("127.0.0.1","一区 祖达克"));
        sList.Add(new ServerItem("127.0.0.1", "二区 克尔苏加德"));
        sList.Add(new ServerItem("127.0.0.1", "三区 诅咒之地"));
        sList.Add(new ServerItem("127.0.0.1", "四区 死亡之翼"));
        sList.Add(new ServerItem("127.0.0.1", "五区 纳克萨斯"));
        sList.Add(new ServerItem("127.0.0.1", "六区 瘟疫之地"));
        sList.Add(new ServerItem("127.0.0.1", "七区 达拉然"));
        sList.Add(new ServerItem("127.0.0.1", "八区 祖安"));
        sList.Add(new ServerItem("127.0.0.1", "九区 废弃都市"));
        sList.Add(new ServerItem("127.0.0.1", "十区 嘉泰尔"));
    }
	
}

public class ServerItem
{
    public string Ip { get; set; }
    public string Name { get; set; }
    public int ConnectCount { get; set; }
    public ServerItem(string ip,string name,int count=0)
    {
        Ip = ip;
        Name = name;
        ConnectCount = count;
    }
}


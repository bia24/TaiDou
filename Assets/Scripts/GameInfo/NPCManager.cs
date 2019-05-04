using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {
    //持有npc的引用
    public GameObject[] npcArray;
    //存储npc的map，便于通过id进行访问
    private Dictionary<int, GameObject> npcDic;
    //副本点
    public GameObject raidPosition;

    private void Awake()
    {
        InitNpcDic();
    }


    #region 逻辑方法
    private void InitNpcDic()
    {
        npcDic = new Dictionary<int, GameObject>();
        foreach (GameObject go in npcArray)
        {
            int id = int.Parse(go.name.Substring(0, 4));
            npcDic.Add(id, go);
        }
    }

    public GameObject GetNpcById(int id)
    {
        GameObject go = null;
        npcDic.TryGetValue(id, out go);
        return go;
    }
    #endregion
}

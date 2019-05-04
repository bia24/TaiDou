using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerBase : MonoBehaviour {
    //持有本场景所有要使用的ui组件的引用
    private Dictionary<string, Transform> UIDic;

    private void Awake()
    {
        InitDic();
    }
    /// <summary>
    /// 初始化UIDic
    /// </summary>
    private void  InitDic()
    {
        UIDic = new Dictionary<string, Transform>();
        Transform[] transforms = transform.GetComponentsInChildren<Transform>();
        foreach(var t in transforms)
        {
            if (t.name.EndsWith("_u"))
            {
                if (!UIDic.ContainsKey(t.name))
                {
                    UIDic.Add(t.name,t);
                    t.gameObject.AddComponent<UIBehavior>();
                }
            }
        }
    }
    /// <summary>
    /// 获取一个ui对象
    /// </summary>
    /// <param name="name">对象名字</param>
    /// <returns>对象的transform 组件</returns>
    protected UIBehavior GetUI(string name)
    {
        Transform transform=null;
        if (!UIDic.TryGetValue(name, out transform))
        {
            Debug.LogError(name+"找不到" );
        }
        return transform.GetComponent<UIBehavior>();
    }

    /// <summary>
    /// 通过携程定时隐藏某个对象
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="time">时间</param>
    /// <returns></returns>
    protected IEnumerator HideObjectDelay(UIBehavior obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.HideUI();
    }

}

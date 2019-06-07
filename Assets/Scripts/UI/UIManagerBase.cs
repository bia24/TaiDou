using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerBase : MonoBehaviour {
    //持有本场景所有要使用的ui组件的引用
    private Dictionary<string, Transform> UIDic;

    public  virtual void Awake()
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
    public UIBehavior GetUI(string name)
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
    /// <summary>
    /// 将对象obj的所有子按钮都设置为状态state
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="state"></param>
    public void SetChildBtnState(UIBehavior obj,bool state)
    {
        UIButton[] children = obj.transform.GetComponentsInChildren<UIButton>();
        foreach(var c in children)
        {
            c.enabled = state;
        }
    }

}

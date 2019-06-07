using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIBehavior : MonoBehaviour {

    #region ui对象的普通方法
    public void ShowUI() { gameObject.SetActive(true); }
    public void HideUI() { gameObject.SetActive(false); }
    public void DisableUIButton()
    {
        try
        {
            GetComponent<UIButton>().SetState(UIButtonColor.State.Disabled, true);
            GetComponent<Collider>().enabled = false;
        }
        catch (Exception e)
        {
            Debug.Log(gameObject.name + " : " + e.Message);
        }
}
    public void EnableUIButton()
    {
        try
        {
            GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, true);
            GetComponent<Collider>().enabled = true;
        }
        catch (Exception e)
        {
            Debug.Log(gameObject.name + " : " + e.Message);
        }
    }
    #endregion

    #region ui对象中的组件事件注册
    //Onclick 事件注册
    public void RegisterOnClick(EventDelegate item)
    {
        UIButton btn = GetComponent<UIButton>();
        if (btn != null)
        {
            btn.onClick.Add(item);
        }
        else
        {
            Debug.LogError("不存在UIButton： "+gameObject.name);
        }
        
    }
    //OnSubmit 事件注册
    public void RegisterSubmit(EventDelegate item)
    {
        UIInput btn = GetComponent<UIInput>();
        if (btn != null)
        {
            btn.onSubmit.Add(item);
        }
        else
        {
            Debug.LogError("不存在UIInput： " + gameObject.name);
        }

    }
    #endregion

   
}

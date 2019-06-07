using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarManager : MonoBehaviour {
    private static HpBarManager instance;
    public static HpBarManager Instance { get { return instance; } }
    public GameObject hpBarPrefab;
    public GameObject hudTextPrefab;
    private Camera mainCamera;
    private Camera uiCamera;
    private void Awake()
    {
        instance = this;
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        uiCamera = GameObject.Find("UI Root").transform.Find("Camera").GetComponent<Camera>();
    }
    


    public GameObject CreateOneHpBar(GameObject target)
    {
        GameObject go= NGUITools.AddChild(gameObject, hpBarPrefab);
        go.GetComponent<UIFollowTarget>().target = target.transform;
        go.GetComponent<UIFollowTarget>().gameCamera = mainCamera;
        go.GetComponent<UIFollowTarget>().uiCamera = uiCamera;
        return go;
    }

    public GameObject CreateOneHudText(GameObject target)
    {
        GameObject go = NGUITools.AddChild(gameObject, hudTextPrefab);
        go.GetComponent<UIFollowTarget>().target = target.transform;
        go.GetComponent<UIFollowTarget>().gameCamera = mainCamera;
        go.GetComponent<UIFollowTarget>().uiCamera = uiCamera;
        return go;
    }

}

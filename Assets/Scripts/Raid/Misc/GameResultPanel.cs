using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultPanel : MonoBehaviour {
    private static GameResultPanel instance;
    public static GameResultPanel Instance { get { return instance; } }

    private UILabel resultLab;
    private UIButton returnBtn;
    public UIButton ReturnBtn { get { return returnBtn; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //引用赋值
        resultLab = transform.Find("ResulLab").GetComponent<UILabel>();
        returnBtn = transform.Find("ReturnBtn").GetComponent<UIButton>();
        gameObject.SetActive(false);
    }

    public void Show(string message,Color color)
    {
        gameObject.SetActive(true);
        resultLab.text = message;
        resultLab.color = color;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}

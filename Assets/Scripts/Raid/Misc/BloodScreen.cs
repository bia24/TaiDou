using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BloodScreen : MonoBehaviour {
    private static BloodScreen instance;
    public static BloodScreen Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }
    public void ShowBloodScreen()
    {
        GetComponent<UISprite>().alpha = 1;
        GetComponent<TweenAlpha>().ResetToBeginning();
        GetComponent<TweenAlpha>().PlayForward();
    }
}

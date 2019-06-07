using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Combo : MonoBehaviour {
    private static Combo instance;
    public static  Combo Instance { get { return instance; } }

    public float offTime = 2f;
    private float offTimer;
    private int count;
    private UILabel label;

    private void Awake()
    {
        instance = this;
        count = 0;
        label = transform.GetComponentInChildren<UILabel>();
        gameObject.SetActive(false);
    }

  
	
	// Update is called once per frame
	void Update () {
        if (offTimer <= 0)
        {
            gameObject.SetActive(false);
            count = 0;
        }
        offTimer -= Time.deltaTime;
	}


    public void ComboAdd()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        count++;
        label.text = count.ToString();
        offTimer = offTime;
        //动画
        gameObject.transform.DOScale(1.2f, 0.1f).From(1);
        iTween.ShakePosition(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.1f);
        
    }
}

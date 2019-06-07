using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHpBar : MonoBehaviour {

    private static BossHpBar instance;
    public static BossHpBar Instance { get { return instance; } }

    private UILabel bossHpLab;
    private UISlider bossHpBar;
    private int hpMax;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //引用赋值
        bossHpBar = GetComponent<UISlider>();
        bossHpLab = transform.Find("BossHpLab").GetComponent<UILabel>();
        gameObject.SetActive(false);
    }

    public void Show(int hpMax)
    {
        gameObject.SetActive(true);
        bossHpBar.value = 1f;
        bossHpLab.text = hpMax + "/" + hpMax;
        this.hpMax = hpMax;
    }

    public void UpdateShow(int hp)
    {
        if (hp < 0) hp = 0;
        bossHpBar.value = (float)hp / hpMax;
        bossHpLab.text = hp + "/" + hpMax;
    }

    public  void Hide()
    {
        gameObject.SetActive(false);
    }
}

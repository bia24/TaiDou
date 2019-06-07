using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using System.Linq;

public class PlayerAttackManager : MonoBehaviour {
    //攻击特效存储
    private Dictionary<string, AttackEffectControl> attackEffects;
    //其他炫酷特效存储
    public List<GameObject> otherEffectList;
    private Dictionary<string, GameObject> otherEffectDic;
    //攻击范围
    private enum AttackRange { Forward, Around }
    //前方攻击距离
    public float forwardAttackRange = 2f;
    //范围攻击距离
    public float aroundAttackRange = 2f;
    //伤害--todo：先模拟
    private int[] damages = new int[] { 10, 20, 30, 40 };
    //生命值
    public event Action<int> OnHpChangeEvent;
    public int hpSet = 1000;
    private int HP;
    public int hp
    {
        get { return HP; }
        set
        {
            HP = value;
            if (OnHpChangeEvent != null)
            {
                OnHpChangeEvent(value);
            }
        }
    }
    private bool isDead;
    //播放动画的脚本引用
    private PlayerAnimation playerAnimation;
    //hpbar和战斗文字相关引用
    private GameObject hpBarPos;
    public GameObject hpBar { get; set; }
    public GameObject hudText { get; set; }
    private GameObject hudTextPos;
    //与边际碰撞检测
    private bool isEnterBoundary;

    private void Awake()
    {
        InitAttackEffectsDic();
        InitOtherEffectDic();
    }
    private void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        hpBarPos = transform.Find("HpBarPos").gameObject;
        hpBar = HpBarManager.Instance.CreateOneHpBar(hpBarPos);
        hudTextPos = transform.Find("HudTextPos").gameObject;
        hudText = HpBarManager.Instance.CreateOneHudText(hudTextPos);
        hpBar.transform.Find("HpBarFor").GetComponent<UISprite>().color = Color.blue;
        hp = hpSet;
        isDead = false;
        isEnterBoundary = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Boundary")
        {
            isEnterBoundary = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Boundary")
        {
            isEnterBoundary = false;
        }
    }

    #region 内部封装逻辑方法
    private void InitAttackEffectsDic()
    {
        attackEffects = new Dictionary<string, AttackEffectControl>();
        AttackEffectControl[] aes = transform.GetComponentsInChildren<AttackEffectControl>();
        foreach(var ae in aes)
        {
            attackEffects.Add(ae.name, ae);
        }
    }
    private void InitOtherEffectDic()
    {
        otherEffectDic = new Dictionary<string, GameObject>();
        foreach(var t in otherEffectList)
        {
            otherEffectDic.Add(t.name, t);
        }
    }
         

    private void ShowAttackEffect(string name)
    {
        AttackEffectControl aec = null;
        if(attackEffects.TryGetValue(name,out aec))
        {
            aec.Show();
        }
    }

    private void MoveOffset(float offset)
    {
        if (offset == 0) return;
        if (isEnterBoundary == true) return;
        transform.DOMove(transform.position +transform.forward * offset, 0.2f);
            
    }

    private List<GameObject> GetEnemyInAttackRange(AttackRange range)
    {
        List<GameObject> enemys = new List<GameObject>();
        if (range == AttackRange.Forward)
        {
            foreach(GameObject go in RaidManager.Instance.enemys)
            {
                Vector3 p= transform.InverseTransformPoint(go.transform.position);//目标世界坐标转为玩家的局部坐标
                if (p.z > -0.5f)//前方
                {
                    if (Vector3.Distance(Vector3.zero, p) <= forwardAttackRange)
                    {
                        enemys.Add(go);
                    }
                }
            }
        }
        else //范围攻击
        {
            foreach (GameObject go in RaidManager.Instance.enemys)
            {
                if (Vector3.Distance(go.transform.position, transform.position) <= aroundAttackRange)
                {
                    enemys.Add(go);
                }
            }
        }
        return enemys;
    }

    private void AttackEnemy(string attackRange,float backOffset,float upOffset)
    {
        AttackRange range = AttackRange.Around;
        int damage = 0;
        switch (attackRange)
        {
            case "basic":
                range = AttackRange.Forward;
                damage = damages[0];
                break;
            case "skill1":
                range = AttackRange.Around;
                damage = damages[1];
                break;
            case "skill2":
                range = AttackRange.Around;
                damage = damages[2];
                break;
            case "skill3":
                range = AttackRange.Around;
                damage = damages[3];
                break;
        }
        List<GameObject> enemys = GetEnemyInAttackRange(range);
        foreach(GameObject go in enemys)
        {
            if (go.tag == "Boss")
            {
                go.GetComponent<Boss>().GetDamaged(damage);
            }
            else
            {
                go.GetComponent<Enemy>().GetDamaged(damage, backOffset, upOffset);
            }
        }
    }

    private void Die()
    {
        if (!isDead)
        {
            playerAnimation.SetTrigger("DieTrigger");
            //RaidManager.Instance.SendLocalAnimationToServer("DieTrigger", TransationType.Trigger, true);//同步本动画操作
            isDead = true;

            //游戏失败
            RaidManager.Instance.OnGameEnd(false);

        }
    }

    #endregion


    #region 外部调用方法
    /// <summary>
    /// args[0]-技能类型: basic skill1 skill2 skill3
    /// args[1]-特效名字: name
    /// args[2]-声音名字: name
    /// args[3]-向前移动的距离
    /// args[4]-向上移动的距离
    /// </summary>
     public void Attack(string arg)
    {
        string[] args = arg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        string effectName = args[1];
        string soundName = args[2];
        float forwardOffset = float.Parse(args[3]);
        //特效显示
        ShowAttackEffect(effectName);
        //声音播放
        SoundManager.Instance.Play(soundName);
        //位移控制
        MoveOffset(forwardOffset);
        //被攻击的敌人操作
        float upOffset = float.Parse(args[4]);
        AttackEnemy(args[0],forwardOffset,upOffset);
    }

    public void ShowDevilHandEffect()
    {
        AttackRange range = AttackRange.Forward;
        GameObject go = null;
        if(otherEffectDic.TryGetValue("DevilHandMobile",out go))
        {
            List<GameObject> enemys = GetEnemyInAttackRange(range);
            foreach (GameObject e in enemys)
            {
                Vector3 p = e.transform.Find("BloodPoint").position;
                Instantiate(go, new Vector3(p.x, transform.position.y, p.z), e.transform.rotation);
            }
        }
    }

    public void ShowEffectSelfToTarget(string effectName)
    {
        GameObject go = null;
        if (otherEffectDic.TryGetValue(effectName, out go))
        {
            List<GameObject> enemys = GetEnemyInAttackRange(AttackRange.Around);
            foreach (GameObject e in enemys)
            {
                GameObject effect= Instantiate(go,transform.position+Vector3.up,Quaternion.identity);
                effect.GetComponent<EffectSettings>().Target = e;
            }
        }
    }

    public void ShowEffectToTarget(string effectName)
    {
        GameObject go = null;
        if (otherEffectDic.TryGetValue(effectName, out go))
        {
            List<GameObject> enemys = GetEnemyInAttackRange(AttackRange.Around);
            foreach (GameObject e in enemys)
            {
                Instantiate(go, e.transform.position, Quaternion.identity);
            }
        }
    }

    public void GetDamage(int damage)
    {
        if (RaidManager.Instance.isEnd) return;
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            Die();
        }
        else
        {
            //播放被攻击的动画
            int random = 0;//UnityEngine.Random.Range(0, 100);
            if (random < damage)
            {
                playerAnimation.SetTrigger("TakeDamage");
                if (GameControl.Instance.raidType == RaidType.Person)
                {
                    //屏幕上血红特效的显示
                    BloodScreen.Instance.ShowBloodScreen();
                }
                else
                {
                    if (gameObject == RaidManager.Instance.Player)
                    {
                        //本地player才有红色特效
                        BloodScreen.Instance.ShowBloodScreen();
                    }
                }
            }
            //血量减少显示
            hpBar.GetComponent<UISlider>().value = (float)hp / hpSet;
            if (hp != 0)
            {
                hudText.GetComponent<HUDText>().Add("-" + damage, Color.red, 0.6f);
            }
        }
    }

    #endregion
}

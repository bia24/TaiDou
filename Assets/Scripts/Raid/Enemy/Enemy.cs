using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour {
    private Transform bloodPoint;
    public int hpSet = 100;
    private int hp;
    public GameObject bloodEffect;
    public float moveSpeed = 5f;
    public float moveDistance = 30f;
    public float attackDistance = 2f;
    public float attackRate = 2f;
    public float downSpeed = 1f;
    private CharacterController cc;
    private float distance;
    private float attackTimer;
    private bool isDead = false;
    private GameObject hpBarPos;
    private GameObject hpBar;
    private GameObject hudText;
    private GameObject hudTextPos;
    public int attackDamage = 10;
    private bool isGetHpBar = false;
    [HideInInspector]
    public string guid = "";
    private GameObject bestPlayer;
    private Vector3 lastPosition;
    private Vector3 lastEuler;
    private Animation anim;
    [HideInInspector]
    public bool isIdle;
    [HideInInspector]
    public bool isWalk;
    [HideInInspector]
    public bool isAttack01;

    private void Start()
    {
        hp = hpSet;
        bloodPoint = transform.Find("BloodPoint");
        cc = GetComponent<CharacterController>();
        bestPlayer = RaidManager.Instance.Player;
        distance = Vector3.Distance(RaidManager.Instance.Player.transform.position, transform.position);
         InvokeRepeating("ComputeDistanceAndBestPlayer", 0, 0.1f);
        attackTimer = attackRate;
        isGetHpBar = false;

        lastPosition = transform.position;
        lastEuler = transform.eulerAngles;
        anim = GetComponent<Animation>();
        isIdle = anim.IsPlaying("idle");
        isWalk = anim.IsPlaying("walk");
        isAttack01 = anim.IsPlaying("attack01");
    }

    private void FixedUpdate()
    {
        if (RaidManager.Instance.isEnd)
        {
            CancelInvoke();
            return;
        }
        if (Vector3.Distance(RaidManager.Instance.Player.transform.position,transform.position) < 6 && !isGetHpBar) GetHpBarAndHudText(); //ngui bug？血条产生中间有黑的
        if (gameObject.transform.position.y < -10) { hp = 0; Die(); }
        if (hp <= 0) return;
        if (!RaidManager.Instance.isMaster && GameControl.Instance.raidType == RaidType.Team) return; //团队非主机不进行主动移动和动画的update
        if (distance < moveDistance)
        {
            if (distance < attackDistance)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    if(!GetComponent<Animation>().IsPlaying("takedamage"))
                        GetComponent<Animation>().Play("attack01");
                    attackTimer = attackRate;
                }
                else
                {
                    if(!GetComponent<Animation>().IsPlaying("attack01")&&!GetComponent<Animation>().IsPlaying("takedamage"))
                           GetComponent<Animation>().Play("idle");
                }
            }
            else
            {
                attackTimer = attackRate;
                Move();//移动
                if (!GetComponent<Animation>().IsPlaying("takedamage"))
                    GetComponent<Animation>().Play("walk");
            }
         }
        else
        {
            GetComponent<Animation>().Play("idle");
            attackTimer = attackRate;
        }
        SyncMoveAndAnimation();//进行敌人动画和移动的同步
     }


    #region 外部调用方法
    //敌人受到伤害[参数列表]:  0：int 伤害  1：后移距离  2：浮空距离
    //1.播放受到攻击的动画 2.后退和浮空 3.出血特效的播放
    public void GetDamaged(int damage,float backOffset,float upOffset)
    {
        if (GameControl.Instance.raidType == RaidType.Team && !RaidManager.Instance.isMaster) return;
        if (GameControl.Instance.raidType == RaidType.Team)
        {
            EnemyTakeAttackModel model = new EnemyTakeAttackModel();
            model.Guid = GetComponent<Enemy>().guid;
            model.Damage = damage;
            model.backOffset = backOffset;
            model.upOffset = upOffset;
            RaidManager.Instance.enemysTakeAttack.Add(model);
        }
        hp -= damage;
        //战斗浮动信息
        if (hpBar != null && hpBar != null)
        {
            hpBar.GetComponent<UISlider>().value = (float)hp / hpSet;
            hudText.GetComponent<HUDText>().Add("+" + damage, Color.red, 0.6f);
        }
        if (hp <= 0)
        {//死亡
            Die();
            return;
        }
        //受到攻击动画播放
        GetComponent<Animation>().Play("takedamage");
        //旋转看向player
        Vector3 target = new Vector3(bestPlayer.transform.position.x,
            transform.position.y,
            bestPlayer.transform.position.z);
        transform.LookAt(target);
        //朝自己的背后后退和浮空
        transform.DOMove(transform.position - transform.forward * backOffset+Vector3.up*upOffset, 0.2f);
        //出血特效播放-特效中绑定了被击打声音
        Instantiate(bloodEffect, bloodPoint.position, Quaternion.identity);
        Combo.Instance.ComboAdd();
    }
    
    //本地客户端同步服务器操作的 被攻击方法
    public void SyncGetDamaged(int damage, float backOffset, float upOffset)
    {
        hp -= damage;
        //战斗浮动信息
        if (hpBar != null && hudText != null)
        {
            hpBar.GetComponent<UISlider>().value = (float)hp / hpSet;
            hudText.GetComponent<HUDText>().Add("+" + damage, Color.red, 0.6f);
        }
        if (hp <= 0)
        {//死亡
            Die();
            return;
        }
        //受到攻击动画播放
        GetComponent<Animation>().Play("takedamage");
        //旋转看向player
        Vector3 target = new Vector3(bestPlayer.transform.position.x,
            transform.position.y,
            bestPlayer.transform.position.z);
        transform.LookAt(target);
        //朝自己的背后后退和浮空
        transform.DOMove(transform.position - transform.forward * backOffset + Vector3.up * upOffset, 0.2f);
        //出血特效播放-特效中绑定了被击打声音
        Instantiate(bloodEffect, bloodPoint.position, Quaternion.identity);
        Combo.Instance.ComboAdd();
    }
   
    //攻击player的方法，在指定动画的合适位置作为事件调用
    public void Attack()
    {
        if (distance <= attackDistance) //在攻击距离中发动攻击，调用player中的getdamage方法
        {
            bestPlayer.GetComponent<PlayerAttackManager>().GetDamage(attackDamage);
        }
    }

    #endregion

    #region 内部封装方法
    public void Die()
    {
        if (!isDead)
        {
            GetComponent<Animation>().Play("die");
            transform.DOMove(transform.position - transform.up * downSpeed, downSpeed).SetDelay(2f);
            Destroy(gameObject, downSpeed+2f);
            gameObject.GetComponent<CharacterController>().enabled = false;
            RaidManager.Instance.enemys.Remove(gameObject);
            if (!RaidManager.Instance.isMaster)
            {
                if (gameObject.tag == "Boss")
                {
                    RaidManager.Instance.enemysDic.Remove(gameObject.GetComponent<Boss>().guid);
                }
                else
                {
                    RaidManager.Instance.enemysDic.Remove(gameObject.GetComponent<Enemy>().guid);
                }
            }
            Destroy(hpBar);
            Destroy(hudText);
            isDead = true;
        }
    }
    private void Move()
    {
        if (bestPlayer == null) return;
        //朝向player
        Vector3 target = new Vector3(bestPlayer.transform.position.x,
            transform.position.y,
            bestPlayer.transform.position.z);
        transform.LookAt(target);
        //移动
        cc.SimpleMove(transform.forward * moveSpeed);
    }
    private void ComputeDistanceAndBestPlayer()
    {
        if (anim.IsPlaying("attack01")) return;
        if (GameControl.Instance.raidType == RaidType.Person)
        {
            bestPlayer = RaidManager.Instance.Player;
            distance= Vector3.Distance(bestPlayer.transform.position, transform.position);
            return;
        }
        GameObject tempGo = null;
        float tempDis = Mathf.Infinity;
        foreach (string name in GameControl.Instance.playerNames)
        {
            GameObject go = null;
            if(RaidManager.Instance.playersAll.TryGetValue(name,out go))
            {
                float d = Vector3.Distance(go.transform.position, transform.position);
                if (d < tempDis)
                {
                    tempDis = d;
                    tempGo = go;
                }
            }
        }
        if (tempGo == null) return;
        else
        {
            distance = tempDis;
            bestPlayer = tempGo;//赋值最优解
        }
    }
    private void GetHpBarAndHudText()
    {
        hpBarPos = transform.Find("HpBarPos").gameObject;
        hpBar = HpBarManager.Instance.CreateOneHpBar(hpBarPos);
        hudTextPos = transform.Find("HudTextPos").gameObject;
        hudText = HpBarManager.Instance.CreateOneHudText(hudTextPos);
        hpBar.GetComponent<UISlider>().value = (float)hp / hpSet;
        isGetHpBar = true;
    }
    private void SyncMoveAndAnimation()
    {
        if (GameControl.Instance.raidType == RaidType.Person) return;
        if (lastPosition != transform.position || lastEuler != transform.eulerAngles)
        {
            //团队主机，将本游戏对象加入待同步集合
            if (!RaidManager.Instance.enemysNeedSync.Contains(gameObject))
            {
                RaidManager.Instance.enemysNeedSync.Add(gameObject);
            }
            lastPosition = transform.position;
            lastEuler = transform.eulerAngles;
        }
        if (isIdle != anim.IsPlaying("idle") || isWalk != anim.IsPlaying("walk") || isAttack01 != anim.IsPlaying("attack01"))
        {
            isIdle = anim.IsPlaying("idle");
            isWalk = anim.IsPlaying("walk");
            isAttack01 = anim.IsPlaying("attack01");
            //团队主机，将本游戏对象加入待同步集合
            if (!RaidManager.Instance.enemysNeedSyncAm.Contains(gameObject))
            {
                RaidManager.Instance.enemysNeedSyncAm.Add(gameObject);
            }
        }
    }

    #endregion
}

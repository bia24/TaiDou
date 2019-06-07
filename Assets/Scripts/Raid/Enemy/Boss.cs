using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss : MonoBehaviour {

    public float viewAngle = 50;
    public float rotateSpeed = 1;
    public float attackDistance = 3;
    public float moveSpeed = 2;
    public float attackInnerCD = 1f;
    public int[] damageArray;
    private float attackTimer=0;
    private bool isAttacking = false;
    private int attackIndex = 1;
    private GameObject bestPlayer;
    private Animation anim;
    private GameObject attack01;
    private GameObject attack02;
    public GameObject attack03;
    private Transform attack03Pos;
    public int hpSet=2000;
    private int hp;
    private GameObject hpBarPos;
    private GameObject hpBar;
    private GameObject hudText;
    private GameObject hudTextPos;
    private Transform bloodPoint;
    public GameObject bloodEffect;
    private bool isDead=false;
    public float downSpeed=1f;
    [HideInInspector]
    public string guid = "";
    private Vector3 lastPosition;
    private Vector3 lastEuler;
    [HideInInspector]
    public bool isIdle;
    [HideInInspector]
    public bool isWalk;
    [HideInInspector]
    public bool isAttack01;
    [HideInInspector]
    public bool isAttack02;
    [HideInInspector]
    public bool isAttack03;

    // Use this for initialization
    void Start () {
        bestPlayer = RaidManager.Instance.Player;
        anim = GetComponent<Animation>();
        attackTimer = 0;
        isAttacking = false;
        attackIndex = 1;
        attack01 = transform.Find("Attack01").gameObject;
        attack02 = transform.Find("Attack02").gameObject;
        attack01.SetActive(false);
        attack02.SetActive(false);
        attack03Pos = transform.Find("Attack03Pos").gameObject.transform;
        hp = hpSet;
        hpBarPos = transform.Find("HpBarPos").gameObject;
        hpBar = HpBarManager.Instance.CreateOneHpBar(hpBarPos);
        hudTextPos = transform.Find("HudTextPos").gameObject;
        hudText = HpBarManager.Instance.CreateOneHudText(hudTextPos);
        bloodPoint = transform.Find("BloodPoint");
        //大血条显示吧
        BossHpBar.Instance.Show(hpSet);

        lastPosition = transform.position;
        lastEuler = transform.eulerAngles;
        isIdle = anim.IsPlaying("idle");
        isWalk = anim.IsPlaying("walk");
        isAttack01 = anim.IsPlaying("attack01");
        isAttack02 = anim.IsPlaying("attack02");
        isAttack03 = anim.IsPlaying("attack03");
        if (GameControl.Instance.raidType == RaidType.Team)
        {
            InvokeRepeating("ComputeAndBestPlayer", 0, 0.1f);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (RaidManager.Instance.isEnd)
        {
            CancelInvoke();
            return;
        }
        if (isDead) return;
        if (isAttacking) return;
        if (!RaidManager.Instance.isMaster && GameControl.Instance.raidType == RaidType.Team) return; //团队非主机不进行主动移动和动画的update
        //计算夹角
        if (bestPlayer == null) return;
        Vector3 playerPos = bestPlayer.transform.position;
        playerPos.y = transform.position.y;//使玩家和boss的y轴在同一水平线上
        float angle = Vector3.Angle(playerPos - transform.position, transform.forward);
        if (angle <= viewAngle / 2)
        {
            //在视野内的处理
            float distance = Vector3.Distance(playerPos, transform.position);
            if (distance <= attackDistance)
            { //攻击
                if (!isAttacking)
                {
                    attackTimer += Time.deltaTime;
                    anim.CrossFade("idle");
                }
                if (attackTimer >= attackInnerCD)
                {
                    Attack();
                }
            }
            else
            {
                //追击 
                anim.CrossFade("walk");
                Quaternion targetRotation = Quaternion.LookRotation(playerPos - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
                GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * moveSpeed);
            }
        }
        else
        {
            anim.CrossFade("walk");
            //不在视野内，进行boss的旋转
            Quaternion targetRotation= Quaternion.LookRotation(playerPos - transform.position);//产生一个由世界坐标forward到目标方向的旋转
            transform.rotation= Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*rotateSpeed);//因为boss的当前旋转也是以世界坐标forward为基准的，目标可以和boss旋转同步，
                                                                                                                //所以，得到的结果也是以世界坐标为基准的，直接给boss的rotation赋值可以同步
        }
        SyncMoveAndAnimation();
    }


    private void Attack()
    {
        isAttacking = true;
        attackTimer = 0;
        anim.CrossFade("attack0" + attackIndex);
        attackIndex++;
        if (attackIndex == 4) attackIndex = 1;
    }

    private void AttackCallBack()
    {
        isAttacking = false;
    }

    public void ShowEffect(string name)
    {
        GameObject effect = null;
        int damageIndex = 0;
        switch (name)
        {
            case "Attack01":
                effect = attack01;
                damageIndex = 0;
                break;
            case "Attack02":
                effect = attack02;
                damageIndex = 1;
                break;
        }
        effect.SetActive(true);
        if (bestPlayer == null) return;
        Vector3 playerPos = bestPlayer.transform.position;
        playerPos.y = transform.position.y;
        float distance = Vector3.Distance(playerPos, transform.position);
        if (distance < attackDistance)
        {
            bestPlayer.GetComponent<PlayerAttackManager>().GetDamage(damageArray[damageIndex]);
        }
    }

    public void ShowAttack03Effect()
    {
       GameObject go= Instantiate(attack03, attack03Pos.position, Quaternion.identity);
        go.GetComponent<BossBullet>().Damage = damageArray[2];
        go.GetComponent<BossBullet>().player = bestPlayer;
    }

    public void GetDamaged(int damage)
    {
        if (GameControl.Instance.raidType == RaidType.Team && !RaidManager.Instance.isMaster) return;
        if (GameControl.Instance.raidType == RaidType.Team)
        {
            EnemyTakeAttackModel model = new EnemyTakeAttackModel();
            model.Guid = GetComponent<Boss>().guid;
            model.Damage = damage;
            RaidManager.Instance.enemysTakeAttack.Add(model);
        }
        hp -= damage;
        //战斗浮动信息
        if (hpBar != null && hudText != null)
        {
            hpBar.GetComponent<UISlider>().value = (float)hp / hpSet;
            hudText.GetComponent<HUDText>().Add("+" + damage, Color.red, 0.6f);
        }
        //大血条更新
        BossHpBar.Instance.UpdateShow(hp);
        if (hp <= 0)
        {//死亡
            Die();
            return;
        }
        //受到攻击动画播放
        if (!isAttacking)
        {
            anim.CrossFade("takedamage");
        }
        //出血特效播放-特效中绑定了被击打声音
        Instantiate(bloodEffect, bloodPoint.position, Quaternion.identity);
        Combo.Instance.ComboAdd();
    }

    public void SyncGetDamaged(int damage)
    {
        hp -= damage;
        //战斗浮动信息
        if (hpBar != null && hudText != null)
        {
            hpBar.GetComponent<UISlider>().value = (float)hp / hpSet;
            hudText.GetComponent<HUDText>().Add("+" + damage, Color.red, 0.6f);
        }
        //大血条更新
        BossHpBar.Instance.UpdateShow(hp);
        if (hp <= 0)
        {//死亡
            Die();
            return;
        }
        //受到攻击动画播放
        if (!isAttacking)
        {
            anim.CrossFade("takedamage");
        }
        //出血特效播放-特效中绑定了被击打声音
        Instantiate(bloodEffect, bloodPoint.position, Quaternion.identity);
        Combo.Instance.ComboAdd();
    }

    private void Die()
    {
        if (!isDead)
        {
            anim.CrossFade("die");
            transform.DOMove(transform.position - transform.up * downSpeed, downSpeed).SetDelay(5f);
            Destroy(gameObject, downSpeed + 5f);
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
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
            //大血条隐藏
            BossHpBar.Instance.Hide();
            //调用游戏胜利
            RaidManager.Instance.OnGameEnd(true);
            isDead = true;
        }
    }

    //同步boss的移动和动画
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
        if (isIdle != anim.IsPlaying("idle") || isWalk != anim.IsPlaying("walk") || isAttack01 != anim.IsPlaying("attack01")
            ||isAttack02!=anim.IsPlaying("attack02")||isAttack03!=anim.IsPlaying("attack03"))
        {
            isIdle = anim.IsPlaying("idle");
            isWalk = anim.IsPlaying("walk");
            isAttack01 = anim.IsPlaying("attack01");
            isAttack02 = anim.IsPlaying("attack02");
            isAttack03 = anim.IsPlaying("attack03");
            //团队主机，将本游戏对象加入待同步集合
            if (!RaidManager.Instance.enemysNeedSyncAm.Contains(gameObject))
            {
                RaidManager.Instance.enemysNeedSyncAm.Add(gameObject);
            }
        }
    }

    //轮询计算当前最优玩家
    private void ComputeAndBestPlayer()
    {
        if (isAttacking) return;
        GameObject tempGo = null;
        float tempDis = Mathf.Infinity;
        foreach (string name in GameControl.Instance.playerNames)
        {
            GameObject go = null;
            if (RaidManager.Instance.playersAll.TryGetValue(name, out go))
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
            bestPlayer = tempGo;//赋值最优解
        }
    }

}

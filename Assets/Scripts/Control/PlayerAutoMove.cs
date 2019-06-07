using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerAutoMove : MonoBehaviour {
    //自动导航组件
    public NavMeshAgent agent;
    //停止距离
    public float minDistance;
    
    //控制动画第一次变化
    private bool isFirstChange;
    //当前寻路的目标
    private GameObject AutoMoveTarget;
    //给与distance计算时间
    private int count=1;
    //动态浮动标志，用来控制每次的目标地点不一致
    private bool isPlus = true;

    
	// Use this for initialization
	void Start () {
        //向gamecontrol注册自己
        GameControl.Instance.playerAutoMove = this;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        isFirstChange = false;
    }
	
	// Update is called once per frame
	void Update () {
      
        if (!agent.isStopped)
        {
            GetComponent<PlayerAnimationControl>().SetAnimation_IdleToRun();
            if(Mathf.Abs(Input.GetAxis("Horizontal"))>0||Mathf.Abs(Input.GetAxis("Vertical"))>0){
                //跑动过程中停止,关闭自动寻路
                OffPlayerAutoMove();
            }
            if (agent.remainingDistance <= minDistance && agent.remainingDistance != 0)//到达目的地,关闭自动寻路
            {
                if (count == 2)
                {
                    count = 0;
                    OffPlayerAutoMove();
                    //此时调用npc对话框弹出
                    switch (AutoMoveTarget.tag)
                    {
                        case "NPC":
                            ((UIManager_02NoviceVillage)GameControl.Instance.mUIManager).OnPlayerReachedNpc();
                            break;
                        case "NPC_RaidEntry":
                            ((UIManager_02NoviceVillage)GameControl.Instance.mUIManager).OnRaidChooseMapShow();
                            break;
                    }
                }
                count++;
            }
        }
        else
        {
            if (isFirstChange)//第一次改变动画状态,防止和手动控制角色动画播放冲突
            {
                GetComponent<PlayerAnimationControl>().SetAnimation_RunToIdle();
                isFirstChange = false;
            }
        }
	}


    #region 逻辑方法
    public void OpenPlayerAutoMove(GameObject target)
    {
        Vector3 offest = Vector3.one * 0.1f;
        if (!isPlus)
        {
            offest = -1f * offest;
            isPlus = false;
        }
        agent.SetDestination(target.transform.position+offest);
        agent.isStopped = false;
        isFirstChange = true;
        AutoMoveTarget = target;
    }
    public void OffPlayerAutoMove()
    {
        agent.isStopped = true;
        Vector3 v = GameControl.Instance.playerAutoMove.GetComponent<Rigidbody>().velocity;
        GameControl.Instance.playerAutoMove.GetComponent<Rigidbody>().velocity = new Vector3(0, v.y, 0);
    }
    #endregion

}

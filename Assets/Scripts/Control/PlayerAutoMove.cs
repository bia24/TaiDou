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

    
	// Use this for initialization
	void Start () {
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
                OffPlayerAutoMove();
                //此时调用npc对话框弹出
                switch (AutoMoveTarget.tag) {
                    case "NPC":
                        ((UIManager_02NoviceVillage)GameControl.Instance.mUIManager).OnPlayerReachedNpc();
                        break;
                    case "NPC_RaidEntry":
                        break;
                }
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
        agent.SetDestination(target.transform.position);
        agent.isStopped = false;
        isFirstChange = true;
        AutoMoveTarget = target;
    }
    public void OffPlayerAutoMove()
    {
        agent.isStopped = true;
    }
    #endregion

}

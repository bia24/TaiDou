using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour {
    public List<GameObject> enemys;
    public List<Transform> spawnPos;
    public float delaySpawnTime=1f;
    public float spawnRate = 2f;
    private bool isFisrtEnter = true;
    public int spawnTimes = 3;
    private void Start()
    {
        isFisrtEnter = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameControl.Instance.raidType == RaidType.Person)
        {
            if (other.gameObject.tag == "Player")
            {
                if (isFisrtEnter)
                {
                    StartCoroutine(SpawnEnemy());
                    isFisrtEnter = false;
                }
            }
        }
        else if(GameControl.Instance.raidType==RaidType.Team)
        {
            if (other.gameObject.tag == "Player" && RaidManager.Instance.isMaster)
            {
                if (isFisrtEnter)
                {
                    StartCoroutine(SpawnEnemy());
                    isFisrtEnter = false;
                }
            }
        }
        else
        {
            return;
        }
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(delaySpawnTime);
        
        for(int i = 0; i < spawnTimes; i++)
        {
            List<EnemyCreateModel> enemies = new List<EnemyCreateModel>();
            foreach (Transform t in spawnPos)
            {
                int index = UnityEngine.Random.Range(0, enemys.Count);
                GameObject go = enemys[index];
                GameObject e= Instantiate(go, t.position, Quaternion.identity);
                string guid = null;
                if (e.tag == "Boss")
                {
                    e.GetComponent<Boss>().guid = Guid.NewGuid().ToString();
                    guid = e.GetComponent<Boss>().guid;
                }
                else
                {
                    e.GetComponent<Enemy>().guid = Guid.NewGuid().ToString();
                    guid = e.GetComponent<Enemy>().guid;
                }
                RaidManager.Instance.enemys.Add(e);
                //创建EnemyModel
                EnemyCreateModel model = new EnemyCreateModel();
                model.Name = go.name;
                model.Position = new TaidouCommon.Model.Position(t.position.x, t.position.y, t.position.z);
                model.Guid = guid;
                enemies.Add(model);
            }
            //向服务器发送敌人生成请求
            RaidManager.Instance.SendEnemyCreateToServer(enemies);
            yield return new WaitForSeconds(spawnRate);
        }
    }
}

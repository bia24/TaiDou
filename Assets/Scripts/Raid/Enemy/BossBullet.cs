using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour {
    public GameObject player;
    public float destroyTime = 10f;
    public float speed = 2f;
    private float timer = 0;
    public int Damage { get; set; }
    public float attackRate = 0.25f;
    public float force = 5f;
    private List<GameObject> isAttackingGo = new List<GameObject>();
	// Use this for initialization
	void Start () {
       // player = RaidManager.Instance.Player;
        Vector3 target = player.transform.position;
        target.y = transform.position.y;
        transform.LookAt(target);
        timer = destroyTime;
        InvokeRepeating("Attack", 0, attackRate);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(transform.forward*Time.deltaTime*speed, Space.World);
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!isAttackingGo.Contains(other.gameObject))
            {
                isAttackingGo.Add(other.gameObject);
            }
        }
    }

   private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isAttackingGo.Contains(other.gameObject))
            {
                isAttackingGo.Remove(other.gameObject);
            }
        }
    }

    private void Attack()
    {
        if (isAttackingGo.Count != 0)
        {
            foreach (var go in isAttackingGo)
            {
                go.GetComponent<PlayerAttackManager>().GetDamage(Damage);
                go.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Force);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int health = 500;
    public GameObject deathEffect;
    public bool isInvulnerable = false;
    public Transform target;
    public float maxDistance = 10f;
    private Animator anim;

    void Start()
    {
        
        anim = GetComponent<Animator>();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }
    void Update()
    {       
        CheckDistanceToTarget();    
    }
    void CheckDistanceToTarget()
    {

        if (target != null)
        {
            
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            
            if (distanceToTarget <= maxDistance)
            {
                if (anim != null)
                {
                    anim.SetBool("isNear", true);
                }
            }
            else
            {
                if (anim != null)
                {
                    anim.SetBool("isNear", false);
                }
            }
        }
    }
    public void TakeDamage(int damage)
    {
        if (isInvulnerable)
            return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
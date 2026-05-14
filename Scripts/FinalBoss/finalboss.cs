using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class finalboss : Fighter
	{
    enum States { patrol, pursuit }

    [SerializeField]
    States state = States.patrol;
    [SerializeField]
    float searchRange = 10;
    [SerializeField]
    float stoppingDistance = 3;
    [SerializeField]
    float AttackCoolDown = 2;
    [SerializeField] private float vida;
    Transform player;
    Vector3 target;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("SetTarget", 0, 5);
        InvokeRepeating("SendAttack", 0, AttackCoolDown); //se designan los parametros para el rastreo y cd de ataque del jefe
    }

    void SendAttack()
    {
        if(state != States.pursuit)
        {
            return;
        }
        if(vel.magnitude != 0)
        {
            return;
        }
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("golpe") && !anim.GetCurrentAnimatorStateInfo(0).IsName("muerte"))
        {
            anim.SetTrigger("isGolpe");
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRange);
        Gizmos.DrawWireSphere(target, 0.2f);
    }

    void SetTarget()
    {
        if (state != States.patrol)
            return;
        target = new Vector2(transform.position.x + Random.Range(-searchRange, searchRange), Random.Range(LimitsY.y, LimitsY.x));//se da un rango para seguir al protagonista
    }

    Vector2 vel;
    void Update()
    {
        if (state == States.pursuit)// se dan los parametros del modo patrulla y seguimiento respectivamente
        {
            target = player.transform.position;
            if (Vector3.Distance(target, transform.position) < searchRange * 1.2f)
            {
                target = transform.position;
                state = States.patrol;
                return;
            }
        }
        else if (state == States.patrol)
        {
            var ob = Physics2D.CircleCast(transform.position, searchRange, Vector2.up);
            if (ob.collider != null)
            {
                if (ob.collider.CompareTag("Player"))
                {
                    state = States.pursuit;
                    return;
                }
            }
        }
        vel = target - transform.position;
        sr.flipX = vel.x < 0;
        if(vel.magnitude < stoppingDistance)
            vel =-Vector2.zero; 
        vel.Normalize();
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("golpe") && !anim.GetCurrentAnimatorStateInfo(0).IsName("muerte"))
        {
            anim.SetBool("isWalking", vel.magnitude != 0);
        }
        else
        {
            vel = Vector2.zero;
        }
       rb.linearVelocity = new Vector2(vel.x * horizontalSpeed, vel.y * verticalSpeed);

    }
     public void TomarDano(float dano)// se establecen los parametros para tomar dano 
    {
        vida-=dano;
        if(vida>0)
        {
            anim.SetTrigger("getPunch");
        }
        if(vida<=0)
        {
            anim.SetTrigger("isMuerto");
            GetComponent<CapsuleCollider2D>().enabled=false;
            Destroy(gameObject,5);
            SceneManager.LoadScene(7);
        }
    }
}

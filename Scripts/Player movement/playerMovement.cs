using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerMovement : Fighter
{
    [SerializeField] private float vida;
    [SerializeField] private float vidaMaxima;
    [SerializeField] private BarraVida BarraVida;
    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    Transform firePoint;

    Vector2 cntrl;

    // =========================
    // ATAQUE AUTOMATICO
    // =========================

    [SerializeField]
    float rangoAtaque = 10f;

    [SerializeField]
    float danoAtaque = 5f;

    void Start()
    {
        vida = vidaMaxima;
        BarraVida.inicioBarra(vida);
    }

    void Update()
    {
        cntrl = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (cntrl.x != 0)
        {
            sr.flipX = cntrl.x < 0;
        }

       if (Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetTrigger("sendPunch");

            DispararProyectil();
        }

        anim.SetBool("isDefense", Input.GetKey(KeyCode.X));

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Punch") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("GetPunch") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Defense"))
        {
            anim.SetBool("isWalking", cntrl.magnitude != 0);

            // CORREGIDO
            rb.linearVelocity = new Vector2(
                cntrl.x * horizontalSpeed,
                cntrl.y * verticalSpeed
            );

            transform.position = new Vector3(
                transform.position.x,
                Mathf.Clamp(transform.position.y, LimitsY.y, LimitsY.x),
                transform.position.z
            );
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void AtacarEnemigoMasCercano()
{
    Enemy[] enemigos = FindObjectsOfType<Enemy>();

    Enemy enemigoMasCercano = null;

    float distanciaMasCercana = rangoAtaque;

    foreach (Enemy enemigo in enemigos)
    {
        if (enemigo == null)
            continue;

        float distancia =
            Vector2.Distance(
                transform.position,
                enemigo.transform.position
            );

        if (distancia < distanciaMasCercana)
        {
            distanciaMasCercana = distancia;
            enemigoMasCercano = enemigo;
        }
    }

    if (enemigoMasCercano != null)
    {
        enemigoMasCercano.TomarDano(danoAtaque);

        Debug.Log(
            "Golpeado enemigo: "
            + enemigoMasCercano.name
        );
    }
}

    public void TomarDano(float dano)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Defense"))
        {
            vida -= dano;

            BarraVida.vidaActual(vida);

            Debug.Log("Player recibe daño: " + dano);

            if (vida > 0)
            {
                anim.SetTrigger("getPunch");
            }

            // CORREGIDO
            if (vida <= 0)
            {
                anim.SetTrigger("Death");

                Destroy(gameObject, 1.8f);

                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().buildIndex
                );
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            rangoAtaque
        );
    }

    void DispararProyectil()
    {
        Enemy enemigo = BuscarEnemigoMasCercano();

        if(enemigo == null)
            return;

        GameObject proyectil =
            Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

        PlayerProjectile playerProjectile =
            proyectil.GetComponent<PlayerProjectile>();

        if(playerProjectile != null)
        {
            Vector2 direccion =
                (enemigo.transform.position -
                firePoint.position).normalized;

            playerProjectile.SetDirection(direccion);
        }
    }

    Enemy BuscarEnemigoMasCercano()
    {
        Enemy[] enemigos = FindObjectsOfType<Enemy>();

        Enemy enemigoMasCercano = null;

        float distanciaMasCercana = rangoAtaque;

        foreach (Enemy enemigo in enemigos)
        {
            float distancia =
                Vector2.Distance(
                    transform.position,
                    enemigo.transform.position
                );

            if (distancia < distanciaMasCercana)
            {
                distanciaMasCercana = distancia;
                enemigoMasCercano = enemigo;
            }
        }

        return enemigoMasCercano;
    }
}
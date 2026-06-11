using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Fighter
{
    // =========================
    // VARIABLE GLOBAL
    // CONTROLA SI ALGUN ENEMIGO
    // ESTA ATACANDO ACTUALMENTE
    // =========================
    static bool enemigoAtacando = false;
    bool estaMuerto = false;

    // =========================
    // ESTADOS DEL ENEMIGO
    // =========================
    enum States
    {
        patrol,
        pursuit,
        retreat
    }

    [SerializeField]
    States state = States.patrol;

    [SerializeField]
    float searchRange = 10;

    [SerializeField]
    float stoppingDistance = 3;

    [SerializeField]
    float AttackCoolDown = 2;

    // =========================
    // CONFIGURACION DEL ATAQUE
    // =========================

    [SerializeField]
    float tiempoAtaque = 2f;

    // =========================
    // CONFIGURACION DEL RETROCESO
    // =========================

    // ahora el enemigo apenas
    // da un pequeño paso atras
    [SerializeField]
    float retreatSpeed = 0.8f;

    [SerializeField]
    float retreatTime = 0.18f;

    bool isRetreating = false;

    // =========================
    // CONTROL ATAQUE
    // =========================

    // evita moverse mientras ataca
    bool estaAtacando = false;

    // =========================
    // RESISTENCIA EMPUJES
    // =========================

    // ayuda a que no puedan
    // arrastrarlo facilmente
    [SerializeField]
    float resistenciaEmpuje = 6f;

    // =========================
    // DISTANCIA ENTRE ENEMIGOS
    // =========================

    [SerializeField]
    float separacionAliados = 2f;

    // =========================
    // FORMACION ROMANA
    // =========================

    [SerializeField]
    bool usarFormacion = true;

    [SerializeField]
    float distanciaFormacion = 1.5f;

    [SerializeField]
    int anchoFormacion = 3;

    // =========================
    // COMPORTAMIENTO ESCUDO
    // =========================

    [SerializeField]
    bool unidadEscudo = true;

    [SerializeField]
    Transform lider;

    [SerializeField]
    float rangoProteccion = 8f;

    // =========================
    // EVITAR OBSTACULOS
    // =========================

    // distancia para detectar obstaculos
    [SerializeField]
    float distanciaDeteccion = 1.5f;

    // fuerza para rodearlos
    [SerializeField]
    float fuerzaEsquive = 2f;

    // capas consideradas obstaculos
    [SerializeField]
    LayerMask obstacleMask;

    // =========================
    // DASH ESPECIAL
    // =========================

    [SerializeField]
    float probabilidadDash = 0.2f;

    [SerializeField]
    float velocidadDash = 15f;

    [SerializeField]
    float tiempoDash = 0.5f;

    [SerializeField]
    ParticleSystem dashParticles;

    // =========================
    // PROYECTIL
    // =========================

    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    Transform firePoint;

    // =========================
    // VIDA DEL ENEMIGO
    // =========================

    [SerializeField]
    private float vidaMaxima = 100;

    private float vida;

    [SerializeField]
    private RectTransform barraVida;

    private float anchoOriginalBarra;

    // =========================

    Transform player;

    Vector3 target;

    Vector2 vel;

    // =========================
    // DETECCION DEL JUGADOR
    // =========================

    [SerializeField]
    float rangoDeteccion = 10f;

    [SerializeField]
    float tiempoPerdidaObjetivo = 2f;

    float temporizadorPerdida = 0f;

    void Start()
    {
        // inicializa vida
        vida = vidaMaxima;
        anchoOriginalBarra = barraVida.sizeDelta.x;

        // actualiza barra
        ActualizarBarraVida();

        // obtiene jugador
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // patrulla
        InvokeRepeating("SetTarget", 0, 5);

        // cooldown ataque
        InvokeRepeating(
            "SendAttack",
            Random.Range(0f, 1f),
            AttackCoolDown
        );

        // =========================
        // RESISTENCIA EMPUJES
        // =========================

        // evita ser arrastrado facilmente
        rb.linearDamping = resistenciaEmpuje;

        // evita rotaciones raras
        rb.freezeRotation = true;
    }

    void SendAttack()
    {
        // solo ataca persiguiendo
        if (state != States.pursuit)
        {
            return;
        }

        // evita ataques dobles
        if (estaMuerto)
        {
            return;
        }

        if (state == States.retreat)
        {
            return;
        }

        if (rb.linearVelocity.magnitude > 0.1f)
        {
            return;
        }

        if (estaAtacando)
        {
            return;
        }

        // si otro enemigo ataca
        // este se aparta
        if (enemigoAtacando)
        {
            StartCoroutine(Retroceder());

            return;
        }

        // distancia al jugador
        float distanciaJugador =
            Vector2.Distance(
                transform.position,
                player.position
            );

        // demasiado lejos
        //if (distanciaJugador > stoppingDistance + 0.5f)
        //{
        //    return;
        //}
            if (distanciaJugador > 8f)
            {
                return;
            }

        // =========================
        // INICIA ATAQUE
        // =========================

        estaAtacando = true;

        enemigoAtacando = true;

        // detiene movimiento
        rb.linearVelocity = Vector2.zero;

        vel = Vector2.zero;

        // desactiva caminar
        anim.SetBool("isWalking", false);

        // dispara proyectil
        DispararProyectil();

        StartCoroutine(EsperarRetroceso());
    }

    // =========================
    // ESPERA Y RETROCEDE
    // =========================
    IEnumerator EsperarRetroceso()
    {
        // espera duracion ataque
        yield return new WaitForSeconds(tiempoAtaque);

        enemigoAtacando = false;

        estaAtacando = false;

        float random = Random.Range(0f, 1f);

        if(random <= probabilidadDash)
        {
            StartCoroutine(DashEspecial());
        }
        else
        {
            StartCoroutine(Retroceder());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, searchRange);

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, separacionAliados);

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, rangoProteccion);

        // deteccion obstaculos
        Gizmos.color = Color.green;

        Gizmos.DrawRay(
            transform.position,
            transform.right * distanciaDeteccion
        );
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(
            transform.position,
            rangoDeteccion
        );
    }

    void SetTarget()
    {
        if (state != States.patrol)
            return;

        // patrulla aleatoria
        target = new Vector2(
            transform.position.x + Random.Range(-searchRange, searchRange),
            Random.Range(LimitsY.y, LimitsY.x)
        );
    }

    void Update()
    {

        if (estaMuerto)
            {
                rb.linearVelocity = Vector2.zero;

                anim.SetBool("isWalking", false);

                return;
            }
        // =========================
        // DETECCION JUGADOR
        // =========================

        if(player != null)
        {
            float distanciaJugador =
                Vector2.Distance(
                    transform.position,
                    player.position
                );

            // entra en persecucion
            if(distanciaJugador <= rangoDeteccion)
            {
                state = States.pursuit;
                temporizadorPerdida = 0f;
            }
            // sale del rango
            else if(state == States.pursuit)
            {
                temporizadorPerdida += Time.deltaTime;

                if(temporizadorPerdida >= tiempoPerdidaObjetivo)
                {
                    state = States.patrol;

                    temporizadorPerdida = 0f;

                    SetTarget();
                }
            }
        }
        // =========================
        // SI RETROCEDE
        // =========================
        if (state == States.retreat)
        {
            anim.SetBool("isWalking", true);

            if(rb.linearVelocity.x != 0)
            {
                sr.flipX = rb.linearVelocity.x < 0;
            }

            return;
        }

        // =========================
        // SI ESTA ATACANDO
        // =========================

        if (estaAtacando)
        {
            // inmovil durante ataque
            rb.linearVelocity = Vector2.zero;

            // evita animacion caminar
            anim.SetBool("isWalking", false);

            // mira jugador
            if(player != null)
            {
                sr.flipX =
                    player.position.x < transform.position.x;
            }

            return;
        }

        // =========================
        // PERSECUCION
        // =========================
        if (state == States.pursuit)
        {
            // sigue jugador
            target = player.transform.position;

            // =========================
            // FORMACION ROMANA
            // =========================

            if (usarFormacion)
            {
                Enemy[] enemigos = FindObjectsOfType<Enemy>();

                int indice = 0;

                foreach (Enemy enemigo in enemigos)
                {
                    if (enemigo == this)
                    {
                        break;
                    }

                    indice++;
                }

                // calcula fila y columna
                int fila = indice / anchoFormacion;
                int columna = indice % anchoFormacion;

                // offsets
                float offsetX =
                    (columna - (anchoFormacion / 2f))
                    * distanciaFormacion;

                float offsetY =
                    fila * distanciaFormacion;

                Vector3 offset =
                    new Vector3(offsetX, offsetY, 0);

                target += offset;
            }

            // =========================
            // PROTEGER LIDER
            // =========================

            if(unidadEscudo && lider != null)
            {
                float distanciaLider =
                    Vector2.Distance(transform.position, lider.position);

                if(distanciaLider < rangoProteccion)
                {
                    // se posiciona delante
                    // del lider
                    Vector2 direccionJugador =
                        (player.position - lider.position).normalized;

                    Vector2 posicionEscudo =
                        (Vector2)lider.position +
                        direccionJugador * 2f;

                    target = posicionEscudo;
                }
            }
        }

        // =========================
        // MOVIMIENTO BASE
        // =========================

        vel = target - transform.position;

        // =========================
        // EVITAR OBSTACULOS
        // Y ALIADOS
        // =========================

        // raycast frontal
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            vel.normalized,
            distanciaDeteccion,
            obstacleMask
        );

        // si detecta algo
        if(hit.collider != null)
        {
            // ignorar jugador
            if(!hit.collider.CompareTag("Player"))
            {
                // =========================
                // CALCULA DOS DIRECCIONES
                // =========================

                // izquierda
                Vector2 izquierda =
                    Quaternion.Euler(0, 0, 45)
                    * vel.normalized;

                // derecha
                Vector2 derecha =
                    Quaternion.Euler(0, 0, -45)
                    * vel.normalized;

                // revisa izquierda
                RaycastHit2D hitIzquierda =
                    Physics2D.Raycast(
                        transform.position,
                        izquierda,
                        distanciaDeteccion,
                        obstacleMask
                    );

                // revisa derecha
                RaycastHit2D hitDerecha =
                    Physics2D.Raycast(
                        transform.position,
                        derecha,
                        distanciaDeteccion,
                        obstacleMask
                    );

                // =========================
                // ELIGE EL CAMINO LIBRE
                // =========================

                // izquierda libre
                if(hitIzquierda.collider == null)
                {
                    vel = izquierda;
                }

                // derecha libre
                else if(hitDerecha.collider == null)
                {
                    vel = derecha;
                }

                // ambos bloqueados
                else
                {
                    // retrocede un poco
                    vel = -vel.normalized;
                }
            }
        }

        // =========================
        // SEPARACION ALIADOS
        // =========================

        Enemy[] aliados = FindObjectsOfType<Enemy>();

        foreach (Enemy enemigo in aliados)
        {
            if (enemigo == this)
                continue;

            float distancia =
                Vector2.Distance(
                    transform.position,
                    enemigo.transform.position
                );

            // evita amontonamiento
            if (distancia < separacionAliados)
            {
                Vector2 separacion =
                    (transform.position -
                    enemigo.transform.position).normalized;

                vel += separacion;
            }
        }

        // =========================
        // DETENERSE CERCA
        // =========================

        float distanciaObjetivo =
            Vector2.Distance(transform.position, target);

        if (distanciaObjetivo < stoppingDistance)
        {
            vel = Vector2.zero;
        }

        // normaliza
        vel.Normalize();

        // =========================
        // MOVIMIENTO FINAL
        // =========================

        Vector2 movimientoFinal = new Vector2(
            vel.x * horizontalSpeed,
            vel.y * verticalSpeed
        );

        rb.linearVelocity = movimientoFinal;

        // =========================
        // ANIMACIONES
        // =========================

        bool estaMoviendose =
            rb.linearVelocity.magnitude > 0.1f;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Death "))
        {
            anim.SetBool("isWalking", estaMoviendose);
        }
        else
        {
            anim.SetBool("isWalking", false);

            vel = Vector2.zero;
        }

        // =========================
        // MIRAR AL JUGADOR
        // =========================

        if(player != null)
        {
            sr.flipX =
                player.position.x < transform.position.x;
        }
    }

    // =========================
    // RETROCESO NORMAL
    // =========================
    IEnumerator Retroceder()
    {
        if (isRetreating)
            yield break;

        isRetreating = true;

        state = States.retreat;

        // direccion opuesta jugador
        Vector2 direccionRetroceso =
            (transform.position - player.position).normalized;

        float tiempo = 0;

        while (tiempo < retreatTime)
        {
            rb.linearVelocity =
                direccionRetroceso * retreatSpeed;

            tiempo += Time.deltaTime;

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        state = States.pursuit;

        isRetreating = false;
    }

    // =========================
    // DASH ESPECIAL
    // =========================
    IEnumerator DashEspecial()
    {
        if (isRetreating)
            yield break;

        isRetreating = true;

        state = States.retreat;

        if(dashParticles != null)
        {
            dashParticles.Play();
        }

        // dash agresivo
        Vector2 direccionDash =
            (player.position - transform.position).normalized;

        float tiempo = 0;

        while (tiempo < tiempoDash)
        {
            rb.linearVelocity =
                direccionDash * velocidadDash;

            tiempo += Time.deltaTime;

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        state = States.pursuit;

        isRetreating = false;
    }

    // =========================
    // DISPARAR PROYECTIL
    // =========================
    void DispararProyectil()
    {
        if (player == null)
            return;

        if (projectilePrefab == null)
            return;

        if (firePoint == null)
            return;

        GameObject proyectil =
            Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

        EnemyProjectile enemyProjectile =
            proyectil.GetComponent<EnemyProjectile>();

        if(enemyProjectile != null)
        {
            Vector2 direccion =
                (player.position - firePoint.position).normalized;

            enemyProjectile.SetDirection(direccion);
        }
    }

    // =========================
    // ACTUALIZA VIDA
    // =========================
   void ActualizarBarraVida()
{
    float porcentaje = vida / vidaMaxima;

    Vector2 nuevoTamano = barraVida.sizeDelta;

    nuevoTamano.x = anchoOriginalBarra * porcentaje;

    barraVida.sizeDelta = nuevoTamano;
}

    // =========================
    // RECIBIR DAÑO
    // =========================
    public void TomarDano(float dano)
    {
        if (estaMuerto)
            return;

        vida -= dano;

        ActualizarBarraVida();

        if (vida <= 0)
        {
            estaMuerto = true;

            enemigoAtacando = false;

            CancelInvoke();

            StopAllCoroutines();

            state = States.patrol;

            rb.linearVelocity = Vector2.zero;

            vel = Vector2.zero;

            anim.SetBool("isWalking", false);

            anim.SetTrigger("Death");

            Destroy(gameObject, 1.3f);
        }
    }
}
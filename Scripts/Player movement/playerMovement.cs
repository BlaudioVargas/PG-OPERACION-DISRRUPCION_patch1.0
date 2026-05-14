using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class playerMovement : Fighter
{
    [SerializeField] private float vida;
    [SerializeField] private float vidaMaxima;
    [SerializeField]private BarraVida BarraVida;
    Vector2 cntrl;
    void Start()
    {
        vida=vidaMaxima;
        BarraVida.inicioBarra(vida);
    }

    void Update()
    {
        cntrl = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));

        if(cntrl.x != 0){
            sr.flipX = cntrl.x < 0;
        }

        if (Input.GetKeyDown(KeyCode.Z))  // se asignan el control de ataque
        {
            anim.SetTrigger("sendPunch"); 
        }
        anim.SetBool("isDefense", Input.GetKey(KeyCode.X));// se asigna el control de defensa 


        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Punch")&& !anim.GetCurrentAnimatorStateInfo(0).IsName("GetPunch")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Defense")) // se comprueba que otra animacion no este siendo ejecutada
        {
        anim.SetBool("isWalking", cntrl.magnitude != 0); // se asigna los controles y limites para el movimiento
        rb.linearVelocity = new Vector2(cntrl.x*horizontalSpeed, cntrl.y * verticalSpeed);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, LimitsY.y,LimitsY.x),transform.position.z);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }  
        
    }
     public void TomarDano(float dano) // scripts para tomar daño
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Defense"))  // se comprueba que el personaje no se encuentre bloqueando
        { 
         vida-=dano;                                                 //se resta el daño  a la vida del jugador
         BarraVida.vidaActual(vida);
        if(vida>0){ 
        anim.SetTrigger("getPunch");}

        if(vida==0) //se comprueba que la vida del jugador sea igual a 0
        {
            
            anim.SetTrigger("Death"); //se inicia la animacion de muerte y se acaba el juego
            Destroy(gameObject,1.8f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    
        } 
    }
   
}

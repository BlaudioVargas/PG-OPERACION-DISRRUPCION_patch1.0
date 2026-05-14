using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaludCombate : MonoBehaviour
{
   [SerializeField] private float vida;
   [SerializeField] private float vidaMaxima;
   [SerializeField]private BarraVida BarraVida;
   [SerializeField]private movements player;
    void Start()
    {
        vida=vidaMaxima;
        BarraVida.inicioBarra(vida);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TomarDano(float dano) // si la vida del jugador baja a 0 se inica la animacion de muerte
    {
        vida-=dano;
        BarraVida.vidaActual(vida);
        if(vida<=0)
        {
            player.GetComponent<Animator>().SetTrigger("Death");
            Destroy(gameObject);
        }
    
    }
    
}

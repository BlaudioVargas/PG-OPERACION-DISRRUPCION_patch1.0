using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitboxplayer : MonoBehaviour
{
    // este metodo se ejecuta cuando el hitbox entra en contacto
    // con otro collider marcado como trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // =========================
        // ENEMIGOS NORMALES
        // =========================
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TomarDano(5);
        }

        // =========================
        // SUBJEFES
        // =========================
        else if(other.CompareTag("Subjefe"))
        {
            other.GetComponent<subjefe>().TomarDano(20);
        }

        else if(other.CompareTag("Subjefe 2"))
        {
            other.GetComponent<subjefe2>().TomarDano(20);
        }

        else if(other.CompareTag("Subjefe 3"))
        {
            other.GetComponent<subjefe3>().TomarDano(20);
        }

        else if(other.CompareTag("Subjefe 4"))
        {
            other.GetComponent<subjefe4>().TomarDano(20);
        }

        // =========================
        // JEFE FINAL
        // =========================
        else if(other.CompareTag("Final Boss"))
        {
            other.GetComponent<finalboss>().TomarDano(25);
        }
    }
}
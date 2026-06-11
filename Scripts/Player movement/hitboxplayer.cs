using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitboxplayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Golpe detectado con: " + other.name);

        // =========================
        // ENEMIGOS NORMALES
        // =========================
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TomarDano(5);
                Debug.Log("Daño aplicado a Enemy");
            }
        }

        // =========================
        // SUBJEFES
        // =========================
        else if (other.CompareTag("Subjefe"))
        {
            subjefe boss = other.GetComponentInParent<subjefe>();

            if (boss != null)
            {
                boss.TomarDano(20);
            }
        }

        else if (other.CompareTag("Subjefe 2"))
        {
            subjefe2 boss = other.GetComponentInParent<subjefe2>();

            if (boss != null)
            {
                boss.TomarDano(20);
            }
        }

        else if (other.CompareTag("Subjefe 3"))
        {
            subjefe3 boss = other.GetComponentInParent<subjefe3>();

            if (boss != null)
            {
                boss.TomarDano(20);
            }
        }

        else if (other.CompareTag("Subjefe 4"))
        {
            subjefe4 boss = other.GetComponentInParent<subjefe4>();

            if (boss != null)
            {
                boss.TomarDano(20);
            }
        }

        // =========================
        // JEFE FINAL
        // =========================
        else if (other.CompareTag("Final Boss"))
        {
            finalboss boss = other.GetComponentInParent<finalboss>();

            if (boss != null)
            {
                boss.TomarDano(25);
            }
        }
    }
}
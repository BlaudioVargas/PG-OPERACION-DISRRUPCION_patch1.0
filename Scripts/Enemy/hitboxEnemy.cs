using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitboxEnemy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement player =
                other.GetComponentInParent<playerMovement>();

            if (player != null)
            {
                player.TomarDano(20);

                Debug.Log("Enemy golpeó al Player");
            }
        }
    }
}
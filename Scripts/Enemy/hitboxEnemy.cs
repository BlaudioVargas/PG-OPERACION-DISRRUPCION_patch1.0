using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitboxEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     private void OnCollisionEnter2D(Collision2D other) // se designa el daño recibido al ser atacado
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<playerMovement>().TomarDano(20);
        }
    }
}

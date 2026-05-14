using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class OrderInLayer : MonoBehaviour
{
    
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    
    void Update()
    {
        sr.sortingOrder = -(int)(transform.position.y * 100);
    }
}

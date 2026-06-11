using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OrderInLayer : MonoBehaviour
{
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
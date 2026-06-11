using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;

    [SerializeField]
    float damage = 15f;

    [SerializeField]
    float lifeTime = 1f;

    Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("PlayerProjectile impactó: " + other.name);

        Enemy enemy =
            other.GetComponentInParent<Enemy>();

        if(enemy != null)
        {
            Debug.Log("Daño aplicado al enemigo");

            enemy.TomarDano(damage);

            Destroy(gameObject);

            return;
        }

        if(other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
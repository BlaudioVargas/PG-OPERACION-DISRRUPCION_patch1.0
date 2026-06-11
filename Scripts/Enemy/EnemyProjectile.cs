using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;

    [SerializeField]
    float damage = 10f;

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
        playerMovement player =
            other.GetComponentInParent<playerMovement>();

        if(player != null)
        {
            player.TomarDano(damage);
            Destroy(gameObject);
        }
    }
}
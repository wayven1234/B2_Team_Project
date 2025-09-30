using UnityEngine;

public class Stage2Bullet : MonoBehaviour
{
    public float moveSpeed;
    public float damage;

    void Update()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Stage2Enemy enemy = collision.GetComponent<Stage2Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}

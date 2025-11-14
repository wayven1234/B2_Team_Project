using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private PlayerController _player;

    [SerializeField] private GameObject _expPrefab;

    public float maxHealth;
    public float currentHealth;

    public float moveSpeed = 1f; // 이동 속도

    public float damage;

    private Transform player;    // 플레이어 Transform
    private Rigidbody2D rb;      // Rigidbody2D 참조

    private void Start()
    {
        currentHealth = maxHealth;

        // "Player" 태그를 붙은 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // 플레이어까지의 방향 벡터 구하기
            Vector2 direction = (player.position - transform.position).normalized;

            // Rigidbody2D를 이용한 이동
            rb.linearVelocity = direction * moveSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collison)
    {
        if (collison.gameObject.tag == "Player")
        {
            //_player.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Current HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Instantiate(_expPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}

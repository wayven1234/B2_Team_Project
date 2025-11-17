using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PlayerController player;

    [SerializeField] private GameObject _expPrefab;
    [SerializeField] private string playerTag = "Player";

    public float maxHealth; 
    public float currentHealth;

    public float moveSpeed = 1f; // 이동 속도

    public float damage;

    private Rigidbody2D rb;      // Rigidbody2D 참조

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            if (GameManager.instance.currentGameState != GameState.Playing)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            player = PlayerController.instance;

            if (player == null)
                return;

            Debug.Log(gameObject.name + ": Player 추적 시작");
        }

        if (GameManager.instance.currentGameState != GameState.Playing)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 플레이어까지의 방향 벡터 구하기
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Rigidbody2D를 이용한 이동
        rb.linearVelocity = direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            if (player != null)
            {
                player.TakeDamage(damage);
            }
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

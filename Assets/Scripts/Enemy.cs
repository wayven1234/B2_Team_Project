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
        if (GameManager.instance != null && GameManager.instance.currentGameState != GameState.Playing)
        {
            // 게임이 멈췄다면 적도 움직임을 멈춥니다.
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            return;
        }

        // 2. Player 인스턴스 참조 확인 및 획득
        if (player == null)
        {
            player = PlayerController.instance;

            // 플레이어가 파괴되었거나 아직 찾을 수 없다면 더 이상 움직이지 않습니다.
            if (player == null)
            {
                if (rb != null)
                    rb.linearVelocity = Vector2.zero; // 멈춤 처리
                return;
            }

            Debug.Log(gameObject.name + ": Player 추적 시작");
        }

        // 3. 플레이어 추적 및 이동 (Playing 상태일 때만 실행됨)
        // 플레이어까지의 방향 벡터 구하기
        Vector2 direction = (player.transform.position - transform.position).normalized;

        // Rigidbody2D를 이용한 이동
        if (rb != null)
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
            Debug.Log("Enemy Die");
        }
    }

    void Die()
    {
        Instantiate(_expPrefab, transform.position, Quaternion.identity);

        if (EnemyManager.instance != null)
            EnemyManager.instance.EnemyKilled();

        Destroy(gameObject);
    }
}

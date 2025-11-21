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

    [Header("추적 설정")]
    public float stoppingDistance = 0.5f;

    public float separationRadius = 0.5f; // 다른 적을 감지할 반경
    public float separationForce = 3f;    // 밀어내는 힘의 세기 (클수록 강하게 밀어냄)

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

        Vector2 targetPosition = player.transform.position;
        Vector2 currentPosition = transform.position;
        Vector2 toPlayer = targetPosition - currentPosition;
        float distance = toPlayer.magnitude;

        Vector2 finalMoveVector = Vector2.zero;

        // 3. 플레이어 추적 및 이동 (Playing 상태일 때만 실행됨)
        // 플레이어까지의 방향 벡터 구하기
        if (distance > stoppingDistance)
        {
            // 멈춤 거리에 도달하지 않았다면 플레이어 쪽으로 이동 벡터를 추가
            Vector2 directionToPlayer = toPlayer.normalized;
            finalMoveVector += directionToPlayer * moveSpeed;
        }

        Vector2 separationVector = CalculateSeparationForce();
        finalMoveVector += separationVector;

        if (finalMoveVector.magnitude > moveSpeed)
        {
            // Vector2.ClampMagnitude를 사용하여 벡터의 최대 길이를 제한합니다.
            finalMoveVector = Vector2.ClampMagnitude(finalMoveVector, moveSpeed);
        }

        // C. Rigidbody2D를 이용한 최종 이동
        if (rb != null)
            rb.linearVelocity = finalMoveVector;
    }

    /// <summary>
    /// 주변의 다른 적들을 감지하고, 겹침을 피하기 위한 분리 힘(Vector)을 계산합니다.
    /// </summary>
    Vector2 CalculateSeparationForce()
    {
        Vector2 separation = Vector2.zero;

        // "Enemy" 레이어를 대상으로 separationRadius 반경 내의 콜라이더를 감지
        // Physics2D.OverlapCircleAll를 사용하려면 모든 Enemy 프리팹의 Layer를 "Enemy"로 설정해야 합니다.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, separationRadius, LayerMask.GetMask("Enemy"));

        foreach (var hit in hitColliders)
        {
            // 1. 자기 자신 제외
            if (hit.gameObject == gameObject) continue;

            // 2. 적 태그 확인 (Enemy 컴포넌트가 붙어있는지 확인하는 것이 더 안전할 수 있습니다.)
            if (hit.CompareTag("Enemy"))
            {
                Vector2 neighborPosition = (Vector2)hit.transform.position;
                Vector2 offset = (Vector2)transform.position - neighborPosition; // 밀어낼 방향 (자신 -> 이웃)
                float distance = offset.magnitude;

                if (distance > 0f)
                {
                    float strength = separationForce * (separationRadius - distance) / separationRadius;

                    // 정규화된 방향 벡터에 계산된 힘을 곱합니다.
                    separation += offset.normalized * strength;
                }
            }
        }

        // 분리 벡터를 반환합니다.
        return separation;
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

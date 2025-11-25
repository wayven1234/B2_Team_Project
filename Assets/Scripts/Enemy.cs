using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PlayerController player;

    [SerializeField] private string playerTag = "Player";

    public float maxHealth;
    public float currentHealth;

    public float moveSpeed = 1f; // 이동 속도
    public float damage;

    public StageData.StageType currentStageType = StageData.StageType.Normal;

    [Header("추적 설정")]
    public float stoppingDistance = 0.5f;

    public float separationRadius = 0.5f; // 다른 적을 감지할 반경
    public float separationForce = 3f;    // 밀어내는 힘의 세기

    [Header("공격 설정")]
    public float attackInterval = 1f;
    private float attackTimer;

    private Rigidbody2D rb;      // Rigidbody2D 참조

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        attackTimer = 0f;    // 공격 타이머 초기화
    }

    void FixedUpdate()
    {
        // 1. 공격 타이머 업데이트
        if (attackTimer > 0f) attackTimer -= Time.fixedDeltaTime;

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
        Vector2 finalMoveVector = Vector2.zero;

        // Wall 충돌 감지 플래그 (분리 힘 처리 시 사용)
        bool isAgainstWall = false;

        // 3. Stage Type에 따른 이동 로직 분기
        if (currentStageType == StageData.StageType.Vertical)
        {
            // Vertical Stage: 수평 이동 및 Wall 충돌 방지 로직
            float horizontalDirection = Mathf.Sign(toPlayer.x);
            Vector2 directionToTarget = new Vector2(horizontalDirection, 0f);

            int wallLayerMask = LayerMask.GetMask("Wall");
            // Enemy의 크기에 맞게 Raycast 거리를 조정 (예시: 0.6f)
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, directionToTarget, 0.6f, wallLayerMask);

            if (hit.collider == null)
            {
                finalMoveVector += directionToTarget * moveSpeed;
            }
            else
            {
                // Wall에 닿기 직전이라면 플래그 설정
                isAgainstWall = true;
            }
        }
        else // Normal Stage: 기존 전방위 추적 로직
        {
            float distance = toPlayer.magnitude;

            if (distance > stoppingDistance)
            {
                Vector2 directionToPlayer = toPlayer.normalized;
                finalMoveVector += directionToPlayer * moveSpeed;
            }
        }

        //// 4. 분리 힘 계산 및 합산 (두 모드 공통 적용)
        //Vector2 separationVector = CalculateSeparationForce();

        //if (isAgainstWall)
        //{
        //    // Wall에 닿았으므로 Y축 움직임은 차단하고 X축 분리 힘만 허용
        //    separationVector.y = 0f;
        //}

        //finalMoveVector += separationVector;

        // 5. 최종 이동 벡터 제한
        if (finalMoveVector.magnitude > moveSpeed)
        {
            // Vector2.ClampMagnitude를 사용하여 벡터의 최대 길이를 제한합니다.
            finalMoveVector = Vector2.ClampMagnitude(finalMoveVector, moveSpeed);
        }

        // C. Rigidbody2D를 이용한 최종 이동
        if (rb != null)
            rb.linearVelocity = finalMoveVector;
    }

    ///// <summary>
    ///// 주변의 다른 적들을 감지하고, 겹침을 피하기 위한 분리 힘(Vector)을 계산합니다.
    ///// </summary>
    //Vector2 CalculateSeparationForce()
    //{
    //    Vector2 separation = Vector2.zero;

    //    // "Enemy" 레이어를 대상으로 separationRadius 반경 내의 콜라이더를 감지
    //    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, separationRadius, LayerMask.GetMask("Enemy"));

    //    foreach (var hit in hitColliders)
    //    {
    //        // 1. 자기 자신 제외
    //        if (hit.gameObject == gameObject) continue;

    //        // 2. 적 태그 확인
    //        if (hit.CompareTag("Enemy"))
    //        {
    //            Vector2 neighborPosition = (Vector2)hit.transform.position;
    //            Vector2 offset = (Vector2)transform.position - neighborPosition; // 밀어낼 방향 (자신 -> 이웃)
    //            float distance = offset.magnitude;

    //            if (distance > 0f)
    //            {
    //                float strength = separationForce * (separationRadius - distance) / separationRadius;

    //                // 정규화된 방향 벡터에 계산된 힘을 곱합니다.
    //                separation += offset.normalized * strength;
    //            }
    //        }
    //    }

    //    return separation;
    //}

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            if (player != null && attackTimer <= 0f)
            {
                player.TakeDamage(damage);
                attackTimer = attackInterval; // 쿨타임 재설정
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
        if (PlayerController.instance != null)
        {
            PlayerController.instance.GainExperience();
        }

        if (EnemyManager.instance != null)
            EnemyManager.instance.EnemyKilled();

        Destroy(gameObject);
    }
}
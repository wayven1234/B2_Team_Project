using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PlayerController player;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string wallLayerName = "Wall";
    [SerializeField] private float wallAttackDistance = 0.6f;

    public float maxHealth;
    public float currentHealth;

    public float moveSpeed = 1f;
    public float damage;

    public StageData.StageType currentStageType = StageData.StageType.Normal;

    [Header("추적 설정")]
    public float stoppingDistance = 0.5f;

    public float separationRadius = 0.5f;
    public float separationForce = 3f;

    [Header("공격 설정")]
    public float attackInterval = 1f;
    private float attackTimer;

    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        attackTimer = 0f;
    }

    void FixedUpdate()
    {
        if (attackTimer > 0f) attackTimer -= Time.fixedDeltaTime;

        if (GameManager.instance != null && GameManager.instance.currentGameState != GameState.Playing)
        {
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player == null)
        {
            player = PlayerController.instance;
            if (player == null)
            {
                if (rb != null)
                    rb.linearVelocity = Vector2.zero;
                return;
            }
            Debug.Log(gameObject.name + ": Player 추적 시작");
        }

        Vector2 targetPosition = player.transform.position;
        Vector2 currentPosition = transform.position;
        Vector2 toPlayer = targetPosition - currentPosition;
        Vector2 finalMoveVector = Vector2.zero;

        if (currentStageType == StageData.StageType.Vertical)
        {
            float horizontalDirection = Mathf.Sign(toPlayer.x);
            Vector2 directionToTarget = new Vector2(horizontalDirection, 0f);

            int wallLayerMask = LayerMask.GetMask(wallLayerName);
            float raycastDistance = 0.6f;

            Vector3 rayStart = transform.position;
            Vector3 rayDirection = directionToTarget;
            Color rayColor = Color.red;

            RaycastHit2D hit = Physics2D.Raycast(currentPosition, directionToTarget, wallAttackDistance, wallLayerMask);

            if (hit.collider == null)
            {
                Debug.DrawRay(rayStart, rayDirection * raycastDistance, rayColor);

                finalMoveVector += directionToTarget * moveSpeed;
            }
            else
            {
                Debug.DrawRay(rayStart, rayDirection * hit.distance, rayColor);

                HandleWallAttack(hit.collider.gameObject);
            }
        }
        else
        {
            float distance = toPlayer.magnitude;

            if (distance > stoppingDistance)
            {
                Vector2 directionToPlayer = toPlayer.normalized;
                finalMoveVector += directionToPlayer * moveSpeed;
            }
        }

        if (finalMoveVector.magnitude > moveSpeed)
        {
            finalMoveVector = Vector2.ClampMagnitude(finalMoveVector, moveSpeed);
        }

        if (rb != null)
            rb.linearVelocity = finalMoveVector;
    }
    /// <summary>
    /// Raycast로 감지된 Wall 오브젝트에 피해를 줍니다.
    /// </summary>
    void HandleWallAttack(GameObject wallObject)
    {
        if (attackTimer <= 0f)
        {
            Stage2Wall wall = wallObject.GetComponent<Stage2Wall>();

            if (wall != null)
            {
                wall.TakeDamage(damage);
                attackTimer = attackInterval;
                Debug.Log($"{gameObject.name}이 Stage2Wall에 {damage} 피해를 입혔습니다.");
            }
            else
            {
                Debug.LogError($"Wall 오브젝트 {wallObject.name}에서 Stage2Wall 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            if (player != null && attackTimer <= 0f)
            {
                player.TakeDamage(damage);
                attackTimer = attackInterval;
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
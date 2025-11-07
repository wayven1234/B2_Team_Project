using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public static string gameState = "playing";             // 게임 상태 playing

    [SerializeField] private PlayerHealthBar _healthBar;    // PlayerHealthBar.cs 연결
    public float maxHealth;                                 // 최대 체력
    public float currentHealth;                             // 현재 체력

    private float startLevel = 1f;                          // 시작 레벨
    private float maxLevel = 12f;                           // 최대 레벨
    public float currentLevel;                              // 현재 레벨

    public float damage;                                    // 기본 데미지

    public float moveSpeed = 1f;                            // 이동 속도

    private Rigidbody2D rb;                                 // Rigidbody2D 참조
    private Vector2 moveInput;                              // moveInput

    public event Action OnGameOver;                         // GameOver Event

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();                   // Rigidbody2D 참조

        currentHealth = maxHealth;                          // 게임 시작 시 현재 체력을 최대 체력으로 초기화
        currentLevel = startLevel;                          // 게임 시작 시 레벨 초기화 (1레벨)

        _healthBar.Init(currentHealth);                     // 체력바 초기화

        gameState = "playing";                              // 게임 상태 playing으로 초기화
    }

    private void Update()
    {
        // 게임 상태가 playing이 아닐 시 return
        if (gameState != "playing")
            return;

        // W, A, S, D, 입력값 받기
        float moveX = Input.GetAxisRaw("Horizontal");       // A. D
        float moveY = Input.GetAxisRaw("Vertical");         // W, S

        moveInput = new Vector2(moveX, moveY).normalized;   // 대각선 이동 시 속도 보정
    }

    private void FixedUpdate()
    {
        // 게임 상태가 playing이 아닐 시 return
        if (gameState != "playing")
            return;

        // Rigidbody2D를 이용한 이동
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // tag가 Enemy인 Object와 충돌했을 때
        if (collision.gameObject.tag == "Enemy")
        {
            // Enemy Object의 Enemy Component 가져오기
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            // Enemy가 Null이 아닐 때
            if (enemy != null)
            {
                Debug.Log("Enemy Hit!");
                enemy.TakeDamage(damage);                   // Enemy Object에게 Player의 damage만큼 TakeDamage
            }
        }
        // tag가 EXP인 Object와 충돌했을 때
        if (collision.gameObject.tag == "EXP")
        {
            // 현재 레벨이 최대 레벨보다 낮으면
            if (currentLevel < maxLevel)
            {
                currentLevel += 1f;                         // 현재 레벨에서 +1
            }
            Destroy(collision.gameObject);                  // 부딪힌 EXP의 gameObject 파괴
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;                            // 현재 체력에서 받은 damage만큼 TakeDamage
        _healthBar.SetHealth(currentHealth);                // PlayerHealthBar를 현재 체력으로 업데이트

        // 현재 체력이 0이거나 0보다 낮을 때
        if (currentHealth <= 0f)
        {
            Die();                                          // 죽음
        }
    }

    void Die()
    {
        GameOver();                                         // GameOver() 메서드 실행

        Destroy(gameObject);                                // Player 파괴
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();                               // OnGameOver Event

        gameState = "gameOver";                             // 게임 상태 = gameOver
        GameStop();                                         // GameStop() 메서드 실행

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
            col.enabled = false;                            // Collider 비활성화
    }

    void GameStop()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(0.0f, 0.0f);        // 움직이지 못하게 강제로 멈춤
    }
}

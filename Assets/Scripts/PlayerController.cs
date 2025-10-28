using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerHealthBar _healthBar;
    public float maxHealth;
    public float currentHealth;

    private float startLevel = 1f;
    private float maxLevel = 12f;
    public float currentLevel;

    public float damage;

    public float moveSpeed = 1f; // 이동 속도

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Start()
    {
        currentHealth = maxHealth;
        currentLevel = startLevel;  // 게임 시작 시 레벨 초기화 (1레벨)

        _healthBar.Init(currentHealth);  // 체력바 초기화

        rb = GetComponent<Rigidbody2D>();   // Rigidbody2D 참조
    }

    private void Update()
    {
        // W, A, S, D, 입력값 받기
        float moveX = Input.GetAxisRaw("Horizontal"); // A. D
        float moveY = Input.GetAxisRaw("Vertical");   // W, S

        moveInput = new Vector2(moveX, moveY).normalized; // 대각선 이동 시 속도 보정
    }

    private void FixedUpdate()
    {
        // Rigidbody2D를 이용한 이동
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Enemy Hit!");
                enemy.TakeDamage(damage);
            }
        }
        if (collision.gameObject.tag == "EXP")
        {
            if (currentLevel < maxLevel)
            {
                currentLevel += 1f;
            }
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        _healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}

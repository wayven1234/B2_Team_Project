using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerHealthBar healthBar;
    public float maxHealth;
    private float currentHealth;

    public float moveSpeed = 1f; // 이동 속도

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Start()
    {
        healthBar.Init(currentHealth);
        rb = GetComponent<Rigidbody2D>();
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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

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

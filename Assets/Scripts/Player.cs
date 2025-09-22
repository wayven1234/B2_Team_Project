using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f; // 이동 속도

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Start()
    {
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
}

using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 1f; // 이동 속도
    private Transform player;    // 플레이어 Transform
    private Rigidbody2D rb;      // Rigidbody2D 참조

    private void Start()
    {
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
}

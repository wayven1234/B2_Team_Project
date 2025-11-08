using System;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Stage2Player : MonoBehaviour
{
    public static string gameState = "playing";

    public float moveSpeed;

    public event Action OnGameOver;

    private void Start()
    {
        gameState = "playing";
    }

    void Update()
    {
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f;
        }

        Vector3 movement = new Vector3(0f, verticalInput, 0f);
        transform.Translate(movement * moveSpeed * Time.deltaTime);
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

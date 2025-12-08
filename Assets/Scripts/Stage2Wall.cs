using System;
using UnityEngine;

public class Stage2Wall : MonoBehaviour
{
    public event Action<float, float> OnHealthChanged;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    private GameManager gameManager;

    private bool isDestroyed = false;

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    void Start()
    {
        currentHealth = maxHealth;
        gameManager = GameManager.instance;

        if (gameManager == null || gameManager.currentStageIndex != 2)
        {
            gameObject.SetActive(false);
            return;
        }

        Stage2WallHealthBarUI wallUI = FindFirstObjectByType<Stage2WallHealthBarUI>(FindObjectsInactive.Include);
        if (wallUI != null)
        {
            wallUI.LinkToWall(this);
            Debug.Log("Stage2Wall: WallHealthBarUI 연결 완료.");
        }
        else
        {
            Debug.LogError("Stage2Wall: WallHealthBarUI 컴포넌트를 씬에서 찾을 수 없습니다!");
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed || gameManager.currentGameState != GameState.Playing) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            DestroyWall();
        }
    }

    void DestroyWall()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        if (gameManager != null)
        {
            gameManager.ChangeState(GameState.GameOver);
        }

        Destroy(gameObject);
    }
}

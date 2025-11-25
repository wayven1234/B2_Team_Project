using UnityEngine;

public class Stage2Wall : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    private GameManager gameManager;

    private bool isDestroyed = false;

    void Start()
    {
        currentHealth = maxHealth;
        gameManager = GameManager.instance;

        if (gameManager == null || gameManager.currentStageIndex != 2)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed || gameManager.currentGameState != GameState.Playing) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

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

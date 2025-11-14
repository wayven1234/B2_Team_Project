using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private PlayerHealthBar healthBar;    // PlayerHealthBar.cs 연결
    [SerializeField] private UnityEngine.UI.Image[] itemImage;
    [SerializeField] private Sprite itemSprite;

    [Header("플레이어 스탯")]
    public float maxHealth;     // 최대 체력 (기본값 설정)
    public float moveSpeed;     // 이동 속도

    [SerializeField] private float startLevel;  // 시작 레벨
    [SerializeField] private float maxLevel;    // 최대 레벨
    [SerializeField] private float levelPerExpOrb;  // 경험치 구슬 1개당 레벨

    [Header("태그 설정")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string expTag = "EXP";
    [SerializeField] private string itemTag = "HealthItem";

    [Header("현재 상태")]
    public float currentHealth; // 현재 체력
    public float currentLevel;  // 현재 레벨
    public int itemUI;        // 아이템 갯수

    // 내부 컴포넌트
    private Rigidbody2D rb;
    private Vector2 moveInput;

    public event Action OnGameOver; // GameOver Event

    private bool canMoveHorizontal;
    private bool canMoveVertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentLevel = startLevel;
        healthBar.Init(currentHealth);

        SetMovementByStage();
    }

    /// <summary>
    /// GameManager로부터 현재 스테이지 타입을 받아와서
    /// canMoveHorizontal/Vertical 변수를 설정합니다.
    /// </summary>
    void SetMovementByStage()
    {
        if (GameManager.instance == null)
        {
            canMoveHorizontal = true;
            canMoveVertical = true;
            return;
        }

        StageType stage = GameManager.instance.GetStageType();

        switch (stage)
        {
            case StageType.Vertical:
                canMoveHorizontal = false;
                canMoveVertical = true;
                break;

            case StageType.Normal:
            default:
                canMoveHorizontal = true;
                canMoveVertical = true;
                break;
        }
    }

    private void Update()
    {
        if (GameManager.instance.currentGameState != GameState.Playing) return;

        float moveX = 0f;
        float moveY = 0f;

        if (canMoveHorizontal)
            moveX = Input.GetAxisRaw("Horizontal");

        if (canMoveVertical)
            moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.currentGameState != GameState.Playing) return;
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(enemyTag))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
                Debug.Log("제작중");
                //enemy.TakeDamage(damage);
        }

        if (collision.gameObject.CompareTag(expTag))
        {
            if (currentLevel < maxLevel)
                currentLevel += levelPerExpOrb;

            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag(itemTag))
        {
            if (itemUI < 2)
            {
                itemUIUpdate(itemSprite, itemUI);
                itemUI++;
            }
            else
                Debug.Log("이미 2개 모두 획득");
            Destroy(collision.gameObject);
        }
    }

    void itemUIUpdate(Sprite newSprite, int index)
    {
        if (index < 0 || index >= itemImage.Length)
        return;

        if (newSprite == null)
        {
            // 초기화: 슬롯 2개 모두 비우기
            for (int i = 0; i < itemImage.Length; i++)
            {
                itemImage[i].sprite = null;
                itemImage[i].color = new Color(1, 1, 1, 0);
            }
            return;
        }

        // 해당 슬롯에 스프라이트 넣기
        itemImage[index].sprite = newSprite;
        itemImage[index].color = new Color(1, 1, 1, 1);

        //Debug.Log($"아이템 슬롯 {index} 채워짐");
    }

    public void UseItemUI()
    {
        if (itemUI > 0)
        {
            itemUI--;
            itemUIUpdate(null, itemUI);
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        GameOver();
        Destroy(gameObject);
    }

    public void GameOver()
    {
        // OnGameOver 이벤트가 등록된 곳이 있다면 호출
        OnGameOver?.Invoke();

        GameManager.instance.ChangeState(GameState.GameOver);

        GameStop();

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
            col.enabled = false;
    }

    void GameStop()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }
}

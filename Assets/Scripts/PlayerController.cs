using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("캐릭터 데이터")]
    [SerializeField] private CharacterData characterData;

    [Header("연결")]
    [SerializeField] private PlayerHealthBar healthBar;    // PlayerHealthBar.cs 연결
    [SerializeField] private PlayerLevelBar levelBar;      // PlayerLevelBar.cs 연결
    [SerializeField] private UnityEngine.UI.Image[] itemImage;
    [SerializeField] private Sprite itemSprite;

    [Header("플레이어 스탯")]
    [SerializeField] private float startLevel;  // 시작 레벨
    [SerializeField] private float maxLevel;    // 최대 레벨
    [SerializeField] private int expOrbsPerLevel = 20;

    [Header("태그 설정")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string expTag = "EXP";
    [SerializeField] private string itemTag = "HealthItem";

    [Header("현재 상태")]
    public float currentHealth; // 현재 체력
    public float currentLevel;  // 현재 레벨
    public int itemUI;        // 아이템 갯수
    public int currentExpCount = 0;

    // 내부 컴포넌트
    private Rigidbody2D rb;
    private Vector2 moveInput;

    public event Action OnGameOver; // GameOver Event

    private bool canMoveHorizontal;
    private bool canMoveVertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = characterData.maxHealth;

        currentLevel = startLevel;
        healthBar.Init(currentHealth);

        if (levelBar != null)
            levelBar.Init(expOrbsPerLevel);

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
        rb.linearVelocity = moveInput * characterData.moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(enemyTag))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(enemy.damage);
            }
        }

        if (collision.gameObject.CompareTag(expTag))
        {
            if (currentLevel >= maxLevel)
            {
                Destroy(collision.gameObject);
                return;
            }

            currentExpCount++;

            if (levelBar != null)
                levelBar.SetHealth(currentExpCount);

            if (currentExpCount >= expOrbsPerLevel)
                LevelUp();

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

    /// <summary>
    /// 레벨업 처리 함수
    /// </summary>
    private void LevelUp()
    {
        if (currentLevel >= maxLevel) return;
        
        currentLevel++;
        currentExpCount = 0;
        Debug.Log($"레벨업! 현재 레벨: {currentLevel}");

        if (levelBar != null)
            levelBar.SetHealth(0);
    }

    void itemUIUpdate(Sprite newSprite, int index)
    {
        if (index < 0 || index >= itemImage.Length)
            return;

        if (newSprite == null)
        {
            
            itemImage[index].sprite = null;
            itemImage[index].color = new Color(1, 1, 1, 0);
        }
        else
        {
            // 해당 슬롯에 스프라이트 넣기
            itemImage[index].sprite = newSprite;
            itemImage[index].color = new Color(1, 1, 1, 1);

            //Debug.Log($"아이템 슬롯 {index} 채워짐");
        }
    }

    public void UseItemUI()
    {
        if (itemUI > 0)
        {
            itemUI--; // 아이템 갯수 하나 감소

            // 아이템 사용 전에 현재 체력이 최대 체력보다 적을 경우에만 100 증가
            if (currentHealth < characterData.maxHealth)
            {
                currentHealth += 100f;
                Debug.Log("아이템 사용: 체력 회복");
            }

            // 만약 현재 체력이 최대 체력을 초과하면 최대 체력으로 설정
            if (currentHealth > characterData.maxHealth)
            {
                currentHealth = characterData.maxHealth;
                Debug.Log("현재 체력이 최대 체력을 초과하여 최대 체력으로 설정");
            }

            // 체력바 UI 업데이트
            healthBar.SetHealth(currentHealth);

            // 아이템 UI 업데이트
            itemUIUpdate(null, itemUI);

            Debug.Log("아이템 사용 후 체력: " + currentHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

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

    /// <summary>
    /// 이 플레이어의 스킬 데미지 보너스를 반환합니다. (예: 무기 스크립트에서 호출)
    /// </summary>
    /// <returns>캐릭터 타입에 따른 데미지 보너스</returns>
    public float GetSkillDamageBonus()
    {
        if (characterData == null)
        {
            Debug.LogError("CharacterData가 PlayerController에 연결되지 않았습니다.");
            return 0f;
        }

        return characterData.GetSkillDamageBonus();
    }
}

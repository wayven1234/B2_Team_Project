using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; }

    [Header("캐릭터 데이터")]
    [SerializeField] private CharacterData characterData;

    [Header("애니메이터 컨트롤러")]
    [SerializeField] private RuntimeAnimatorController girlAnimController;
    [SerializeField] private RuntimeAnimatorController boyAnimController;

    [Header("연결")]
    private PlayerHealthBar healthBar;
    private PlayerLevelBar levelBar;
    private UnityEngine.UI.Image[] itemImage;
    [SerializeField] private Sprite itemSprite;

    [Header("플레이어 스탯")]
    [SerializeField] private float startLevel;  // 시작 레벨
    [SerializeField] private float maxLevel;    // 최대 레벨
    [SerializeField] private int expOrbsPerLevel = 20;
    [SerializeField] private float healthPotionHealAmount = 100f;

    [Header("태그 설정")]
    //[SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string expTag = "EXP";
    [SerializeField] private string itemTag = "HealthItem";

    [Header("현재 상태")]
    public float currentHealth; // 현재 체력
    public float currentLevel;  // 현재 레벨
    public int itemUI;        // 아이템 갯수
    public int currentExpCount = 0;

    [Tooltip("현재 재생 중인 애니메이션 상태 (디버그용)")]
    [SerializeField] private string currentAnimationState;

    // 애니메이션 관련
    private Animator animator;
    private string playerTypePrefix;

    private int lastDirection = 1; // 1: Front, 2: Back, 3: Left, 4: Right

    private const string PLAYER_FRONT = "Player_Front";
    private const string PLAYER_BACK = "Player_Back";
    private const string PLAYER_LEFT = "Player_Left";
    private const string PLAYER_RIGHT = "Player_Right";

    private const string PLAYER_FRONT_IDLE = "Player_Front_Idle";
    private const string PLAYER_BACK_IDLE = "Player_Back_Idle";
    private const string PLAYER_LEFT_IDLE = "Player_Left_Idle";
    private const string PLAYER_RIGHT_IDLE = "Player_Right_Idle";

    // 내부 컴포넌트
    private Rigidbody2D rb;
    private Vector2 moveInput;

    public event Action OnGameOver; // GameOver Event

    private bool canMoveHorizontal;
    private bool canMoveVertical;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (characterData == null)
            return;

        if (characterData.characterType == CharacterData.CharacterType.Girl)
            animator.runtimeAnimatorController = girlAnimController;
        else if (characterData.characterType == CharacterData.CharacterType.Boy)
            animator.runtimeAnimatorController = boyAnimController;

        playerTypePrefix = characterData.characterType.ToString();

        healthBar = FindFirstObjectByType<PlayerHealthBar>(FindObjectsInactive.Include);
        levelBar = FindFirstObjectByType<PlayerLevelBar>(FindObjectsInactive.Include);

        if (healthBar == null)
            Debug.LogError("PlayerHealthBar 컴포넌트를 찾을 수 없습니다.");
        if (levelBar == null)
            Debug.LogWarning("PlayerLevelBar 컴포넌트를 찾을 수 없습니다.");

        currentHealth = characterData.maxHealth;
        currentLevel = startLevel;

        SetMovementByStage();

        ChangeAnimationState(PLAYER_FRONT_IDLE);
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

        StageData.StageType stage = GameManager.instance.GetStageType();

        switch (stage)
        {
            case StageData.StageType.Vertical:
                canMoveHorizontal = false;
                canMoveVertical = true;
                break;

            case StageData.StageType.Normal:
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

        HandleAnimation(moveX, moveY);
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.currentGameState != GameState.Playing) return;
        rb.linearVelocity = moveInput * characterData.moveSpeed;
    }

    /// <summary>
    /// 입력 값에 따라 애니메이션 상태를 결정하고 변경
    /// </summary>
    private void HandleAnimation(float moveX, float moveY)
    {
        if (moveX == 0 && moveY == 0)
        {
            switch (lastDirection)
            {
                case 1:
                    ChangeAnimationState(PLAYER_FRONT_IDLE);
                    break;
                case 2:
                    ChangeAnimationState(PLAYER_BACK_IDLE);
                    break;
                case 3:
                    ChangeAnimationState(PLAYER_LEFT_IDLE);
                    break;
                case 4:
                    ChangeAnimationState(PLAYER_RIGHT_IDLE);
                    break;
            }
        }
        else
        {
            if (Mathf.Abs(moveY) > Mathf.Abs(moveX))
            {
                if (moveY < 0)
                {
                    ChangeAnimationState(PLAYER_FRONT);
                    lastDirection = 1;
                }
                else
                {
                    ChangeAnimationState(PLAYER_BACK);
                    lastDirection = 2;
                }
            }
            else
            {
                if (moveX < 0)
                {
                    ChangeAnimationState(PLAYER_LEFT);
                    lastDirection = 3;
                }
                else
                {
                    ChangeAnimationState(PLAYER_RIGHT);
                    lastDirection = 4;
                }
            }
        }
    }

    /// <summary>
    /// 실제 애니메이션을 재생하고 상태를 관리하는 헬퍼 함수
    /// </summary>
    /// <param name="newAction">"Player_Front" 등 행동 이름</param>
    private void ChangeAnimationState(string newAction)
    {
        string newState = $"{playerTypePrefix}_{newAction}";

        if (currentAnimationState == newState) return;

        animator.Play(newState);

        currentAnimationState = newState;
    }

    public void LinkUI(PlayerHealthBar newHealthBar, PlayerLevelBar newLevelBar, UnityEngine.UI.Image[] newItemImages)
    {
        healthBar = newHealthBar;
        levelBar = newLevelBar;
        itemImage = newItemImages;

        if (healthBar != null)
            healthBar.Init(currentHealth);
        else
            Debug.LogError("PlayerController: healthBar가 Link되지 않았습니다");

        if (levelBar != null)
            levelBar.Init(expOrbsPerLevel);

        if (itemImage == null || itemImage.Length == 0)
            Debug.LogError("PlayerController: itemImage array가 Link되지 않았습니다");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.gameObject.CompareTag(enemyTag))
        //{
        //    Enemy enemy = other.gameObject.GetComponent<Enemy>();
        //    if (enemy != null)
        //    {
        //        enemy.TakeDamage(enemy.damage);
        //    }
        //}

        if (other.gameObject.CompareTag(expTag))
        {
            if (currentLevel >= maxLevel)
            {
                Destroy(other.gameObject);
                return;
            }

            currentExpCount++;

            if (levelBar != null)
                levelBar.SetHealth(currentExpCount);

            if (currentExpCount >= expOrbsPerLevel)
                LevelUp();

            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag(itemTag))
        {
            if (itemImage == null || itemImage.Length == 0)
            {
                Debug.LogError("아이템 UI가 연결되지 않아 아이템을 먹을 수 없습니다");
                return;
            }

            if (itemUI < itemImage.Length)
            {
                itemUIUpdate(itemSprite, itemUI);
                itemUI++;
            }
            else
                Debug.Log($"이미 {itemImage.Length}개 모두 획득");

            Destroy(other.gameObject);
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
                currentHealth += healthPotionHealAmount;
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

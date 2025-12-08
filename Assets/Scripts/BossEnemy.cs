using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("보스 페이즈 설정")]
    [SerializeField] private float phaseTwoHealthThreshold = 400f;
    [SerializeField] private float phaseTwoHealthIncrease = 400f;
    [SerializeField] private string phaseTwoAnimationState = "Boss_Phase2_Move";

    [Header("Phase 2 애니메이션 이름")]
    private const string P2_FRONT = "Phase2_Move_Down";
    private const string P2_BACK = "Phase2_Move_Up";
    private const string P2_LEFT = "Phase2_Move_Left";
    private const string P2_RIGHT = "Phase2_Move_Right";

    private const string P2_FRONT_IDLE = "Phase2_Move_Down_Idle";
    private const string P2_BACK_IDLE = "Phase2_Move_Up_Idle";
    private const string P2_LEFT_IDLE = "Phase2_Move_Left_Idle";
    private const string P2_RIGHT_IDLE = "Phase2_Move_Right_Idle";

    [SerializeField] private bool isPhaseOne = true;

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Boss took {damage} damage. Current HP: {currentHealth}");

        CheckPhaseTransition();

        if (currentHealth <= 0f)
        {
            Die();
            Debug.Log("Boss Die");
        }
    }

    void CheckPhaseTransition()
    {
        if (isPhaseOne && currentHealth <= phaseTwoHealthThreshold)
        {
            Debug.Log($"보스 체력 {currentHealth} 도달! 페이즈 2로 전환합니다.");

            isPhaseOne = false;

            currentHealth += phaseTwoHealthIncrease;

            Debug.Log($"페이즈 2 전환 완료! 새로운 체력: {currentHealth}");

            ChangeAnimationState(phaseTwoAnimationState);
        }
    }

    protected override void UpdateAnimation(Vector2 moveVector)
    {
        if (anim == null) return;

        bool isMoving = moveVector.magnitude > 0.01f;
        string targetAnimation = "";

        // 현재 페이즈에 따라 사용할 애니메이션 프리픽스를 결정
        string frontMove = isPhaseOne ? ENEMY_FRONT : P2_FRONT; // ENEMY_FRONT는 Enemy.cs의 상수 사용
        string backMove = isPhaseOne ? ENEMY_BACK : P2_BACK;
        string leftMove = isPhaseOne ? ENEMY_LEFT : P2_LEFT;
        string rightMove = isPhaseOne ? ENEMY_RIGHT : P2_RIGHT;

        string frontIdle = isPhaseOne ? ENEMY_FRONT_IDLE : P2_FRONT_IDLE;
        string backIdle = isPhaseOne ? ENEMY_BACK_IDLE : P2_BACK_IDLE;
        string leftIdle = isPhaseOne ? ENEMY_LEFT_IDLE : P2_LEFT_IDLE;
        string rightIdle = isPhaseOne ? ENEMY_RIGHT_IDLE : P2_RIGHT_IDLE;


        if (!isMoving)
        {
            switch (lastDirection)
            {
                case 1: targetAnimation = frontIdle; break;
                case 2: targetAnimation = backIdle; break;
                case 3: targetAnimation = leftIdle; break;
                case 4: targetAnimation = rightIdle; break;
                default: targetAnimation = frontIdle; break;
            }
        }
        else
        {
            if (Mathf.Abs(moveVector.y) > Mathf.Abs(moveVector.x))
            {
                if (moveVector.y < 0)
                {
                    targetAnimation = frontMove;
                    lastDirection = 1;
                }
                else
                {
                    targetAnimation = backMove;
                    lastDirection = 2;
                }
            }
            else
            {
                if (moveVector.x < 0)
                {
                    targetAnimation = leftMove;
                    lastDirection = 3;
                }
                else
                {
                    targetAnimation = rightMove;
                    lastDirection = 4;
                }
            }
        }

        if (!string.IsNullOrEmpty(targetAnimation))
        {
            ChangeAnimationState(targetAnimation);
        }
    }
}

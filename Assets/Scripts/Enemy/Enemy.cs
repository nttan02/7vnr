using UnityEngine;
using Spine.Unity;
using Spine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeleton;
    [SerializeField] private Rigidbody2D rb;
    private Collider2D col;
    private SkeletonBounds bounds;
    public EnemyData data;
    // Stats
    public int maxHp;
    public int hp;
    private int damage;
    private float speed;
    private float attackRange;
    private float attackCooldown;

    // Patrol settings
    private float patrolRange;
    private float patrolSpeed;
    private float patrolWaitTime;
    private Vector2 patrolLeftPoint;
    private Vector2 patrolRightPoint;
    private Vector2 patrolTarget;
    private float patrolWaitTimer;
    private bool isWaitingAtPatrolPoint;

    // State
    private EnemyState state;
    private bool isDead;
    private bool isActive;
    private bool canAttack;
    private float attackTimer;
    private float respawnTimer;

    // Movement
    private float moveX;
    private int direction = 1;

    // Target
    private Character target;

    // Capabilities
    private bool canWalk;
    private bool canDoAttack;

    // Random behavior
    private float behaviorVariation; // 0.8 - 1.2
    private float nextBehaviorChangeTime;

    // Vị trí spawn thực tế
    private Vector2 originalSpawnPosition;

    public bool IsDead => isDead;
    public bool IsActive => isActive;
    public event System.Action<int, int> OnHpChanged;
    private Vector3 baseScale;

    public Transform damagePoint;

    private void Awake()
    {
        bounds = new SkeletonBounds();
        if (skeleton != null)
        {
            PlayAnimation("idle", true);
            skeleton.state.Complete += OnAnimationComplete;
            baseScale = skeleton.transform.localScale;
        }

        // Setup Rigidbody2D nếu chưa có
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        col = GetComponent<Collider2D>();
        // Cấu hình Rigidbody
        rb.gravityScale = 3f; // Enemy bị rơi
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void OnDestroy()
    {
        if (skeleton != null)
        {
            skeleton.state.Complete -= OnAnimationComplete;
        }
    }

    public void Init(EnemyData d, Vector2 spawnPos)
    {
        data = d;

        maxHp = data.hpMax;
        hp = data.hp;
        damage = data.damage;
        speed = data.speed;
        attackRange = data.attackRange;
        attackCooldown = data.attackCooldown;

        // Patrol settings
        patrolRange = data.patrolRange;
        patrolSpeed = data.patrolSpeed;
        patrolWaitTime = data.patrolWaitTime;

        // Random behavior cho mỗi enemy
        behaviorVariation = Random.Range(1f, 2f);
        nextBehaviorChangeTime = Time.time + Random.Range(0.5f, 1f);

        // Lưu vị trí spawn thực tế
        originalSpawnPosition = spawnPos;
        transform.position = spawnPos;

        // Setup patrol points
        if (patrolRange > 0)
        {
            patrolLeftPoint = spawnPos + Vector2.left * patrolRange;
            patrolRightPoint = spawnPos + Vector2.right * patrolRange;
            patrolTarget = Random.value > 0.5f ? patrolRightPoint : patrolLeftPoint;
        }
        else
        {
            patrolLeftPoint = spawnPos;
            patrolRightPoint = spawnPos;
            patrolTarget = spawnPos;
        }

        patrolWaitTimer = 0;
        isWaitingAtPatrolPoint = false;

        // Check animations
        canWalk = !string.IsNullOrEmpty(data.animations.walk);
        canDoAttack = !string.IsNullOrEmpty(data.animations.attack);

        state = EnemyState.Idle;
        isDead = false;
        isActive = true;
        canAttack = true;
        attackTimer = 0;
        respawnTimer = 0;

        PlayAnimation(data.animations.idle, true);

        FindTarget();

        Debug.Log($"{data.name} initialized at {spawnPos} (variation: {behaviorVariation:F2})");
    }

    private void FindTarget()
    {
        Character player = FindObjectOfType<Character>();
        if (player != null)
        {
            SetTarget(player);
        }
    }

    public float GetTopCollider()
    {
        if (col == null) return 1f;
        Bounds b = col.bounds;
        return b.extents.y + 2f;
    }


    private void Update()
    {
        float delta = Time.deltaTime;

        if (!isActive)
        {
            if (isDead)
            {
                respawnTimer += delta;
                if (respawnTimer >= data.respawnTime)
                    Respawn();
            }
            return;
        }

        if (!canAttack)
        {
            attackTimer -= delta;
            if (attackTimer <= 0)
                canAttack = true;
        }

        // Don't update AI during locked states
        if (state == EnemyState.Attack || state == EnemyState.Hit || state == EnemyState.Die)
        {
            moveX = 0;
            return;
        }

        // Random behavior variation
        if (Time.time >= nextBehaviorChangeTime)
        {
            behaviorVariation = Random.Range(1f, 2f);
            nextBehaviorChangeTime = Time.time + Random.Range(0.5f, 1f);
        }

        // Check if player is in attack range
        if (target != null && !target.IsDead)
        {
            float distToPlayer = Vector2.Distance(transform.position, target.transform.position);
            if (distToPlayer <= attackRange)
            {
                if (canDoAttack && canAttack)
                {
                    Attack();
                }
                else if (!canDoAttack && canAttack)
                {
                    DoDamageInstant();
                    canAttack = false;
                    attackTimer = attackCooldown;
                    SetState(EnemyState.Idle);
                }
                else
                {
                    SetState(EnemyState.Idle);
                    moveX = 0;
                }
            }
            else
            {
                // PATROL MODE
                if (patrolRange > 0 && (canWalk || speed > 0))
                {
                    PatrolBehavior(delta);
                }
                else
                {
                    SetState(EnemyState.Idle);
                    moveX = 0;
                }
            }
        }
        else
        {
            // PATROL MODE
            if (patrolRange > 0 && (canWalk || speed > 0))
            {
                PatrolBehavior(delta);
            }
            else
            {
                SetState(EnemyState.Idle);
                moveX = 0;
            }
        }

        // Update facing direction
        if (moveX != 0)
        {
            direction = moveX > 0 ? 1 : -1;
            if (skeleton != null)
            {
                skeleton.skeleton.ScaleX = direction;
            }
        }
    }

    private void FixedUpdate()
    {
        // Áp dụng movement qua Rigidbody
        if (rb != null && isActive && !isDead)
        {
            Vector2 vel = rb.velocity;
            vel.x = moveX;
            rb.velocity = vel;
        }
    }

    private void PatrolBehavior(float delta)
    {
        if (isWaitingAtPatrolPoint)
        {
            patrolWaitTimer -= delta;
            SetState(EnemyState.Idle);
            moveX = 0;

            if (patrolWaitTimer <= 0)
            {
                isWaitingAtPatrolPoint = false;

                // Switch patrol target
                if (Vector2.Distance(transform.position, patrolRightPoint) < 1f)
                {
                    patrolTarget = patrolLeftPoint;
                }
                else
                {
                    patrolTarget = patrolRightPoint;
                }
            }
        }
        else
        {
            float distToPatrolTarget = Vector2.Distance(transform.position, patrolTarget);

            if (distToPatrolTarget <= 0.8f)
            {
                isWaitingAtPatrolPoint = true;
                patrolWaitTimer = patrolWaitTime * behaviorVariation;
                moveX = 0;
                SetState(EnemyState.Idle);
            }
            else
            {
                MoveTowards(patrolTarget, patrolSpeed * behaviorVariation);
                SetState(EnemyState.Walk);
            }
        }
    }

    private void PlayAnimation(string anim, bool loop)
    {
        if (skeleton != null && !string.IsNullOrEmpty(anim))
        {
            try
            {
                skeleton.AnimationState.SetAnimation(0, anim, loop);
                bounds.Update(skeleton.skeleton, true);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{data.name}: Cannot play animation '{anim}'. {e.Message}");
            }
        }
    }

    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.TrackIndex == 0)
        {
            if (state == EnemyState.Attack)
            {
                SetState(EnemyState.Idle);
            }
            else if (state == EnemyState.Hit)
            {
                SetState(EnemyState.Idle);
            }
        }
    }

    private void MoveTowards(Vector2 targetPos, float moveSpeed)
    {
        float targetX = targetPos.x;
        float currentX = transform.position.x;

        if (Mathf.Abs(targetX - currentX) < 0.1f)
        {
            moveX = 0;
        }
        else
        {
            moveX = Mathf.Sign(targetX - currentX) * moveSpeed;
        }
    }

    private void Attack()
    {
        SetState(EnemyState.Attack);
        canAttack = false;
        attackTimer = attackCooldown * behaviorVariation;
        moveX = 0;

        PlayAnimation(data.animations.attack, false);

        Invoke(nameof(DoDamage), 0.3f);
    }

    private void DoDamage()
    {
        if (isDead) return;
        if (target != null && !target.IsDead &&
            Vector2.Distance(transform.position, target.transform.position) <= attackRange)
        {
            target.TakeDamage(damage);
        }
    }

    private void DoDamageInstant()
    {
        if (target != null && !target.IsDead &&
            Vector2.Distance(transform.position, target.transform.position) <= attackRange)
        {
            target.TakeDamage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead || !isActive) return;

        hp -= amount;
        hp = Mathf.Clamp(hp, 0, maxHp);
        DamagePopupSpawner.Instance.ShowDamage(
            amount,
            damagePoint.position
        );

        Debug.Log($"Enemy took {amount} damage → HP {hp}/{maxHp}");

        // 🔥 BÁO UI
        OnHpChanged?.Invoke(hp, maxHp);

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            if (!string.IsNullOrEmpty(data.animations.hit))
            {
                SetState(EnemyState.Hit);
                moveX = 0;
            }
        }
    }



    private void Die()
    {
        SetState(EnemyState.Die);
        isDead = true;
        isActive = false;
        respawnTimer = 0;
        moveX = 0;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        CancelInvoke(nameof(DoDamage));

        Debug.Log($"{data.name} died! Respawn in {data.respawnTime}s");
    }

    private void Respawn()
    {
        hp = maxHp;
        isDead = false;
        isActive = true;
        canAttack = true;
        respawnTimer = 0;

        transform.position = originalSpawnPosition;

        // Reset patrol
        if (patrolRange > 0)
        {
            patrolLeftPoint = originalSpawnPosition + Vector2.left * patrolRange;
            patrolRightPoint = originalSpawnPosition + Vector2.right * patrolRange;
            patrolTarget = Random.value > 0.5f ? patrolRightPoint : patrolLeftPoint;
        }

        patrolWaitTimer = 0;
        isWaitingAtPatrolPoint = false;

        // Random behavior again
        behaviorVariation = Random.Range(0.8f, 1.2f);
        nextBehaviorChangeTime = Time.time + Random.Range(2f, 5f);

        SetState(EnemyState.Idle);

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        FindTarget();

        Debug.Log($"{data.name} respawned at {originalSpawnPosition}!");
    }

    private void SetState(EnemyState newState)
    {
        if (state == newState) return;

        state = newState;

        switch (state)
        {
            case EnemyState.Idle:
                PlayAnimation(data.animations.idle, true);
                moveX = 0;
                break;

            case EnemyState.Walk:
                if (canWalk)
                {
                    PlayAnimation(data.animations.walk, true);
                }
                else
                {
                    PlayAnimation(data.animations.idle, true);
                }
                break;

            case EnemyState.Attack:
                if (canDoAttack)
                {
                    PlayAnimation(data.animations.attack, false);
                }
                moveX = 0;
                break;

            case EnemyState.Hit:
                PlayAnimation(data.animations.hit, false);
                moveX = 0;
                break;

            case EnemyState.Die:
                PlayAnimation(data.animations.die, false);
                moveX = 0;
                break;
        }
    }

    public void SetTarget(Character p)
    {
        target = p;
    }

    private void OnMouseDown()
    {
        if (isDead) return;

        EnemyInfoUI.Show(this);
        Character.Instance.SetTargetEnemy(this);
    }
}
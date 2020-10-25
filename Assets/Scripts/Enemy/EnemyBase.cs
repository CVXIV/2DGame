using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStatus {
    IDEL,
    WAIT,
    WALK,
    RUN,
    ATTACK,
    DEATH
}

public class EnemyBase : MonoBehaviour {
    #region 属性
    public float attackRange;
    public float listenRange;

    protected readonly Collider2D[] targets = new Collider2D[4];
    protected int layerMask;

    protected Damage damage;
    protected BeDamage beDamage;
    protected Transform checkPath;
    protected Animator animator;
    protected Rigidbody2D rigid;
    protected BoxCollider2D m_Collider;
    protected float idel_count = 0;
    protected bool isFlip = false;
    protected bool isLock = false;
    protected EnemyStatus status = EnemyStatus.IDEL;
    protected float forwardDistance;

    protected float idel_time;
    protected Transform curTarget = null;
    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }
    #endregion

    protected virtual void Awake() {
        layerMask = LayerMask.GetMask(ConstantVar.PlayLayer);
        beDamage = GetComponent<BeDamage>();
        beDamage.onDead += OnDead;

        checkPath = transform.Find("CheckPath");

        damage = GetComponent<Damage>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<BoxCollider2D>();

        isFlip = transform.rotation.eulerAngles.y == 180;
        InitCollider();
        InitNumParm();
    }

    protected virtual void FixedUpdate() {
        CheckTarget();
        ActionOfStatus();
        PlayAnimation();
    }


    protected virtual void InitNumParm() {
        idel_time = 3.0f;
        WalkSpeed = 1.5f;
        RunSpeed = 3.0f;
    }

    public virtual void SetSpeedX(float value) {
        if (value != 0) {
            // 设置角色转向
            isFlip = value < 0;
            transform.rotation = Quaternion.Euler(0, value < 0 ? 180 : 0, 0);
        }
        rigid.velocity = new Vector2(value, rigid.velocity.y);
    }
    protected virtual void SetSpeedY(float value) {
        rigid.velocity = new Vector2(rigid.velocity.x, value);
    }

    protected virtual void OnDead(int damageNum) {
        status = EnemyStatus.DEATH;
        // 确保动画必定播放，如果放在PlayAnimation，可能导致状态被冲走
        animator.SetBool("is_dead", true);
    }
    protected virtual void InitCollider() {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ConstantVar.EnemyLayer), LayerMask.NameToLayer(ConstantVar.EnemyLayer));
    }

    /// <summary>
    /// 动画最后一帧调用
    /// </summary>
    protected virtual void AfterDead() { Destroy(gameObject); }


    /// <summary>
    /// 检测周围是否有攻击目标
    /// </summary>
    protected virtual void CheckTarget() {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, listenRange, targets, layerMask);
        if (count > 0 && !IsLock()) {
            Transform resultTransform = targets[0].transform;
            if (Vector2.Distance(transform.position, resultTransform.position) <= attackRange) {
                // 设置为攻击状态
                curTarget = resultTransform;
                SetStatus(EnemyStatus.ATTACK);
                SetRotation(resultTransform.position.x < transform.position.x);
            } else {
                // 跑向目标
                if (Mathf.Abs(resultTransform.position.x - transform.position.x) < 0.1f) {
                    SetStatus(EnemyStatus.WAIT);
                } else {
                    // 设置为奔跑状态
                    SetStatus(EnemyStatus.RUN);
                    RunSpeed = resultTransform.position.x > transform.position.x ? Mathf.Abs(RunSpeed) : -Mathf.Abs(RunSpeed);
                    SetSpeedX(RunSpeed);
                }
            }
        }
    }

    /// <summary>
    /// 每个状态下角色的行为
    /// </summary>
    protected virtual void ActionOfStatus() {
        switch (status) {
            case EnemyStatus.IDEL:
                if (IsLock()) {
                    break;
                }
                SetSpeedX(0);
                idel_count += Time.deltaTime;
                if (idel_count >= idel_time) {
                    idel_count = 0;
                    SetSpeedX(WalkSpeed);
                    status = EnemyStatus.WALK;
                }
                break;
            case EnemyStatus.WAIT:
                SetSpeedX(0);
                break;
            case EnemyStatus.WALK:
                if (CheckPath()) {
                    SetSpeedX(WalkSpeed);
                } else {
                    SetSpeedX(0);
                    WalkSpeed = -WalkSpeed;
                    status = EnemyStatus.IDEL;
                }
                break;
            case EnemyStatus.RUN:
                if (CheckPath()) {
                    SetSpeedX(RunSpeed);
                } else {
                    SetSpeedX(0);
                    RunSpeed = -RunSpeed;
                    status = EnemyStatus.IDEL;
                }
                break;
            case EnemyStatus.ATTACK:
                SetSpeedX(0);
                break;
            case EnemyStatus.DEATH:
                SetSpeedX(0);
                SetSpeedY(0);
                break;
            default:
                break;
        }
    }
    protected virtual void PlayAnimation() { }

    /// <summary>
    /// 检测前方是否可以前行
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckPath() {
        // 首先检测脚下是否有地面
        RaycastHit2D down = Physics2D.Raycast(checkPath.position, Vector2.down, 1f, 1 << ConstantVar.groundLayer);
        // 其次检测面前是否有障碍物
        forwardDistance = Mathf.Abs(m_Collider.bounds.center.x - transform.right.x * m_Collider.bounds.extents.x - transform.position.x);
        RaycastHit2D forward = Physics2D.BoxCast(m_Collider.bounds.center, 1.5f * m_Collider.bounds.extents, 90, transform.right, forwardDistance, 1 << ConstantVar.groundLayer);
        return down && !forward;
    }

    /// <summary>
    /// 提供给外部使用的改变状态的方法
    /// </summary>
    /// <param name="enemyStatus"></param>
    public void SetStatus(EnemyStatus enemyStatus) {
        status = enemyStatus;
    }

    public void SetRotation(bool isFlip) {
        this.isFlip = isFlip;
        transform.rotation = Quaternion.Euler(0, isFlip ? 180 : 0, 0);
    }

    public void Lock() {
        isLock = true;
    }

    public void UnLock() {
        isLock = false;
        SetStatus(EnemyStatus.IDEL);
    }

    public bool IsLock() {
        return isLock;
    }
}
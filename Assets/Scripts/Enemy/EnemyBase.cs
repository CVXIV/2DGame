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
    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }
    #endregion

    protected virtual void Awake() {
        beDamage = GetComponent<BeDamage>();
        beDamage.onDead += OnDead;

        checkPath = transform.Find("CheckPath");

        damage = GetComponent<Damage>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<BoxCollider2D>();

        InitCollider();
        InitNumParm();
    }

    protected virtual void FixedUpdate() {
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

    protected virtual void OnDead(string resPos) {
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
        transform.rotation = Quaternion.Euler(0, isFlip ? 180 : 0, 0);
    }

    public void Lock() {
        isLock = true;
    }

    public void UnLock() {
        isLock = false;
    }

    public bool IsLock() {
        return isLock;
    }
}
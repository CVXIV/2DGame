using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChomperStatus {
    IDEL,
    WALK,
    RUN,
    ATTACK,
    DEATH
}

public class Chomper : MonoBehaviour {
    #region
    private Damage damage;
    private BeDamage beDamage;
    private Transform checkPath;
    private Animator animator;
    private Rigidbody2D rigid;
    private readonly float idel_time = 3f;
    private float idel_count = 0;
    private bool isFlip = false;
    private bool isLock = false;
    private ChomperStatus status = ChomperStatus.IDEL;
    public float WalkSpeed { get; set; } = 1.5f;
    public float RunSpeed { get; set; } = 3;
    #endregion

    private void Awake() {
        beDamage = GetComponent<BeDamage>();
        beDamage.onDead += OnDead;

        checkPath = transform.Find("CheckPath");

        damage = GetComponent<Damage>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();

        InitCollider();
    }



    private void Update() {
        ActionOfStatus();
        PlayAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag(ConstantVar.PlayTag)) {
            damage.Attack(collision.gameObject);
        }
    }

    private void OnDead(string resPos) {
        status = ChomperStatus.DEATH;
        // 确保动画必定播放，如果放在PlayAnimation，可能导致状态被冲走
        animator.SetBool("is_dead", true);
    }

    private void InitCollider() {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ConstantVar.EnemyLayer), LayerMask.NameToLayer(ConstantVar.EnemyLayer));
    }

    // 动画最后一帧调用
    private void AfterDead() {
        Destroy(gameObject);
    }

    public void SetSpeedX(float value) {
        if (value != 0) {
            // 设置角色转向
            isFlip = value < 0;
            transform.rotation = Quaternion.Euler(0, value < 0 ? 180 : 0, 0);
        }
        rigid.velocity = new Vector2(value, rigid.velocity.y);
    }
    private void SetSpeedY(float value) {
        rigid.velocity = new Vector2(rigid.velocity.x, value);
    }

    /// <summary>
    /// 每个状态下角色的行为
    /// </summary>
    private void ActionOfStatus() {
        switch (status) {
            case ChomperStatus.IDEL:
                SetSpeedX(0);
                idel_count += Time.deltaTime;
                if (idel_count >= idel_time) {
                    idel_count = 0;
                    SetSpeedX(WalkSpeed);
                    status = ChomperStatus.WALK;
                }
                break;
            case ChomperStatus.WALK:
                if (CheckPath()) {
                    SetSpeedX(WalkSpeed);
                } else {
                    SetSpeedX(0);
                    WalkSpeed = -WalkSpeed;
                    status = ChomperStatus.IDEL;
                }
                break;
            case ChomperStatus.RUN:
                if (CheckPath()) {
                    SetSpeedX(RunSpeed);
                } else {
                    SetSpeedX(0);
                    RunSpeed = -RunSpeed;
                    status = ChomperStatus.IDEL;
                }
                break;
            case ChomperStatus.ATTACK:
                SetSpeedX(0);
                break;
            case ChomperStatus.DEATH:
                SetSpeedX(0);
                SetSpeedY(0);
                break;
            default:
                break;
        }
    }

    private void PlayAnimation() {
        animator.SetBool("is_run", status == ChomperStatus.RUN);
        animator.SetBool("is_walk", status == ChomperStatus.WALK);
        animator.SetBool("is_attack", status == ChomperStatus.ATTACK);
    }

    /// <summary>
    /// 检测前方是否可以前行
    /// </summary>
    /// <returns></returns>
    private bool CheckPath() {
        // 首先检测脚下是否有地面
        RaycastHit2D down = Physics2D.Raycast(checkPath.position, Vector2.down, 1f, 1 << ConstantVar.groundLayer);
        // 其次检测面前是否有障碍物
        RaycastHit2D forward = Physics2D.Raycast(checkPath.position, Vector2.right, 0.2f, 1 << ConstantVar.groundLayer);
        RaycastHit2D back = Physics2D.Raycast(checkPath.position, Vector2.left, 0.2f, 1 << ConstantVar.groundLayer);
        return down && !forward && !back;
    }

    /// <summary>
    /// 提供给外部使用的改变状态的方法
    /// </summary>
    /// <param name="chomperStatus"></param>
    public void SetStatus(ChomperStatus chomperStatus) {
        status = chomperStatus;
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
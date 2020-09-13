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
    private SpriteRenderer sprite;
    private BoxCollider2D box;
    private readonly float idel_time = 3f;
    private float idel_count = 0;
    private float walk_speed = 2, run_speed = 5;
    private ChomperStatus status = ChomperStatus.IDEL;
    #endregion

    private void Awake() {
        beDamage = GetComponent<BeDamage>();
        beDamage.onDead += OnDead;

        damage = GetComponent<Damage>();
        checkPath = transform.Find("CheckPath");
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
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
        animator.SetBool("is_dead", true);
    }

    // 动画最后一帧调用
    private void AfterDead() {
        Destroy(gameObject);
    }

    private void SetSpeedX(float value) {
        if (value != 0) {
            // 设置角色转向
            sprite.flipX = value < 0;
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
                    status = ChomperStatus.WALK;
                }
                break;
            case ChomperStatus.WALK:
                if (CheckPath()) {
                    SetSpeedX(walk_speed);
                } else {
                    SetSpeedX(0);
                    walk_speed = -walk_speed;
                    run_speed = -run_speed;
                    status = ChomperStatus.IDEL;
                    checkPath.localPosition = new Vector3(-checkPath.localPosition.x, checkPath.localPosition.y, checkPath.localPosition.z);
                }
                break;
            case ChomperStatus.RUN:
                break;
            case ChomperStatus.ATTACK:
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
        Vector3 start = box.bounds.center + new Vector3(checkPath.localPosition.x > 0 ? box.bounds.extents.x : -box.bounds.extents.x, -box.bounds.extents.y, 0);
        RaycastHit2D forward = Physics2D.Raycast(start, checkPath.localPosition.x > 0 ? Vector2.right : Vector2.left, 0.2f, 1 << ConstantVar.groundLayer);
        return down && !forward;
    }
}
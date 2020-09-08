using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using static ConstantVar;

public class CharacterControl : MonoBehaviour {
    #region 变量
    private bool canMove = true;
    private BoxCollider2D box;
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Animator animator;
    public float speedX = 7;
    public float speedY = 11;
    private float timeY = 0f;
    private bool isOnGround = true;
    private bool isJump = false;
    private PlayerStatus status = PlayerStatus.IDEL;
    private PassPlatform passPlatform;
    private ContactFilter2D groundMask;
    private BeDamage beDamage;
    #endregion

    #region 回调
    private void Awake() {
        InitBeDamage();
        groundMask.SetLayerMask(1 << ConstantVar.groundLayer);
        box = GetComponent<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if (canMove) {
            // 方向判断
            SetVelocity();

            // 跳跃
            SetJump();

            // 下跳跃
            SetDownJump();
        }
        // 检测地面
        CheckIsOnGround();

        // 更新状态
        UpdateStatus();

        // 播放动画
        PlayAnimation();
    }
    #endregion

    #region 攻击，受伤，死亡相关
    private void OnAttack() {
        Debug.Log("攻击！");
    }
    private void OnHurt() {
        animator.SetTrigger("on_hurt");
        animator.SetBool("is_invincible", true);
        GamePanel._instance.UpdateHP(beDamage.health);
        beDamage.Disable();
        Invoke(nameof(RestoreEnable), 1);
    }
    private void OnDead() {
        GamePanel._instance.UpdateHP(beDamage.health);
        Debug.Log("死亡！");
    }

    private void RestoreEnable() {
        beDamage.Enable();
        animator.SetBool("is_invincible", false);
    }
    #endregion

    #region 初始化
    private void InitBeDamage() {
        beDamage = GetComponent<BeDamage>();
        beDamage.onHurt += OnHurt;
        beDamage.onDead += OnDead;
        // 初始化血量
        GamePanel._instance.InitHP(PlayerPrefs.GetInt(ConstantVar.HP, 5));
    }
    #endregion
    private void CheckIsOnGround() {
        Vector3 left = new Vector3(box.bounds.center.x - box.bounds.extents.x, box.bounds.center.y - box.bounds.extents.y, box.bounds.center.z);
        Vector3 right = new Vector3(box.bounds.center.x + box.bounds.extents.x, box.bounds.center.y - box.bounds.extents.y, box.bounds.center.z);
        // 发射4条射线确认角色是否处于地面
        RaycastHit2D hit1 = Physics2D.Raycast(left, Vector3.down, box.bounds.extents.y * 0.2f, 1 << ConstantVar.groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(right, Vector3.down, box.bounds.extents.y * 0.2f, 1 << ConstantVar.groundLayer);
        RaycastHit2D hit3 = Physics2D.Raycast(left, Vector3.up, box.bounds.extents.y * 0.2f, 1 << ConstantVar.groundLayer);
        RaycastHit2D hit4 = Physics2D.Raycast(right, Vector3.up, box.bounds.extents.y * 0.2f, 1 << ConstantVar.groundLayer);
        isOnGround = hit1 || hit2 || hit3 || hit4;
        Collider2D transformCollider = hit1.collider != null ? hit1.collider : hit2.collider != null ? hit2.collider : hit3.collider != null ? hit3.collider : hit4.collider;

        // 如果是空中平台，则需要判断是否触碰了上边的碰撞线
        if (isOnGround && transformCollider.CompareTag(SkyGroundTag)) {
            passPlatform = transformCollider.transform.GetComponent<PassPlatform>();
            float transform_y = transformCollider.bounds.center.y + transformCollider.bounds.extents.y;
            if (passPlatform != null) {
                if (passPlatform.ReadyToGround) {
                    isOnGround = true;
                } else {
                    isOnGround = passPlatform.ReadyToGround = left.y - transform_y >= 0;
                }
            }
        } else {
            if (passPlatform != null) {
                passPlatform.ReadyToGround = false;
                passPlatform = null;
            }
        }
    }


    public void NotMove() {
        this.canMove = false;
        this.SetSpeedX(0);
        //this.SetSpeedY(0);
    }

    public void Restore() {
        this.canMove = true;
    }

    private void SetVelocity() {
        SetSpeedX(Input.GetAxisRaw("Horizontal") * speedX);
    }

    private void SetJump() {
        // 跳跃判断
        if (isOnGround && status != PlayerStatus.CROUCH) {
            if (Input.GetButtonDown("Jump")) {
                timeY = 0;
                isJump = true;
            }
        }
        if (Input.GetButtonUp("Jump")) {
            isJump = false;
        }
        if (isJump && Input.GetAxisRaw("Jump") != 0) {
            timeY += Time.deltaTime;
            if (timeY < 0.2f) {
                SetSpeedY(speedY);
            }
        }
    }

    private void SetDownJump() {
        if (passPlatform != null && status == PlayerStatus.CROUCH && Input.GetButtonDown("Jump")) {
            passPlatform.Pass(this.gameObject);
        }
    }


    private void PlayAnimation() {
        animator.SetBool("is_run", status == PlayerStatus.RUN);
        animator.SetBool("is_crouch", status == PlayerStatus.CROUCH);
        animator.SetBool("is_jump", status == PlayerStatus.JUMP);
        animator.SetFloat("speed_y", this.rigid.velocity.y);
    }

    private void SetSpeedX(float value) {
        if (value != 0) {
            // 设置角色转向
            sprite.flipX = value < 0;
        }
        if (status == PlayerStatus.CROUCH) {
            value = 0;
        }
        rigid.velocity = new Vector2(value, rigid.velocity.y);
    }

    private void SetSpeedY(float value) {
        if (status == PlayerStatus.CROUCH) {
            value = 0;
        }
        rigid.velocity = new Vector2(rigid.velocity.x, value);
    }

    private void UpdateStatus() {
        status = PlayerStatus.IDEL;

        if (rigid.velocity.x != 0) {
            status = PlayerStatus.RUN;
        }

        if (!isOnGround) {
            status = PlayerStatus.JUMP;
        }

        if (status == PlayerStatus.IDEL && Input.GetAxisRaw("Vertical") == -1) {
            status = PlayerStatus.CROUCH;
        }
    }

}
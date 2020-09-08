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
    #endregion

    #region 回调
    private void Awake() {
        this.box = GetComponent<BoxCollider2D>();
        this.rigid = GetComponent<Rigidbody2D>();
        this.sprite = GetComponent<SpriteRenderer>();
        this.animator = GetComponent<Animator>();
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

    private void CheckIsOnGround() {
        // 发射两条射线确认角色是否处于地面
        Vector3 left = new Vector3(box.bounds.center.x - box.bounds.extents.x, box.bounds.center.y - box.bounds.extents.y, box.bounds.center.z);
        Vector3 right = new Vector3(box.bounds.center.x + box.bounds.extents.x, box.bounds.center.y - box.bounds.extents.y, box.bounds.center.z);
        RaycastHit2D hit1 = Physics2D.Raycast(left, Vector3.down, box.bounds.extents.y, 1 << ConstantVar.groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(right, Vector3.down, box.bounds.extents.y, 1 << ConstantVar.groundLayer);

        Collider2D transformCollider = hit1.collider != null ? hit1.collider : hit2.collider;
        isOnGround = hit1 || hit2;
        // 如果是空中平台，则需要判断是否触碰了上边的碰撞线
        if (isOnGround && transformCollider.CompareTag(SkyGroundTag)) {
            passPlatform = transformCollider.transform.GetComponent<PassPlatform>();
            float transform_y = transformCollider.bounds.center.y + transformCollider.bounds.extents.y;
            isOnGround = left.y - transform_y >= 0;
        } else {
            passPlatform = null;
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
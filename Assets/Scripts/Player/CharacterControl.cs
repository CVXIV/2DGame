using CVXIV;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantVar;

public enum AttackType {
    NormalAttack = 0,
    ShootAttack = 1
}

public class CharacterControl : MonoBehaviour {
    #region 变量
    private bool canMove = true;
    private bool isFlip;
    private bool isPush;
    private BoxCollider2D box;
    private Rigidbody2D rigid;
    private Animator animator;
    public float speedX = 7;
    public float speedY = 11;
    private float timeY = 0f;
    private bool isOnGround = true;
    private bool isJump = false;
    private PlayerStatus status = PlayerStatus.IDEL;
    private PassPlatform passPlatform;
    private PlayerBeDamage beDamage;
    private Damage damage;
    private readonly float attackGap = 0.8f;
    public bool IsHasWeapon {
        get;
        set;
    }
    private bool isReadyAttack = true;

    // 攻击范围检测
    private BoxCollider2D attackRange;
    private AttackRange attackCheck;
    // 子弹生成位置
    private Transform bulletPos;
    private GameObject bullet;
    // 推动物体
    private ContactFilter2D contactFilter2D;
    private readonly List<Collider2D> contacts = new List<Collider2D>();
    // 重生点
    private CheckPoint checkPoint;
    private Vector3 initPos;
    #endregion

    #region 回调
    private void Awake() {
        damage = GetComponent<Damage>();
        bulletPos = transform.Find("BulletPos");
        attackRange = transform.Find("AttackRange").GetComponent<BoxCollider2D>();
        attackCheck = attackRange.GetComponent<AttackRange>();
        box = GetComponent<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //IsHasWeapon = DataManager.Instance.GetData(ConstantVar.weapon_key) is Data1Value<bool> data && data.Value1;
        isFlip = transform.rotation.y == 1;
        IsHasWeapon = true;

        contactFilter2D.SetLayerMask(1 << ConstantVar.groundLayer);

        initPos = transform.position;
    }

    private void Start() {
        InitBeDamage();
    }

    private void FixedUpdate() {
        // 移动判断
        SetVelocity(canMove);
        // 检测地面
        CheckIsOnGround();
        // 更新状态
        UpdateStatus();
        // 播放动画
        PlayAnimation();
    }

    private void Update() {
        if (canMove) {
            // 跳跃
            SetJump();
            // 下跳跃
            SetDownJump();
            // 攻击
            SetAttack();
        }
    }
    #endregion

    #region 推动物体
    private bool CheckIsPush() {
        int count = rigid.GetContacts(contactFilter2D, contacts);
        for (int i = 0; i < count; ++i) {
            Pushable pushObj = contacts[i].gameObject.GetComponent<Pushable>();
            if (pushObj != null) {
                BoxCollider2D pushObjBox = pushObj.GetComponent<BoxCollider2D>();
                if (pushObjBox != null && Mathf.Abs(pushObjBox.bounds.center.x - box.bounds.center.x) >= pushObjBox.bounds.extents.x + box.bounds.extents.x) {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region 攻击，受伤，死亡相关
    private void SetAttack() {     
        if (!isPush && IsHasWeapon && isReadyAttack) {
            if (Input.GetAxisRaw("Fire1") != 0 || Input.GetButtonDown("Fire1")) {
                OnAttack(AttackType.NormalAttack);
                isReadyAttack = false;
                Invoke(nameof(ResetAttack), attackGap);
            } else if (Input.GetAxisRaw("Fire2") != 0 || Input.GetButtonDown("Fire2")) {
                OnAttack(AttackType.ShootAttack);
                isReadyAttack = false;
                Invoke(nameof(ResetAttack), attackGap);
            }
        }
    }

    private void ResetAttack() {
        isReadyAttack = true;
        animator.SetFloat("attack_threshold", 0);
    }

    private void OnAttack(AttackType attackType) {
        animator.SetTrigger("attack");
        animator.SetInteger("attack_type", (int)attackType);
        if (attackType == AttackType.ShootAttack) {
            animator.SetFloat("attack_threshold", 1);
            // 创建子弹，延迟调用确保在动画执行之后生成子弹
            Invoke(nameof(MakeBullet), 0.05f);
        }
    }

    // 普通攻击某一帧调用
    private void AttackDamage() {
        damage.Attack(attackCheck.GetDamageObjs().ToArray());
    }

    private void MakeBullet() {
        if (bullet == null) {
            bullet = Resources.Load<GameObject>("prefab/FlyingProb/Bullet");
        }
        GameObject newBullet = Instantiate(bullet);
        newBullet.transform.position = bulletPos.position;
        newBullet.GetComponent<Bullet>().SetSpeed(!isFlip);
    }


    private void OnHurt(DamageType damageType, int damageNum) {
        switch (damageType) {
            case DamageType.Normal:
                animator.SetTrigger("on_hurt");
                SetInvincible(1);
                break;
            case DamageType.Dead:
                // 播放死亡动画，重置位置
                StartCoroutine(DieRespawnCoroutine(false, true));
                break;
        }

    }

    /// <summary>
    /// 使得角色无敌一段时间
    /// </summary>
    /// <param name="time"></param>
    private void SetInvincible(float time) {
        animator.SetBool("is_invincible", true);
        beDamage.Disable();
        // 避免和Enemy碰撞
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ConstantVar.EnemyLayer), LayerMask.NameToLayer(ConstantVar.PlayLayer), true);
        Invoke(nameof(RestoreEnable), time);
    }

    private void RestoreEnable() {
        beDamage.Enable();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ConstantVar.EnemyLayer), LayerMask.NameToLayer(ConstantVar.PlayLayer), false);
        animator.SetBool("is_invincible", false);
    }

    private void PrepareDead() {
        NotMove();
        beDamage.Disable();
        animator.SetBool("is_dead", true);
    }

    private void OnDead(int damageNum) {
        StartCoroutine(DieRespawnCoroutine(true, false));
    }

    /// <summary>
    /// 角色受到重置位置攻击时调用
    /// </summary>

    private IEnumerator DieRespawnCoroutine(bool resetHealth, bool useCheckPoint) {
        PrepareDead();
        yield return new WaitForSeconds(1.0f); // 等待1秒
        yield return StartCoroutine(ScreenFader.FadeSceneIn(useCheckPoint ? ScreenFader.FadeType.Black : ScreenFader.FadeType.GameOver));
        // 如果是角色血量耗尽，则进入游戏结束界面且多等待2秒以展示该界面
        if (!useCheckPoint) {
            yield return new WaitForSeconds(2f);
        }
        Respawn(resetHealth, useCheckPoint);
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(ScreenFader.FadeSceneOut());
        CanMove();
        SetInvincible(2);
    }

    /// <summary>
    /// 重置角色位置
    /// </summary>
    /// <param name="resetHealth"></param>
    /// <param name="useCheckpoint"></param>
    private void Respawn(bool resetHealth, bool useCheckpoint) {
        if (resetHealth) {
            beDamage.ResetHealth();
        }
        if (useCheckpoint && checkPoint != null) {
            GameObjectTeleporter.Teleport(gameObject, checkPoint.transform.position);
        } else {
            GameObjectTeleporter.Teleport(gameObject, initPos);
        }
        SetSpeedX(0);
        SetSpeedY(0);
        animator.SetBool("is_dead", false);
    }


    public void CheckPoint(CheckPoint checkPoint) {
        this.checkPoint = checkPoint;
    }

    #endregion

    #region 初始化
    private void InitBeDamage() {
        beDamage = GetComponent<PlayerBeDamage>();
        beDamage.onHurt += OnHurt;
        beDamage.onDead += OnDead;
    }
    #endregion
    private void CheckIsOnGround() {
        Vector3 left = new Vector3(box.bounds.center.x - box.bounds.extents.x, box.bounds.center.y - box.bounds.extents.y, box.bounds.center.z);
        Vector3 right = new Vector3(box.bounds.center.x + box.bounds.extents.x, box.bounds.center.y - box.bounds.extents.y, box.bounds.center.z);
        // 发射4条射线确认角色是否处于地面
        RaycastHit2D hit1 = Physics2D.Raycast(left, Vector3.down, box.bounds.extents.y * 0.5f, 1 << ConstantVar.groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(right, Vector3.down, box.bounds.extents.y * 0.5f, 1 << ConstantVar.groundLayer);
        RaycastHit2D hit3 = Physics2D.Raycast(left, Vector3.up, box.bounds.extents.y * 0.5f, 1 << ConstantVar.groundLayer);
        RaycastHit2D hit4 = Physics2D.Raycast(right, Vector3.up, box.bounds.extents.y * 0.5f, 1 << ConstantVar.groundLayer);
        isOnGround = hit1 || hit2 || hit3 || hit4;
        Collider2D transformCollider = hit1.collider != null ? hit1.collider : (hit2.collider != null ? hit2.collider : (hit3.collider != null ? hit3.collider : hit4.collider));
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
    }

    public void CanMove() {
        this.canMove = true;
    }

    private void SetVelocity(bool canMove) {
        SetSpeedX(canMove ? Input.GetAxisRaw("Horizontal") * speedX : 0);
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
        animator.SetBool("is_push", isPush);
    }

    private void SetSpeedX(float value) {
        if (value != 0) {
            // 设置角色转向
            isFlip = value < 0;
            transform.rotation = Quaternion.Euler(0, value < 0 ? 180 : 0, 0);
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

        if (Mathf.Abs(rigid.velocity.x) == speedX) {
            status = PlayerStatus.RUN;
        }

        if (!isOnGround) {
            status = PlayerStatus.JUMP;
        }

        if (status == PlayerStatus.IDEL && Input.GetAxisRaw("Vertical") == -1) {
            status = PlayerStatus.CROUCH;
        }

        isPush = CheckIsPush() && status == PlayerStatus.RUN;
    }

}
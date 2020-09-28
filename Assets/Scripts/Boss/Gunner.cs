using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunnerStatus {
    IDEL,
    WALK,
    DISABLED,
    ATTACK,
    DEAD
}

public class Gunner : MonoBehaviour {
    #region 变量
    public Projectile projectile;
    public float projectileSpeed = 40f;
    public GrenadeK grenade;
    public Collider2D player;
    public GameObject beamLaser;
    public LightningManage lightningManage;
    public GameObject lightningEffect;
    public BeDamage gunnerHealth;
    public BeDamage shieldHealth;
    public Animator animator;
    public Damage damage;
    public Transform grenadePos;
    public Transform checkPath;

    private ContactFilter2D groundFilter;
    private readonly List<RaycastHit2D> groundHit = new List<RaycastHit2D>();
    private GameObject lightningAttack;
    private Collider2D shieldColl;
    private Collider2D gunnerColl;
    private Rigidbody2D rigid;
    private GunnerStatus gunnerStatus = GunnerStatus.WALK;
    // 护盾消失后BOSS无法行动的时间
    private readonly float disabledTime = 4f;
    private Vector3 attackTarget;
    private bool isDead = false;
    private bool isFlip = false;
    private int currentAttackType = 0;
    private readonly float laserTrackingSpeed = 3.0f;
    private readonly float walkSpeed = 1.2f;
    #endregion

    private void Awake() {
        if (player == null) {
            throw new System.Exception("Please confirm if the player exists");
        }
        gunnerColl = GetComponent<BoxCollider2D>();
        shieldColl = shieldHealth.GetComponent<CircleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        groundFilter.SetLayerMask(LayerMask.GetMask(ConstantVar.GroundLayerName));
        InitBedamage();
    }


    private void InitBedamage() {
        gunnerHealth.onHurt += GunnerOnHurt;
        gunnerHealth.onDead += GunnerOnDead;
        gunnerHealth.Disable();

        shieldHealth.onHurt += ShieldOnHurt;
        shieldHealth.onDead += ShieldOnDead;
    }

    #region 受伤相关
    private void GunnerOnHurt(DamageType damageType, string resetPos, int damageNum) {

    }

    private void GunnerOnDead(string resetPos) {
        gunnerStatus = GunnerStatus.DEAD;
        isDead = true;
        shieldHealth.gameObject.SetActive(false);
        gunnerHealth.Disable();
    }

    private void AfterDead() {
        Destroy(gameObject);
    }

    private void AfterExplode() {
        gunnerColl.enabled = false;
    }

    private void ShieldOnHurt(DamageType damageType, string resetPos, int damageNum) {
        // todo
    }

    private void ShieldOnDead(string resetPos) {
        gunnerStatus = GunnerStatus.DISABLED;
        Invoke(nameof(ResetShield), disabledTime);
    }

    // 动画调用
    private void AfterShieldDisappear() {
        gunnerHealth.Enable();
        shieldColl.enabled = false;
    }

    /// <summary>
    /// 重置护盾血量
    /// </summary>
    private void ResetShield() {
        gunnerStatus = GunnerStatus.IDEL;
        gunnerHealth.Disable();
        shieldColl.enabled = true;
        shieldHealth.ResetHealth();
    }
    #endregion

    private void Update() {
        attackTarget = player.bounds.center;
        // 激光追踪玩家
        Vector2 targetMovement = (attackTarget - beamLaser.transform.position).normalized;
        beamLaser.transform.right = -Vector3.Slerp(-beamLaser.transform.right, targetMovement, laserTrackingSpeed * Time.deltaTime);
    }


    private void FixedUpdate() {
        ActionOfStatus();
        UpdateStatus();
    }


    private void GrenadeAttack() {
        animator.SetTrigger("grenade_attack");
    }

    private void BeamAttack() {
        animator.SetTrigger("beam_attack");
    }

    private void LightningAttack() {
        animator.SetTrigger("lightning_attack");
    }

    private void Attack() {
        SetRotation(attackTarget.x > transform.position.x);
        //currentAttackType = Random.Range(1, 4);
        currentAttackType = 3;
        switch (currentAttackType) {
            case 1:
                GrenadeAttack();
                break;
            case 2:
                BeamAttack();
                break;
            case 3:
                LightningAttack();
                break;
        }
    }

    private void ActionOfStatus() {
        switch (gunnerStatus) {
            case GunnerStatus.IDEL:
                break;
            case GunnerStatus.WALK:
                if (CheckPath()) {
                    rigid.velocity = -transform.right * walkSpeed;
                } else {
                    SetRotation(!isFlip);
                }
                break;
            case GunnerStatus.DISABLED:
                break;
            case GunnerStatus.ATTACK:
                Attack();
                break;
            case GunnerStatus.DEAD:
                break;
            default:
                break;
        }
    }

    private void UpdateStatus() {
        animator.SetBool("is_walk", gunnerStatus == GunnerStatus.WALK);
        animator.SetBool("is_disabled", gunnerStatus == GunnerStatus.DISABLED);
        animator.SetBool("is_dead", gunnerStatus == GunnerStatus.DEAD);
    }

    private void MakeBullet() {
        Projectile newBullet = Instantiate(projectile);
        newBullet.transform.position = beamLaser.transform.position;
        var direction = -beamLaser.transform.right;
        newBullet.LockTarget(direction, damage.resetPos, projectileSpeed);
    }

    private void MakeGrenade() {
        GrenadeK newBullet = Instantiate(grenade);
        newBullet.transform.position = grenadePos.position;
        // 这里左边是图片的正方向
        newBullet.LockTarget(-transform.right, damage.resetPos);
    }

    private void MakeLightling() {
        LightningManage newBullet = Instantiate(lightningManage);
        lightningAttack = newBullet.gameObject;

        int count = Physics2D.Raycast(lightningEffect.transform.position, Vector3.down - transform.right, groundFilter, groundHit);
        if (count > 0) {
            // 取最近地面的坐标
            Vector2 groundPoint = groundHit[0].point;
            List<LineInfo> infos = new List<LineInfo> {
            new LineInfo { start = lightningEffect.transform.position, end = groundPoint},
            new LineInfo { start = lightningEffect.transform.position, end = groundPoint},
            new LineInfo { start = groundPoint, end = groundPoint + Vector2.left * 20},
            new LineInfo { start = groundPoint, end = groundPoint + Vector2.right * 20},
            new LineInfo { start = groundPoint, end = groundPoint + Vector2.left * 20},
            new LineInfo { start = groundPoint, end = groundPoint + Vector2.right * 20}
            };
            newBullet.InitLines(infos);
        }
    }

    private void ClearLightning() {
        Destroy(lightningAttack);
    }

    private void SetRotation(bool isFlip) {
        this.isFlip = isFlip;
        transform.rotation = Quaternion.Euler(0, isFlip ? 180 : 0, 0);
    }

    /// <summary>
    /// 检测前方是否可以前行
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckPath() {
        // 首先检测脚下是否有地面
        RaycastHit2D down = Physics2D.Raycast(checkPath.position, Vector2.down, 1f, 1 << ConstantVar.groundLayer);
        // 其次检测面前是否有障碍物
        // 计算物体坐标和碰撞体之间的差值
        float forwardDistance = Mathf.Abs(gunnerColl.bounds.center.x + transform.right.x * gunnerColl.bounds.extents.x - transform.position.x);
        RaycastHit2D forward = Physics2D.BoxCast(gunnerColl.bounds.center, 2 * gunnerColl.bounds.extents, 0, -transform.right, forwardDistance, 1 << ConstantVar.groundLayer);
        return down && !forward;
    }
}
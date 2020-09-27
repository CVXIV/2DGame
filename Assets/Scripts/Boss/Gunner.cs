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

    private ContactFilter2D groundFilter;
    private readonly List<ContactPoint2D> groundPoints = new List<ContactPoint2D>();
    private GameObject lightningAttack;
    private Collider2D shieldColl;
    private Collider2D gunnerColl;
    private Rigidbody2D rigid;
    private GunnerStatus gunnerStatus = GunnerStatus.ATTACK;
    // 护盾消失后BOSS无法行动的时间
    private readonly float disabledTime = 4f;
    private Transform grenadePos;
    private Vector3 attackTarget;
    private bool isDead = false;
    private bool isFlip = false;
    private int currentAttackType = 0;
    private readonly float laserTrackingSpeed = 3.0f;
    #endregion

    private void Awake() {
        if (player == null) {
            throw new System.Exception("Please confirm jif the player exists");
        }
        gunnerColl = GetComponent<BoxCollider2D>();
        shieldColl = shieldHealth.GetComponent<CircleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        groundFilter.SetLayerMask(LayerMask.GetMask(ConstantVar.GroundLayerName));
        InitBedamage();
        InitGrenade();
    }

    private void InitGrenade() {
        grenadePos = transform.Find("GrenadePos");
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
        //currentAttackType = Random.Range(1, 3);
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
                rigid.velocity = -transform.right * 1.5f;
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
        int count = rigid.GetContacts(groundFilter, groundPoints);
        if (count > 0) {
            float max_y = groundPoints[0].point.y;
            for (int i = 1; i < count; ++i) {
                if (max_y < groundPoints[i].point.y) {
                    max_y = groundPoints[i].point.y;
                }
            }
            float delta_y = Mathf.Abs(lightningEffect.transform.position.y - max_y);
            Vector3 direction = -transform.right * delta_y - transform.up * delta_y;
            Vector2 groundPoint = lightningEffect.transform.position + direction;
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
}
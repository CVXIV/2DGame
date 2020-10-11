using CVXIV;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GunnerStatus {
    IDEL,
    WALK,
    DISABLED,
    ATTACK,
    DEAD
}

public class Gunner : MonoBehaviour {

    [System.Serializable]
    public class BossRound {
        public int bossHP = 10;
        public int shieldHP = 10;
    }

    #region 变量
    public BossRound[] rounds;
    private int round = 0;

    [Space]
    public LightningManage lightningManage;
    public Projectile projectile;
    public float projectileSpeed = 40f;
    public GrenadeK grenade;
    public Collider2D player;
    public GameObject beamLaser;
    public BeDamage gunnerHealth;
    public BeDamage shieldHealth;
    public Animator animator;
    public Damage damage;
    public Transform lightningEffect;
    public Transform grenadePos;
    public Transform checkPath;

    private GameObject lightning;
    private Collider2D gunnerColl;
    private Rigidbody2D rigid;
    private ContactFilter2D groundFilter;
    private readonly List<RaycastHit2D> groundHit = new List<RaycastHit2D>();
    // 护盾消失后BOSS无法行动的时间
    private readonly float disabledTime = 3f;
    // 切换阶段所需的时间
    private readonly float shiftRoundTime = 2f;
    private Vector3 attackTarget;
    private bool isFlip = false;
    private readonly float laserTrackingSpeed = 3.0f;
    private readonly float WalkSpeed = 1.9f;
    // 延迟调用时间
    private readonly float beamDelay = 1.8f;
    private readonly float grenadeDelay = 1.1f;
    private readonly float lightningDelay = 1.8f;
    private readonly float lightningTime = 0.4f;  
    private readonly float shieldFadeDelay = 1.0f;
    private readonly float explodeDelay = 4.5f;
    private readonly float deadDelay = 2.8f;
    private readonly float walkDelay = 1f;
    private readonly float haltDelay = 0.35f;
    private float distancePerWalk;

    [Header("UI")]
    public Slider healthSlider;
    public Slider shieldSlider;
    public Animator healthAnimator;
    public Animator shieldAnimator;
    private int totalHealth = 0;
    private int currentHealth = 0;

    // AI
    private Root AI;
    #endregion

    private void Awake() {
        if (player == null) {
            throw new System.Exception("Please confirm if the player exists");
        }
        distancePerWalk = 2 * walkDelay * WalkSpeed;
        groundFilter.SetLayerMask(LayerMask.GetMask(ConstantVar.GroundLayerName));
        gunnerColl = GetComponent<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        InitBedamage();
    }


    private void InitBedamage() {
        gunnerHealth.onHurt += GunnerOnHurt;
        gunnerHealth.onDead += GunnerOnDead;

        shieldHealth.onHurt += ShieldOnHurt;
        shieldHealth.onDead += ShieldOnDead;
    }

    private void OnEnable() {
        AI = FakeAI.Root();
        AI.OpenBranch(
            FakeAI.Repeat(rounds.Length).OpenBranch(
                FakeAI.Call(NextRound),
                FakeAI.While(IsAlive).OpenBranch(
                    FakeAI.While(IsShieldOn).OpenBranch(
                        FakeAI.Call(ModifyDirection),
                        FakeAI.RandomSequence(null).OpenBranch(
                            FakeAI.Root().OpenBranch(
                                // 将WaitForAnimatorState放到开头，因为如果处于其他状态那么设置Trigger后不一定马上执行对应的动画从而导致Wait偏差
                                // 另外需要注意Trigger设置完动画并不是马上执行
                                FakeAI.WaitForAnimatorState(animator, "idel"),
                                FakeAI.Trigger(animator, "beam_attack", true),
                                FakeAI.Wait(beamDelay),
                                FakeAI.Call(MakeBullet),
                                FakeAI.Wait(0.5f)
                                ),
                            FakeAI.Root().OpenBranch(
                                FakeAI.WaitForAnimatorState(animator, "idel"),
                                FakeAI.Trigger(animator, "lightning_attack", true),
                                FakeAI.Wait(lightningDelay),
                                FakeAI.Call(MakeLightning, DestroyLightning),
                                FakeAI.Wait(lightningTime),
                                FakeAI.Call(DestroyLightning),
                                FakeAI.Wait(0.5f)
                            ),
                            FakeAI.Root().OpenBranch(
                                FakeAI.WaitForAnimatorState(animator, "idel"),
                                FakeAI.Trigger(animator, "grenade_attack", true),
                                FakeAI.Wait(grenadeDelay),
                                FakeAI.Call(MakeGrenade),
                                FakeAI.Wait(0.5f)
                            ),
                            FakeAI.If(CheckPath).OpenBranch(
                                FakeAI.WaitForAnimatorState(animator, "idel"),
                                FakeAI.Trigger(animator, "walk", true),
                                FakeAI.Call(Walk, Halt),
                                FakeAI.Wait(walkDelay),
                                FakeAI.Call(Halt),
                                FakeAI.Wait(haltDelay),
                                FakeAI.Call(Walk, Halt),
                                FakeAI.Wait(walkDelay),
                                FakeAI.Call(Halt),
                                FakeAI.Wait(haltDelay)
                            )
                        )
                    ),
                    FakeAI.Trigger(animator, "disabled", true),
                    FakeAI.Trigger(shieldAnimator, "Defeat", true),
                    FakeAI.Wait(shieldFadeDelay),
                    FakeAI.Call(ShieldOnDead),
                    FakeAI.Wait(disabledTime),
                    FakeAI.Call(ResetShield),
                    FakeAI.Trigger(animator, "enabled", true)
                ),
                FakeAI.If(IsMiddleRound).OpenBranch(
                    FakeAI.Wait(shiftRoundTime),
                    FakeAI.Trigger(animator, "enabled", true)
                )
            ),
            FakeAI.Trigger(animator, "death", true),
            FakeAI.Trigger(healthAnimator, "Defeat", true),
            FakeAI.Wait(explodeDelay),
            FakeAI.Call(AfterExplode),
            FakeAI.Wait(deadDelay),
            FakeAI.Call(AfterDead),
            FakeAI.Terminate()
        );

        foreach (var round in rounds) {
            totalHealth += round.bossHP;
        }
        currentHealth = totalHealth;
        healthSlider.maxValue = totalHealth;
        healthSlider.value = totalHealth;
    }

    private void Update() {
        AI.Execute();

        // 激光追踪玩家
        attackTarget = player.bounds.center;
        Vector2 targetMovement = (attackTarget - beamLaser.transform.position).normalized;
        // 不用right的原因是当沿Z轴旋转180度时，会自动改为沿Y轴旋转180度，导致激光不可见
        //beamLaser.transform.right = -Vector3.Slerp(-beamLaser.transform.right, targetMovement, laserTrackingSpeed * Time.deltaTime);
        beamLaser.transform.rotation = Quaternion.Slerp(beamLaser.transform.rotation, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.left, targetMovement)), laserTrackingSpeed * Time.deltaTime);
    }

    #region 受伤相关
    private void GunnerOnHurt(DamageType damageType, int damageNum) {
        currentHealth -= damageNum;
        healthSlider.value = currentHealth;
    }

    private void GunnerOnDead(int damageNum) {
        currentHealth = currentHealth - damageNum - gunnerHealth.Health;
        healthSlider.value = currentHealth;
        shieldHealth.gameObject.SetActive(false);
        gunnerHealth.Disable();
    }

    private void ShieldOnHurt(DamageType damageType, int damageNum) {
        shieldSlider.value -= damageNum;
    }

    private void ShieldOnDead(int damageNum) {
        shieldSlider.value -= damageNum;
    }

    private void ShieldOnDead() {
        shieldHealth.gameObject.SetActive(false);
        gunnerHealth.Enable();
    }

    /// <summary>
    /// 重置护盾血量
    /// </summary>
    private void ResetShield() {
        shieldHealth.gameObject.SetActive(true);
        gunnerHealth.Disable();
        shieldHealth.ResetHealth();
        shieldSlider.value = shieldHealth.Health;
    }

    private bool IsAlive() {
        return gunnerHealth.Health > 0;
    }

    private bool IsShieldOn() {
        return shieldHealth.Health > 0;
    }

    #endregion

    #region 流程相关
    private void NextRound() {
        // 更行BOSS和护盾的血量最大值
        gunnerHealth.UpdateMaxHealth(rounds[round].bossHP);
        shieldHealth.UpdateMaxHealth(rounds[round].shieldHP);
        gunnerHealth.Disable();
        shieldHealth.Enable();
        shieldHealth.gameObject.SetActive(true);

        shieldSlider.maxValue = rounds[round].shieldHP;
        shieldSlider.value = rounds[round].shieldHP;

        round++;
    }

    private bool IsMiddleRound() {
        return round < rounds.Length;
    }

    private void AfterDead() {
        Destroy(gameObject);
    }

    private void AfterExplode() {
        gunnerColl.enabled = false;
    }

    private void MakeBullet() {
        Projectile newBullet = Instantiate(projectile);
        newBullet.transform.position = beamLaser.transform.position;
        var direction = -beamLaser.transform.right;
        newBullet.LockTarget(direction, projectileSpeed);
    }

    private void MakeGrenade() {
        GrenadeK newBullet = Instantiate(grenade);
        newBullet.transform.position = grenadePos.position;
        // 这里左边是图片的正方向
        newBullet.LockTarget(-transform.right);
    }

    private void MakeLightning() {
        int count = Physics2D.Raycast(lightningEffect.position, Vector3.down - transform.right, groundFilter, groundHit);
        if (count > 0) {
            LightningManage example = Instantiate(lightningManage);
            example.transform.position = lightningEffect.position;
            lightning = example.gameObject;
            // 取最近地面的坐标
            Vector2 groundPoint = groundHit[0].point;
            List<LineInfo> infos = new List<LineInfo> {
            new LineInfo { start = lightningEffect.position, end = groundPoint},
            new LineInfo { start = lightningEffect.position, end = groundPoint},
            new LineInfo { start = groundPoint, end = groundPoint + Vector2.left * 20},
            new LineInfo { start = groundPoint, end = groundPoint + Vector2.right * 20},
            new LineInfo { start = groundPoint, end = groundPoint + Vector2.left * 20},
            new LineInfo { start = groundPoint, end = groundPoint + Vector2.right * 20}
            };
            example.InitLines(infos);
        }
    }

    private void DestroyLightning() {
        Destroy(lightning);
    }

    private void Walk() {
        rigid.velocity = -transform.right * WalkSpeed;
    }

    private void Halt() {
        rigid.velocity = Vector2.zero;
    }

    private void ModifyDirection() {
        bool isFlip = player.bounds.center.x > gunnerColl.bounds.center.x;
        SetRotation(isFlip);
    }

    #endregion


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
        float forwardDistance;
        if (transform.right.x * transform.position.x <= transform.right.x * gunnerColl.bounds.center.x) {
            float diff = -transform.right.x * (transform.position.x - gunnerColl.bounds.center.x - gunnerColl.bounds.extents.x);
            forwardDistance = -transform.right.x * (transform.position.x - transform.right.x * diff - (gunnerColl.bounds.center.x - transform.right.x * gunnerColl.bounds.extents.x));
        } else {
            forwardDistance = 0;
        }

        RaycastHit2D forward = Physics2D.BoxCast(gunnerColl.bounds.center, 2 * gunnerColl.bounds.extents, 0, -transform.right, forwardDistance + distancePerWalk, 1 << ConstantVar.groundLayer);
        return down && !forward;
    }
}
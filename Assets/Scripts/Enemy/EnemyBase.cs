using UnityEngine;

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
    protected bool isFlip = false;
    protected float forwardDistance;
    protected BoxCollider2D enemyBoxCollider = null;

    protected float idel_time;
    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }

    #endregion

    protected virtual void Awake() {
        layerMask = LayerMask.GetMask(ConstantVar.PlayLayer);
        beDamage = GetComponent<BeDamage>();
        beDamage.onDead += OnDead;
        beDamage.onHurt += OnHurt;

        checkPath = transform.Find("CheckPath");

        damage = GetComponent<Damage>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<BoxCollider2D>();

        isFlip = transform.rotation.eulerAngles.y == 180;
        InitCollider();
        InitNumParm();
    }


    protected virtual void InitNumParm() {
        idel_time = 3.0f;
        WalkSpeed = 1.5f;
        RunSpeed = 3.0f;
    }

    protected virtual void SetSpeedX(float value) {
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

    protected virtual void OnDead(int damageNum) {}
    protected virtual void OnHurt(DamageType damageType, int value) {}

    protected virtual void InitCollider() {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ConstantVar.EnemyLayer), LayerMask.NameToLayer(ConstantVar.EnemyLayer));
    }


    public virtual void EmenyCheck() {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, listenRange, targets, layerMask);
        if (count > 0) {
            enemyBoxCollider = targets[0].GetComponent<BoxCollider2D>();
        } else {
            enemyBoxCollider = null;
        }
    }


    protected virtual bool IsAlive() {
        return beDamage.Health > 0;
    }

    public virtual void Destroy() {
        Destroy(gameObject);
    }


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

    public void SetRotation(bool isFlip) {
        this.isFlip = isFlip;
        transform.rotation = Quaternion.Euler(0, isFlip ? 180 : 0, 0);
    }

}
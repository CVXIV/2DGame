using CVXIV;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Spitter : EnemyBase {
    #region 变量
    private GameObject bullet;
    public GameObject bloodBarPre;
    private Canvas canvas;
    private Slider bloodBar;
    private readonly string attack = "attack";
    private readonly string speed = "speed";
    private readonly string dead = "dead";
    private float idleCount = 0;
    #endregion

    protected override void Awake() {
        base.Awake();
        InitBloodBar();
        SceneLinkedSMB<Spitter>.Initialise(animator, this);
    }

    private void InitBloodBar() {
        Assert.AreNotEqual(bloodBarPre, null);
        canvas = Instantiate(bloodBarPre, transform).GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.transform.rotation = Quaternion.Euler(0, 0, 0);

        bloodBar = canvas.GetComponentInChildren<Slider>();
        bloodBar.wholeNumbers = true;
        bloodBar.value = bloodBar.maxValue = beDamage.Health;
        bloodBar.transform.position = transform.position + new Vector3(0, m_Collider.bounds.size.y, 0);
    }

    #region AI
    public void AliveCheck() {
        if (!IsAlive()) {
            animator.SetBool(dead, true);
        }
    }

    public void SetIdle() {
        SetSpeedX(0);
    }

    public override void EmenyCheck() {
        base.EmenyCheck();
        if (enemyBoxCollider != null) {
            idleCount = 0;
            ModifyDirection();
            if (Vector2.Distance(transform.position, enemyBoxCollider.transform.position) <= attackRange) {
                animator.SetTrigger(attack);
            } else {
                if (Mathf.Abs(enemyBoxCollider.transform.position.x - transform.position.x) < 0.3f) {
                    SetIdle();
                } else {
                    Run();
                }
            }
        } else {
            Walk();
        }
    }
    private void Walk() {
        if (CheckPath()) {
            SetSpeedX(isFlip ? -WalkSpeed : WalkSpeed);
        } else {
            if (idleCount >= idel_time) {
                idleCount = 0;
                TurnAround();
            } else {
                SetIdle();
                idleCount += Time.deltaTime;
            }
        }
    }

    private void Run() {
        if (CheckPath()) {
            SetSpeedX(isFlip ? -RunSpeed : RunSpeed);
        } else {
            SetSpeedX(0);
        }
    }

    private void ModifyDirection() {
        bool isFlip = enemyBoxCollider.bounds.center.x < m_Collider.bounds.center.x;
        SetRotation(isFlip);
    }

    private void TurnAround() {
        SetRotation(!isFlip);
    }

    #endregion

    protected override void SetSpeedX(float value) {
        base.SetSpeedX(value);
        animator.SetFloat(speed, value);
        canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    protected override void OnHurt(DamageType damageType, int value) {
        bloodBar.value = beDamage.Health;
    }

    protected override void OnDead(int value) {
        bloodBar.value = beDamage.Health;
    }

    protected override void InitNumParm() {
        idel_time = 1.5f;
        WalkSpeed = 1f;
        RunSpeed = 2f;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag(ConstantVar.PlayTag)) {
            damage.Attack(collision.gameObject);
        }
    }

    public void MakeBullet() {
        if (bullet == null) {
            bullet = Resources.Load<GameObject>("prefab/FlyingProb/TraceBullet");
        }
        TraceBullet newBullet = Instantiate(bullet).GetComponent<TraceBullet>();
        newBullet.LockTarget(enemyBoxCollider.transform, isFlip);
        newBullet.SetPosition(m_Collider.bounds.center + transform.right * m_Collider.bounds.extents.x);
    }
}
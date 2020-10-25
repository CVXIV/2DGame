using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spitter : EnemyBase {
    #region 变量
    private GameObject bullet;
    public GameObject bloodBarPre;
    private Canvas canvas;
    private Slider bloodBar;
    #endregion

    protected override void Awake() {
        base.Awake();
        InitBloodBar();
    }

    private void InitBloodBar() {
        if (bloodBarPre != null) {
            canvas = Instantiate(bloodBarPre, transform).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvas.transform.rotation = Quaternion.Euler(0, 0, 0);

            bloodBar = canvas.GetComponentInChildren<Slider>();
            bloodBar.wholeNumbers = true;
            bloodBar.value = bloodBar.maxValue = beDamage.Health;
            bloodBar.transform.position = transform.position + new Vector3(0, m_Collider.bounds.size.y, 0);
            beDamage.onHurt += OnHurt;
            beDamage.onDead += Ondead;
        }
    }

    public override void SetSpeedX(float value) {
        base.SetSpeedX(value);
        canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnHurt(DamageType damageType, int value) {
        bloodBar.value = beDamage.Health;
    }

    private void Ondead(int value) {
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

    protected override void PlayAnimation() {
        animator.SetBool("is_run", status == EnemyStatus.RUN);
        animator.SetBool("is_walk", status == EnemyStatus.WALK);
        animator.SetBool("is_attack", status == EnemyStatus.ATTACK);
    }

    public void MakeBullet() {
        if (bullet == null) {
            bullet = Resources.Load<GameObject>("prefab/FlyingProb/TraceBullet");
        }
        TraceBullet newBullet = Instantiate(bullet).GetComponent<TraceBullet>();
        newBullet.LockTarget(curTarget, isFlip);
        newBullet.SetPosition(m_Collider.bounds.center + transform.right * m_Collider.bounds.extents.x);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceBullet : BulletBase {
    #region
    public float speed = 4;
    private Transform target = null;
    // 蓄力时间
    private readonly float accelePeriod = 0.5f;
    // 旋转角度
    private readonly float MaximumRotationSpeed = 120f;
    // 是否可移动
    private bool isMoveAble = true;
    #endregion

    protected override void InitNumParm() {
        MaximumLifeTime = 6f;
    }

    protected override void InitCollider() {
        coll = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate() {
        if (!isMoveAble) {
            rigid.velocity = Vector2.zero;
            return;
        }
        if (target != null) {
            float deltaTime = Time.fixedDeltaTime;
            Vector2 offset = lifeTime < accelePeriod ? transform.right : (target.position - transform.position).normalized;
            // 夹角
            float angle = Vector2.Angle(transform.right, offset);
            float rotationTime = angle / MaximumRotationSpeed;
            // 本次旋转比例
            transform.right = Vector3.Slerp(transform.right, offset, deltaTime / rotationTime).normalized;
        }
        rigid.velocity = transform.right * speed;
    }

    protected override void Bomb() {
        isMoveAble = false;
        coll.enabled = false;
        animator.SetBool("is_bomb", true);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // 造成伤害
        if (collision.gameObject.CompareTag(ConstantVar.PlayTag)) {
            Bomb();
            damage.Attack(collision.gameObject);
        }
    }

    public void LockTarget(Transform target, bool isFlip, string resetPos) {
        damage.resetPos = resetPos;
        this.target = target;
        transform.rotation = Quaternion.Euler(0, isFlip ? 180 : 0, 0);
    }
}
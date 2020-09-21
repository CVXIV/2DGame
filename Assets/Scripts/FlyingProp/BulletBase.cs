using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour {
    #region 变量
    protected Rigidbody2D rigid;
    protected Collider2D coll;
    protected Animator animator;
    protected Damage damage;
    protected float MaximumLifeTime = 20.0f;
    protected float lifeTime = 0.0f;
    #endregion

    #region
    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        damage = GetComponent<Damage>();
        rigid = GetComponent<Rigidbody2D>();
        InitCollider();
        InitCollisionLayer();
        InitNumParm();
    }

    private void Update() {
        lifeTime += Time.deltaTime;
        if (lifeTime >= MaximumLifeTime) {
            Bomb();
        }
    }

    protected virtual void InitCollisionLayer() { }

    protected virtual void InitCollider() { }
    protected virtual void InitNumParm() { }
    protected virtual void Bomb() { }


    // 动画最后一帧调用
    private void Destroy() {
        Destroy(gameObject);
    }
    #endregion
}
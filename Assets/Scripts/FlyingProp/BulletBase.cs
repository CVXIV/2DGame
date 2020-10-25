using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour {
    #region 变量
    protected Rigidbody2D rigid;
    protected Collider2D coll;
    protected Animator animator;
    protected Damage damage;
    protected float MaximumLifeTime;
    protected float lifeTime = 0.0f;
    #endregion

    #region
    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        damage = GetComponent<Damage>();
        rigid = GetComponent<Rigidbody2D>();
        InitCollider();
        InitNumParm();
        gameObject.SetActive(false);
    }

    private void Update() {
        lifeTime += Time.deltaTime;
        if (lifeTime >= MaximumLifeTime) {
            Bomb();
        }
    }

    protected abstract void InitCollider();
    protected abstract void InitNumParm();
    protected abstract void Bomb();

    public void SetPosition(Vector3 pos) {
        transform.position = pos;
        // 设置完位置之后才激活，否则可能在预制体默认位置与外界发生碰撞
        gameObject.SetActive(true);
    }

    // 动画最后一帧调用
    private void Destroy() {
        Destroy(gameObject);
    }
    #endregion
}
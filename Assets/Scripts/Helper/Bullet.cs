using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    #region 变量
    private Rigidbody2D rigid;
    private SpriteRenderer render;
    private Animator animator;
    private Damage damage;
    #endregion

    #region
    private void Awake() {
        animator = GetComponent<Animator>();
        damage = GetComponent<Damage>();
        rigid = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
    }

    public void SetSpeed(bool isRight, float speed = 10f) {
        rigid.velocity = new Vector2(isRight ? speed : -speed, 0);
        render.flipX = !isRight;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // 造成伤害
        if (!collision.gameObject.CompareTag(ConstantVar.PlayTag)) {
            damage.Attack(collision.gameObject);
            // 播放动画
            animator.SetBool("is_bomb", true);
        }
    }

    // 动画最后一帧调用
    private void Destroy() {
        Destroy(gameObject);
    }
    #endregion
}
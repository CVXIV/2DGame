using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    #region 变量
    private Rigidbody2D rigid;
    private BoxCollider2D box;
    private SpriteRenderer render;
    private Animator animator;
    private Damage damage;
    #endregion

    #region
    private void Awake() {
        animator = GetComponent<Animator>();
        damage = GetComponent<Damage>();
        box = GetComponent<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ConstantVar.BulletLayer), LayerMask.NameToLayer(ConstantVar.PlayLayer));
        Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer(ConstantVar.BulletLayer), ~LayerMask.GetMask(ConstantVar.BulletLayer, ConstantVar.IgnoreLayer));
    }


    public void SetSpeed(bool isRight, float speed = 10f) {
        rigid.velocity = new Vector2(isRight ? speed : -speed, 0);
        transform.rotation = Quaternion.Euler(0, isRight ? 0 : 180, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // 造成伤害
        if (!collision.gameObject.CompareTag(ConstantVar.PlayTag)) {
            box.enabled = false;
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
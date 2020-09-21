using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BulletBase {

    protected override void InitCollisionLayer() {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ConstantVar.BulletLayer), LayerMask.NameToLayer(ConstantVar.PlayLayer));
        Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer(ConstantVar.BulletLayer), ~LayerMask.GetMask(ConstantVar.BulletLayer, ConstantVar.IgnoreLayer));
    }

    protected override void InitCollider() {
        coll = GetComponent<BoxCollider2D>();
    }

    protected override void Bomb() {
        coll.enabled = false;
        animator.SetBool("is_bomb", true);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // 造成伤害
        if (!collision.gameObject.CompareTag(ConstantVar.PlayTag)) {
            Bomb();
            damage.Attack(collision.gameObject);
        }
    }

    public void SetSpeed(bool isRight, float speed = 10f) {
        rigid.velocity = new Vector2(isRight ? speed : -speed, 0);
        transform.rotation = Quaternion.Euler(0, isRight ? 0 : 180, 0);
    }
}
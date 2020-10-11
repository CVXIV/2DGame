using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeK : BulletBase {
    protected override void InitNumParm() {
        MaximumLifeTime = 6f;
    }

    protected override void InitCollider() {
        coll = GetComponent<CircleCollider2D>();
    }

    protected override void Bomb() {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag(ConstantVar.PlayTag)) {
            Bomb();
            damage.Attack(collision.gameObject);
        }
    }

    public void LockTarget(Vector2 direction, float speed = 15f) {
        rigid.velocity = direction * speed;    
    }
}
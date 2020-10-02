using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Bullet : BulletBase {

    private ContactFilter2D contactFilter2D;
    private readonly List<Collider2D> colls = new List<Collider2D>();

    private bool canDamage = true;

    protected override void Awake() {
        base.Awake();
        contactFilter2D.SetLayerMask(LayerMask.GetMask(ConstantVar.EnemyLayer, ConstantVar.GroundLayerName));
    }

    protected override void InitCollider() {
        coll = GetComponent<BoxCollider2D>();
    }

    protected override void Bomb() {
        canDamage = false;
        rigid.velocity = Vector2.zero;
        animator.SetBool("is_bomb", true);
    }

    private void FixedUpdate() {
        if (!canDamage) {
            return;
        }
        int count = Physics2D.OverlapBox(coll.bounds.center, 2 * coll.bounds.extents, 0, contactFilter2D, colls);
        if (count > 0) {
            Bomb();
            damage.Attack(colls[0].gameObject);
        }
    }

    public void SetSpeed(bool isRight, float speed = 30f) {
        rigid.velocity = new Vector2(isRight ? speed : -speed, 0);
        transform.rotation = Quaternion.Euler(0, isRight ? 0 : 180, 0);
    }

    protected override void InitNumParm() {
        MaximumLifeTime = 20f;
    }
}
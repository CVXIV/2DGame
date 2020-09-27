using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BulletBase {

    struct CollideInfo {
        public GameObject target;
        public Vector2 collPoint;
    }

    // 特效预制体
    public GameObject explosionEffectPrefab;
    // 特效实例化后的对象
    private GameObject explosionEffect;
    private readonly float explosionTimer = 0.5f;
    private float speed;
    private Vector2 direction;
    private CircleCollider2D circleCollider2D;
    private bool isToBomb = false;
    private CollideInfo collideInfo;

    protected override void Awake() {
        base.Awake();
        explosionEffect = Instantiate(explosionEffectPrefab);
        explosionEffect.SetActive(false);
    }

    protected override void InitNumParm() {
        MaximumLifeTime = 6f;
    }

    protected override void InitCollider() {
        coll = circleCollider2D = GetComponent<CircleCollider2D>();
    }

    protected override void Bomb() {
        explosionEffect.transform.position = collideInfo.collPoint;
        explosionEffect.SetActive(true);
        Destroy(explosionEffect, explosionTimer);
        Destroy(gameObject);
    }

    private void FixedUpdate() {
        if (isToBomb) {
            return;
        }
        // 防止速度过快而穿透物体
        RaycastHit2D hit = Physics2D.CircleCast(coll.bounds.center, circleCollider2D.radius, direction, speed * Time.fixedDeltaTime, LayerMask.GetMask(ConstantVar.PlayLayer, ConstantVar.GroundLayerName));
        if (hit) {            
            isToBomb = true;
            collideInfo.collPoint = hit.centroid;
            collideInfo.target = hit.collider.gameObject;
            float dis = Vector2.Distance(hit.centroid, coll.bounds.center);
            Invoke(nameof(DamageEvent), dis / speed);
        }
    }

    private void DamageEvent() {
        Bomb();
        damage.Attack(collideInfo.target);
    }

    public void LockTarget(Vector2 direction, string resetPos, float speed = 15f) {
        this.direction = direction;
        this.speed = speed;
        rigid.velocity = direction * speed;
        damage.resetPos = resetPos;
    }
}
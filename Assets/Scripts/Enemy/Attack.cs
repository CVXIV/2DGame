using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    // 攻击范围
    public float attackRange;
    private CircleCollider2D circleCollider2D;
    private EnemyBase enemy;

    private void Awake() {
        enemy = transform.parent.GetComponent<EnemyBase>();
        InitAttackRange();
    }

    private void InitAttackRange() {
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = attackRange;
        Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer(ConstantVar.AttackLayer), LayerMask.GetMask(ConstantVar.PlayLayer));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (enemy.IsLock()) {
            return;
        }
        if (collision.CompareTag(ConstantVar.PlayTag)) {
            // 设置为攻击状态
            enemy.SetStatus(EnemyStatus.ATTACK);
            enemy.SetRotation(collision.transform.position.x < enemy.transform.position.x);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        OnTriggerEnter2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag(ConstantVar.PlayTag)) {
            enemy.SetStatus(EnemyStatus.IDEL);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    // 攻击范围
    public float attackRange;
    private CircleCollider2D circleCollider2D;
    private Chomper chomper;

    private void Awake() {
        chomper = transform.parent.GetComponent<Chomper>();
        InitAttackRange();
    }

    private void InitAttackRange() {
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = attackRange;
        Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer(ConstantVar.ListenLayer), LayerMask.GetMask(ConstantVar.PlayLayer));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (chomper.IsLock()) {
            return;
        }
        if (collision.CompareTag(ConstantVar.PlayTag)) {
            // 设置为攻击状态
            chomper.SetStatus(ChomperStatus.ATTACK);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        OnTriggerEnter2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag(ConstantVar.PlayTag)) {
            chomper.SetStatus(ChomperStatus.IDEL);
        }
    }
}
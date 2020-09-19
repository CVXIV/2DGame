using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Listen : MonoBehaviour {

    // 监听范围
    public float listenRange;
    private CircleCollider2D circleCollider2D;
    private EnemyBase enemy;

    private void Awake() {
        enemy = transform.parent.GetComponent<EnemyBase>();
        InitListenRange();
    }

    private void InitListenRange() {
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = listenRange;
        Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer(ConstantVar.ListenLayer), LayerMask.GetMask(ConstantVar.PlayLayer));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (enemy.IsLock()) {
            return;
        }
        if (collision.CompareTag(ConstantVar.PlayTag)) {
            // 跑向目标
            if (Mathf.Abs(collision.transform.position.x - enemy.transform.position.x) < 0.1f) {
                enemy.SetStatus(EnemyStatus.WAIT);
            } else {
                // 设置为奔跑状态
                enemy.SetStatus(EnemyStatus.RUN);
                enemy.RunSpeed = collision.transform.position.x > enemy.transform.position.x ? Mathf.Abs(enemy.RunSpeed) : -Mathf.Abs(enemy.RunSpeed);
                enemy.SetSpeedX(enemy.RunSpeed);
            }
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Listen : MonoBehaviour {

    // 监听范围
    public float listenRange;
    private CircleCollider2D circleCollider2D;
    private Chomper chomper;

    private void Awake() {
        chomper = transform.parent.GetComponent<Chomper>();
        InitListenRange();
    }

    private void InitListenRange() {
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = listenRange;
        Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer(ConstantVar.ListenLayer), LayerMask.GetMask(ConstantVar.PlayLayer));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (chomper.IsLock()) {
            return;
        }
        if (collision.CompareTag(ConstantVar.PlayTag)) {
            // 设置为奔跑状态
            chomper.SetStatus(ChomperStatus.RUN);
            // 跑向目标
            chomper.RunSpeed = collision.transform.position.x > chomper.transform.position.x ? Mathf.Abs(chomper.RunSpeed) : -Mathf.Abs(chomper.RunSpeed);
            chomper.SetSpeedX(chomper.RunSpeed);
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
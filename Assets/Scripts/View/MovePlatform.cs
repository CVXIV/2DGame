using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour {
    public float speed;
    public Vector2 endPos;
    private Vector2 startPos;
    private Vector2 curTarget;
    private bool isForth = true;
    private Rigidbody2D rigid;
    private readonly List<Collider2D> contacts = new List<Collider2D>();
    private ContactFilter2D contactFilter2D;
    private void Awake() {
        contactFilter2D.SetLayerMask(LayerMask.GetMask(ConstantVar.EnemyLayer, ConstantVar.PlayLayer));
        startPos = transform.position;
        rigid = GetComponent<Rigidbody2D>();
    }

    private void TransportObj() {
        int count = rigid.GetContacts(contactFilter2D, contacts);
        for (int i = 0; i < count; ++i) {
            // 只需要x分量的速度
            float velocity_x = (curTarget - new Vector2(transform.position.x, transform.position.y)).normalized.x * speed;
            contacts[i].attachedRigidbody.velocity += new Vector2(velocity_x, 0);
        }
    }

    private void Update() {
        curTarget = isForth ? endPos : startPos;
        transform.position = Vector2.MoveTowards(transform.position, curTarget, speed * Time.deltaTime);
        isForth = Vector2.Distance(transform.position, curTarget) != 0 ? isForth : !isForth;
    }

    private void LateUpdate() {
        TransportObj();
    }
}
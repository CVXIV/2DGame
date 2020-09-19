using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour {
    public float speed;
    protected float currentSpeed;
    public Vector2 endPos;
    private Vector2 startPos;
    private Vector2 curTarget;
    private bool isForth = true;
    private Rigidbody2D rigid;
    private readonly List<Collider2D> contacts = new List<Collider2D>();
    private ContactFilter2D contactFilter2D;
    private BoxCollider2D box;
    protected virtual void Awake() {
        contactFilter2D.SetLayerMask(LayerMask.GetMask(ConstantVar.EnemyLayer, ConstantVar.PlayLayer));
        startPos = transform.position;
        currentSpeed = speed;
        rigid = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    private void TransportObj() {
        int count = Physics2D.OverlapCollider(box, contactFilter2D, contacts);
        //int count = rigid.GetContacts(contactFilter2D, contacts);
        for (int i = 0; i < count; ++i) {
            // 只需要x分量的速度
            float velocity_x = (curTarget - new Vector2(transform.position.x, transform.position.y)).normalized.x * currentSpeed;
            contacts[i].attachedRigidbody.AddForce(new Vector2(contacts[i].attachedRigidbody.mass * velocity_x / Time.fixedDeltaTime, 0));
        }
    }

    protected virtual void Update() {
        curTarget = isForth ? endPos : startPos;
        transform.position = Vector2.MoveTowards(transform.position, curTarget, currentSpeed * Time.deltaTime);
        isForth = Vector2.Distance(transform.position, curTarget) != 0 ? isForth : !isForth;
    }

    /// <summary>
    /// 在FixedUpdate之后执行
    /// </summary>
    IEnumerator AfterFixedUpdate() {
        TransportObj();
        yield return new WaitForFixedUpdate();
    }

    private void FixedUpdate() {
        StartCoroutine(AfterFixedUpdate());
    }
}
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovePlatform : MonoBehaviour, ISwitchAble {
    private readonly RaycastHit2D[] hits = new RaycastHit2D[16];
    public enum MovePlatformType {
        BACK_FORTH,
        LOOP,
        ONCE
    }

    public bool isMoving = true;
    public MovePlatformType movePlatformType;
    public float[] waitTimes = new float[1];
    protected float waitTime;
    [HideInInspector]
    public Vector3[] localNodes = new Vector3[1];
    protected Vector3[] worldNodes;
    public Vector3[] WorldNode { get { return worldNodes; } }
    protected int currentNode;
    protected int nextNode;
    protected int directionNode;

    public float speed = 2f;
    private Rigidbody2D rigid;
    private ContactFilter2D contactFilter2D;
    private BoxCollider2D box;

    protected virtual void Awake() {
        contactFilter2D.SetLayerMask(LayerMask.GetMask(ConstantVar.EnemyLayer, ConstantVar.PlayLayer));
        rigid = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        Init();
    }

    private void Init() {
        localNodes[0] = Vector3.zero;
        worldNodes = new Vector3[localNodes.Length];
        for (int i = 0; i < localNodes.Length; ++i) {
            worldNodes[i] = transform.TransformPoint(localNodes[i]);
        }
        currentNode = 0;
        nextNode = localNodes.Length > 1 ? 1 : 0;
        waitTime = waitTimes[0];
        // 初始方向为正方向
        directionNode = 1;
    }

    private void TransportObj() {
        int count = Physics2D.BoxCastNonAlloc(box.bounds.center, 2 * box.bounds.extents, 0, Vector2.up, hits, 0.1f, contactFilter2D.layerMask);
        for (int i = 0; i < count; ++i) {
            float velocity_x = rigid.velocity.x;
            hits[i].collider.attachedRigidbody.AddForce(new Vector2(hits[i].collider.attachedRigidbody.mass * velocity_x / Time.fixedDeltaTime, 0));
        }
    }

    protected virtual void FixedUpdate() {
        if (!isMoving || currentNode == nextNode) {
            rigid.velocity = Vector2.zero;
            return;
        }
        if (waitTime > 0) {
            rigid.velocity = Vector2.zero;
            waitTime -= Time.fixedDeltaTime;
            return;
        }

        float currentDistance = speed * Time.fixedDeltaTime;
        Vector2 direction = WorldNode[nextNode] - transform.position;

        Vector3 currentSpeedVector = speed * direction.normalized;
        if (direction.sqrMagnitude < currentDistance * currentDistance) {
            // 如果距离已经小于本次更新将要移动的距离，则降低速度使其正好移动到该点
            currentSpeedVector = direction / Time.fixedDeltaTime;

            waitTime = waitTimes[nextNode];
            currentNode = nextNode;
            if (directionNode > 0) {
                nextNode++;
                if (nextNode == worldNodes.Length) {
                    switch (movePlatformType) {
                        case MovePlatformType.BACK_FORTH:
                            directionNode = -directionNode;
                            nextNode -= 2;
                            break;
                        case MovePlatformType.LOOP:
                            nextNode = 0;
                            break;
                        case MovePlatformType.ONCE:
                            nextNode -= 1;
                            StopMoving();
                            break;
                    }
                }
            } else {
                nextNode--;
                if (nextNode < 0) {
                    switch (movePlatformType) {
                        case MovePlatformType.BACK_FORTH:
                            directionNode = -directionNode;
                            nextNode += 2;
                            break;
                        case MovePlatformType.LOOP:
                            nextNode = worldNodes.Length - 1;
                            break;
                        case MovePlatformType.ONCE:
                            nextNode += 1;
                            StopMoving();
                            break;
                    }
                }
            }
        }
        rigid.velocity = currentSpeedVector;
        TransportObj();
    }

    public void StopMoving() {
        isMoving = false;
    }

    public void ControlDoor(SwitchStatus switchStatus) {
        isMoving = switchStatus == SwitchStatus.OPEN;
    }
}
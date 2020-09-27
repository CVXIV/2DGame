using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LineInfo {
    public Vector2 start;
    public Vector3 end;
}

public class Lightning : MonoBehaviour {
    public int pointCountPerSegment = 10;
    [Range(0, 1)]
    public float randomOffset = 0.5f;
    [Range(0, 1)]
    public float updateInterval = 0.05f;
    private Vector2 EndPoint {
        get;
        set;
    }
    private float updateTime = 0;
    private Vector2 segment;
    private Vector3[] points;
    private LineRenderer lineRenderer;
    private Damage damage;
    private readonly List<RaycastHit2D> results = new List<RaycastHit2D>();
    private float angle;
    private float distance;
    private Vector2 direction;

    private void Awake() {
        damage = GetComponent<Damage>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointCountPerSegment;
        // 使用相对位置
        lineRenderer.useWorldSpace = false;
    }

    private void Start() {
        InitPoint();
        angle = Vector2.SignedAngle(Vector2.left, EndPoint - (Vector2)transform.position);
        distance = Vector2.Distance(EndPoint, transform.position);
        direction = (EndPoint - (Vector2)transform.position).normalized;
    }

    private void InitPoint() {
        points = new Vector3[pointCountPerSegment];

        segment = (EndPoint - (Vector2)transform.position) / (pointCountPerSegment - 1);
        points[0] = Vector2.zero;
        points[pointCountPerSegment - 1] = EndPoint - (Vector2)transform.position;
    }

    private void Update() {
        if (updateTime > updateInterval) {
            for (int i = 1; i < pointCountPerSegment - 1; i++) {
                points[i] = segment * i;
                points[i].x += Random.Range(-randomOffset, randomOffset);
                points[i].y += Random.Range(-randomOffset, randomOffset);
            }
            lineRenderer.SetPositions(points);
            updateTime = 0;
        } else {
            updateTime += Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        int count = Physics2D.BoxCast(transform.position, new Vector2(1, 1), angle, direction, damage.attackContactFilter, results, distance);
        for(int i = 0; i < count; ++i) {
            RaycastHit2D raycastHit2D = results[i];
            BeDamage beDamage = raycastHit2D.collider.GetComponent<BeDamage>();
            if (beDamage) {
                damage.Attack(beDamage.gameObject);
            }
        }
    }

    public void SetLinePoint(LineInfo lineInfo) {
        transform.position = lineInfo.start;
        EndPoint = lineInfo.end;
    }



}
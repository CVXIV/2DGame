using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : SwitchBase {
    private GameObject point_light;
    private EdgeCollider2D edgeCollider;
    private readonly float availRange = 0.2f;

    protected override void Awake() {
        base.Awake();
        edgeCollider = GetComponent<EdgeCollider2D>();
        point_light = transform.Find("Light").gameObject;
        point_light.SetActive(false);
    }


    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag(ConstantVar.TriggerTag)) {
            if (status == SwitchStatus.OPEN) {
                React();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.CompareTag(ConstantVar.TriggerTag)) {
            if (status == SwitchStatus.CLOSE && Mathf.Abs(edgeCollider.bounds.center.x - collision.transform.position.x) < availRange) {
                React();
            } else if (status == SwitchStatus.OPEN && Mathf.Abs(edgeCollider.bounds.center.x - collision.transform.position.x) >= availRange) {
                React();
            }
        }
    }

    public override void OnReact() {
        base.OnReact();
        point_light.SetActive(status == SwitchStatus.OPEN);
    }

}
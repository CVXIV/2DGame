using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : SwitchBase {
    private GameObject point_light;
    private EdgeCollider2D edgeCollider;
    private readonly float availRange = 0.2f;
    private HashSet<GameObject> currentPreObj;

    protected override void Awake() {
        base.Awake();
        edgeCollider = GetComponent<EdgeCollider2D>();
        currentPreObj = new HashSet<GameObject>();
        point_light = transform.Find("Light").gameObject;
        point_light.SetActive(false);
    }


    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag(ConstantVar.TriggerTag) || collision.CompareTag(ConstantVar.PlayTag)) {
            if (status == SwitchStatus.OPEN) {
                currentPreObj.Remove(collision.gameObject);
                React();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(ConstantVar.TriggerTag) || collision.CompareTag(ConstantVar.PlayTag)) {
            if (status == SwitchStatus.CLOSE && Mathf.Abs(edgeCollider.bounds.center.x - collision.transform.position.x) < availRange) {
                currentPreObj.Add(collision.gameObject);
                React();
            } else if (status == SwitchStatus.OPEN && Mathf.Abs(edgeCollider.bounds.center.x - collision.transform.position.x) >= availRange) {
                currentPreObj.Remove(collision.gameObject);
                if (currentPreObj.Count == 0) {
                    React();
                }
            }
        }
    }

    public override void OnReact() {
        base.OnReact();
        point_light.SetActive(status == SwitchStatus.OPEN);
    }

}
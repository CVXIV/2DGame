using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Switch : SwitchBase {
    private GameObject point_light;
    private GameObject realControlObj;


    protected override void Awake() {
        base.Awake();
        if (target != null) {
            target = target.transform.Find("sprite").gameObject;
        }
        point_light = transform.Find("Light").gameObject;
        point_light.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag(ConstantVar.TriggerTag)) {
            React();
        }
    }

    public override void OnReact() {
        base.OnReact();
        point_light.SetActive(status == SwitchStatus.OPEN);
    }

}
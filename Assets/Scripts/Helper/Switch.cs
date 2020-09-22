﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Switch : SwitchBase {
    public UnityEvent<Vector3, float> unityEvent;
    private GameObject point_light;


    protected override void Awake() {
        base.Awake();
        point_light = transform.Find("Light").gameObject;
        point_light.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(ConstantVar.TriggerTag)) {
            React();
            if (status == SwitchStatus.OPEN) {
                unityEvent?.Invoke(transform.position, 1);
            }
        }
    }


    public override void OnReact() {
        base.OnReact();
        point_light.SetActive(status == SwitchStatus.OPEN);
    }

}
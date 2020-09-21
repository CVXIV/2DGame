using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatformSwitchAble : MovePlatform, ISwitchAble {
    private SwitchStatus status = SwitchStatus.CLOSE;

    protected override void Awake() {
        base.Awake();
    }

    protected override void FixedUpdate() {
        currentSpeed = status == SwitchStatus.OPEN ? speed : 0;
        base.FixedUpdate();
    } 

    public void ControlDoor(SwitchStatus switchStatus) {
        status = switchStatus;
    }
}
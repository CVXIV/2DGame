using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatformSwitchAble : MovePlatform, ISwitchAble {
    private SwitchStatus status = SwitchStatus.CLOSE;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Update() {
        currentSpeed = status == SwitchStatus.OPEN ? speed : 0;
        base.Update();
    }

    public void ControlDoor(SwitchStatus switchStatus) {
        status = switchStatus;
    }
}
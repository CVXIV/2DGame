﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : SwitchControlBase {

    protected override void InitAnim() {
        this.animName = "open_door";
    }
}
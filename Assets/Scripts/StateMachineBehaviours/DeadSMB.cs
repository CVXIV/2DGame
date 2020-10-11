﻿using UnityEngine;

namespace CVXIV {
    public class DeadSMB : SceneLinkedSMB<PlayerController> {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.CheckIsOnGround();
            monoBehaviour.GroundedHorizontalMovement(false);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateExit(animator, stateInfo, layerIndex);
            monoBehaviour.SetVelocity(Vector2.zero);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVXIV {
    public class LocomotionSMB : SceneLinkedSMB<PlayerController> {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.GroundedHorizontalMovement(true);
            monoBehaviour.CheckFacing();
            monoBehaviour.CheckCrouching();
            monoBehaviour.CheckTumble();
            monoBehaviour.GroundedCheckJump();
            monoBehaviour.CheckIsOnGround();
            monoBehaviour.CheckIsPush();
            monoBehaviour.CheckNormalAttack();
            monoBehaviour.CheckSkillAttack();
            monoBehaviour.SetLocomotionSpeed();
        }
    }
}


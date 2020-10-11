using UnityEngine;

namespace CVXIV {
    public class CrouchingSMB : SceneLinkedSMB<PlayerController> {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.GroundedHorizontalMovement(false);
            monoBehaviour.CheckFacing();
            monoBehaviour.CheckCrouching();
            monoBehaviour.CheckIsOnGround();
            monoBehaviour.CrouchingCheckDownJump();
            monoBehaviour.CheckIsHoldingGun();
        }
    }
}

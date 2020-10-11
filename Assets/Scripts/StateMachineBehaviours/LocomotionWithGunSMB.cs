using UnityEngine;

namespace CVXIV {
    public class LocomotionWithGunSMB : SceneLinkedSMB<PlayerController> {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.GroundedHorizontalMovement(true);
            monoBehaviour.CheckFacing();
            monoBehaviour.CheckCrouching();
            monoBehaviour.GroundedCheckJump();
            monoBehaviour.CheckIsOnGround();
            monoBehaviour.CheckIsPush();
            monoBehaviour.CheckIsHoldingGun();
            monoBehaviour.CheckGunFire();
            monoBehaviour.CheckNormalAttack();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            AnimatorStateInfo nextState = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (!nextState.IsTag("WithGun")) {
                monoBehaviour.ForceNotHoldingGun();
            }
        }
    }
}

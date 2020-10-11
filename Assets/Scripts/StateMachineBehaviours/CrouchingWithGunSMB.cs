using UnityEngine;
namespace CVXIV {
    public class CrouchingWithGunSMB : SceneLinkedSMB<PlayerController> {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.GroundedHorizontalMovement(false);
            monoBehaviour.CheckFacing();
            monoBehaviour.CheckCrouching();
            monoBehaviour.CheckIsOnGround();
            monoBehaviour.CrouchingCheckDownJump();
            monoBehaviour.CheckIsHoldingGun();
            monoBehaviour.CheckGunFire();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            AnimatorStateInfo nextState = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (!nextState.IsTag("WithGun")) {
                monoBehaviour.ForceNotHoldingGun();
            }
        }
    }
}

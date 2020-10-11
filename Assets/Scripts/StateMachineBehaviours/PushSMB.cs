using UnityEngine;

namespace CVXIV {
    public class PushSMB : SceneLinkedSMB<PlayerController> {

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.GroundedHorizontalMovement(true);
            monoBehaviour.CheckIsPush();
            monoBehaviour.CheckFacing();
            monoBehaviour.CheckIsOnGround();
            monoBehaviour.GroundedCheckJump();
        }

    }
}

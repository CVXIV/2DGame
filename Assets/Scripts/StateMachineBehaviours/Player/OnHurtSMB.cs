using UnityEngine;

namespace CVXIV {
    public class OnHurtSMB : SceneLinkedSMB<PlayerController> {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.GroundedHorizontalMovement(false);
            monoBehaviour.CheckIsOnGround();
        }
    }
}

using UnityEngine;

namespace CVXIV {
    public class JumpSMB : SceneLinkedSMB<PlayerController> {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.GroundedHorizontalMovement(true);
            monoBehaviour.CheckFacing();
            monoBehaviour.CheckIsOnGround();
            monoBehaviour.JumpingUpdateJump();
            monoBehaviour.CheckNormalAttack();
            monoBehaviour.CheckSkillAttack();
        }
    }
}

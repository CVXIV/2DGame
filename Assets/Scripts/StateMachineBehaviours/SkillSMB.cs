using UnityEngine;

namespace CVXIV {
    public class SkillSMB : SceneLinkedSMB<PlayerController> {
        private bool hasAttack = false;
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.GroundedHorizontalMovement(false);
            if (!hasAttack && stateInfo.normalizedTime >= 0.5f) {
                monoBehaviour.MakeBullet();
                hasAttack = true;
            }
            monoBehaviour.CheckIsOnGround();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateExit(animator, stateInfo, layerIndex);
            hasAttack = false;
        }

    }
}

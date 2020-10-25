using UnityEngine;

namespace CVXIV {
    public class NormalAttack02SMB : SceneLinkedSMB<PlayerController> {
        private bool hasAttack = false;
        private readonly string nextAttackName = "swordattack03";
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.GroundedHorizontalMovement(false);
            if (!hasAttack && stateInfo.normalizedTime >= 0.8f) {
                monoBehaviour.AttackDamage();
                hasAttack = true;
            }
            monoBehaviour.CheckIsOnGround();
            if (stateInfo.normalizedTime >= 0.8f) {
                monoBehaviour.CheckFacing();
                monoBehaviour.CheckNormalAttack();
            }
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateExit(animator, stateInfo, layerIndex);
            monoBehaviour.SetIsReadyNormalAttack(animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(nextAttackName));
            hasAttack = false;
        }

    }
}

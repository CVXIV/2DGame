using UnityEngine;

namespace CVXIV {
    public class JumpSkillSMB : SceneLinkedSMB<PlayerController> {
        private bool hasAttack = false;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            monoBehaviour.SetKinematic(true);
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            if (!hasAttack && stateInfo.normalizedTime >= 0.5f) {
                monoBehaviour.MakeBullet();
                hasAttack = true;
            }
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateExit(animator, stateInfo, layerIndex);
            hasAttack = false;
            monoBehaviour.SetKinematic(false);
        }

    }
}

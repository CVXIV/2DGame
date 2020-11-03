using UnityEngine;

namespace CVXIV {
    public class TumbleSMB : SceneLinkedSMB<PlayerController> {

        private readonly float forceValue = 180f;
        private readonly float invincibleBeginTime = 0.5f;
        private readonly float invincibleEndTime = 1f;
        private readonly int hashInvincible = Animator.StringToHash("Invincible");

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            monoBehaviour.SetKinematic(false);
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.CheckIsOnGround();
            monoBehaviour.AddForce(forceValue);
            monoBehaviour.GroundedHorizontalMovement(false, true);
            if (stateInfo.normalizedTime >= invincibleBeginTime && stateInfo.normalizedTime <= invincibleEndTime) {
                monoBehaviour.IsInvincible(true);
            }
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateExit(animator, stateInfo, layerIndex);
            if (!animator.GetBool(hashInvincible)) {
                monoBehaviour.IsInvincible(false);
            }
        }

    }
}

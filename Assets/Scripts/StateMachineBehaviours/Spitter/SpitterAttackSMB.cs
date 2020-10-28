using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVXIV {
    public class SpitterAttackSMB : SceneLinkedSMB<Spitter> {
        private bool hasAttack = false;
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.SetIdle();
            if(!hasAttack && stateInfo.normalizedTime >= 0.6f) {
                monoBehaviour.MakeBullet();
                hasAttack = true;
            }
            monoBehaviour.AliveCheck();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            hasAttack = false;
            base.OnSLStateExit(animator, stateInfo, layerIndex);
        }
    }
}

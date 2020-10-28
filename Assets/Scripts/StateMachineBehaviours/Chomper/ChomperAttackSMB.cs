using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVXIV {
    public class ChomperAttackSMB : SceneLinkedSMB<Chomper> {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.SetIdle();
            monoBehaviour.AliveCheck();
        }
    }
}

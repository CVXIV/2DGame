using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVXIV {
    public class ChomperLomotionSMB : SceneLinkedSMB<Chomper> {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            monoBehaviour.EmenyCheck();
            monoBehaviour.AliveCheck();
        }
    }
}

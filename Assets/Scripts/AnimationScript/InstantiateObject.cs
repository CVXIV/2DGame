using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CVXIV {
    public class InstantiateObject : StateMachineBehaviour {
        public GameObject prefab;
        [Range(0, 1)]
        public float timeDelay = 0;
        public bool isChildTransform = true;
        public Vector3 offset;
        public bool destroyOnExit = true;
        private GameObject example;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (timeDelay == 0)
                Spawn(animator);
        }

        private void Spawn(Animator animator) {
            example = Instantiate(prefab);
            example.transform.position = animator.transform.position + new Vector3(animator.transform.right.x * offset.x, offset.y);
            example.transform.rotation = animator.transform.rotation;
            if (isChildTransform) {
                example.transform.parent = animator.transform;
            }
            example.SetActive(true);
            IAnimationBuild animationBuild = example.GetComponent<IAnimationBuild>();
            if (animationBuild != null) {
                animationBuild.AnimationBuild();
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (timeDelay > 0 && stateInfo.normalizedTime >= timeDelay && example == null)
                Spawn(animator);

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (destroyOnExit && example != null)
                Destroy(example);
            example = null;
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }

}

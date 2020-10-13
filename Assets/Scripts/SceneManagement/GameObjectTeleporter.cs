using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVXIV {
    public class GameObjectTeleporter : Singleton<GameObjectTeleporter> {

        protected PlayerInput playerInput;


        protected override void Awake() {
            base.Awake();
            playerInput = FindObjectOfType<PlayerInput>();
            DontDestroyOnLoad(gameObject);
        }

        public static void Teleport(TransitionPoint transitionPoint) {
            SceneTransitionDestination sceneTransitionDestination = Instance.GetDestination(transitionPoint.transitionDestinationName);
            if (sceneTransitionDestination != null) {
                Instance.StartCoroutine(Instance.Transition(transitionPoint.transitioningGameObject, true, transitionPoint.resetInputValuesOnTransition, sceneTransitionDestination.transform.position, true));
            }
        }

        public static void Teleport(GameObject transitioningGameObject, Transform destination) {
            Instance.StartCoroutine(Instance.Transition(transitioningGameObject, false, false, destination.position, false));
        }

        public static void Teleport(GameObject transitioningGameObject, Vector3 destinationPosition) {
            Instance.StartCoroutine(Instance.Transition(transitioningGameObject, false, false, destinationPosition, false));
        }

        protected IEnumerator Transition(GameObject transitioningGameObject, bool releaseControl, bool resetInputValues, Vector3 destinationPosition, bool fade) {

            if (releaseControl) {
                if (playerInput == null) {
                    playerInput = FindObjectOfType<PlayerInput>();
                }
                playerInput.ReleaseControl(resetInputValues);
            }

            if (fade) {
                yield return StartCoroutine(ScreenFader.FadeSceneIn());
            }

            transitioningGameObject.transform.position = destinationPosition;

            if (fade) {
                yield return StartCoroutine(ScreenFader.FadeSceneOut());
            }

            if (releaseControl) {
                playerInput.GainControl();
            }
        }

        protected SceneTransitionDestination GetDestination(string destinationName) {
            SceneTransitionDestination[] entrances = FindObjectsOfType<SceneTransitionDestination>();
            for (int i = 0; i < entrances.Length; i++) {
                if (entrances[i].destinationName == destinationName)
                    return entrances[i];
            }
            Debug.LogWarning("No entrance was found with the " + destinationName + " name.");
            return null;
        }
    }
}

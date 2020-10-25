using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CVXIV {
    public class SceneController : Singleton<SceneController> {

        public SceneTransitionDestination initialSceneTransitionDestination;

        protected bool transitioning;

        /// <summary>
        /// 是否处于传送的过程中
        /// </summary>
        public static bool Transitioning {
            get {
                return Instance.transitioning;
            }
        }

        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            /*            if (initialSceneTransitionDestination != null) {
                            SetEnteringGameObjectLocation(initialSceneTransitionDestination);
                            ScreenFader.SetAlpha(1f);
                            StartCoroutine(ScreenFader.FadeSceneOut());
                            initialSceneTransitionDestination.OnReachDestination.Invoke();
                        } else {
                            currentZoneScene = SceneManager.GetActiveScene();
                            zoneRestartDestinationName = SceneTransitionDestination.DestinationTag.A;
                        }*/
        }

        public static void RestartZone(bool resetHealth = true) {
            if (resetHealth) {
                PlayerController.Instance.GetComponent<PlayerBeDamage>().ResetHealth();
            }
            Instance.StartCoroutine(Instance.Transition(SceneManager.GetActiveScene().name, true, null));
        }

        public static void RestartZoneWithDelay(float delay, bool resetHealth = true) {
            Instance.StartCoroutine(CallWithDelay(delay, RestartZone, resetHealth));
        }

        public static void TransitionToScene(TransitionPoint transitionPoint) {
            Instance.StartCoroutine(Instance.Transition(transitionPoint.newSceneName, transitionPoint.resetInputValuesOnTransition, transitionPoint.transitionDestinationName));
        }

        public static void PureLoadScene(string sceneName) {
            Instance.StartCoroutine(Instance.Transition(sceneName, true, null));
        }

        protected IEnumerator Transition(string newSceneName, bool resetInputValues, string destinationName) {
            transitioning = true;
            //PersistentDataManager.SaveAllData();
            // 加载场景前取消控制
            PlayerInput.Instance.ReleaseControl(resetInputValues);
            PlayerController.Instance.SetKinematic(true);
            PlayerController.Instance.gameObject.SetActive(false);
            yield return StartCoroutine(ScreenFader.FadeSceneIn(ScreenFader.FadeType.Loading));
            //PersistentDataManager.ClearPersisters();
            // 等待场景加载完毕才能继续设置人物的位置信息
            yield return StartCoroutine(ScreenFader.LoadScene(newSceneName));
            // 加载场景后取消控制
            PlayerInput.Instance.ReleaseControl(resetInputValues);
            //PersistentDataManager.LoadAllData();
            // 如果没有传递目的地索引，则寻找默认索引
            SceneTransitionDestination entrance = destinationName == null ? RestartLevelPos.Instance.restartPos : GetDestinationFromName(destinationName);
            SetEnteringGameObjectLocation(entrance);

            yield return StartCoroutine(ScreenFader.FadeSceneOut());
            PlayerInput.Instance.GainControl();
            transitioning = false;
        }

        public static SceneTransitionDestination GetDestinationFromName(string destinationName) {
            SceneTransitionDestination[] entrances = FindObjectsOfType<SceneTransitionDestination>();
            for (int i = 0; i < entrances.Length; i++) {
                if (entrances[i].destinationName == destinationName) {
                    return entrances[i];
                }
            }
            Debug.LogWarning("No entrance was found with the " + destinationName + " name.");
            return null;
        }

        /// <summary>
        /// 设置传送目标的新方位
        /// </summary>
        /// <param name="entrance"></param>
        protected void SetEnteringGameObjectLocation(SceneTransitionDestination entrance) {
            if (entrance == null) {
                Debug.LogWarning("Entering Transform's location has not been set.");
                return;
            }
            Transform entranceLocation = entrance.transform;
            Transform enteringTransform = GameObject.Find("Player").transform;
            enteringTransform.position = entranceLocation.position;
            enteringTransform.rotation = entranceLocation.rotation;

            entrance.OnReachDestination.Invoke();
        }


        private static IEnumerator CallWithDelay<T>(float delay, Action<T> call, T parameter) {
            yield return Yields.GetWaitForSeconds(delay);
            call(parameter);
        }

    }
}

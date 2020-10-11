using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CVXIV {
    public class SceneController : Singleton<SceneController> {

        public SceneTransitionDestination initialSceneTransitionDestination;

        protected Scene currentZoneScene;
        protected string zoneRestartDestinationName;
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

/*        public static void RestartZone(bool resetHealth = true) {
            if (resetHealth) {
                //PlayerCharacter.PlayerInstance.damageable.SetHealth(PlayerCharacter.PlayerInstance.damageable.startingHealth);
            }

            Instance.StartCoroutine(Instance.Transition(Instance.currentZoneScene.name, true, Instance.zoneRestartDestinationName, TransitionPoint.TransitionType.DifferentZone));
        }

        public static void RestartZoneWithDelay(float delay, bool resetHealth = true) {
            Instance.StartCoroutine(CallWithDelay(delay, RestartZone, resetHealth));
        }*/

        public static void TransitionToScene(TransitionPoint transitionPoint) {
            Instance.StartCoroutine(Instance.Transition(transitionPoint.newSceneName, transitionPoint.resetInputValuesOnTransition, transitionPoint.transitionDestinationName, transitionPoint.transitionType));
        }

        public static void PureLoadScene(string sceneName, string destinationName) {
            Instance.StartCoroutine(Instance.Transition(sceneName, true, destinationName));
        }

        protected IEnumerator Transition(string newSceneName, bool resetInputValues, string destinationName, TransitionPoint.TransitionType transitionType = TransitionPoint.TransitionType.DifferentZone) {
            transitioning = true;
            //PersistentDataManager.SaveAllData();

/*            if (m_PlayerInput == null)
                m_PlayerInput = FindObjectOfType<PlayerInput>();
            m_PlayerInput.ReleaseControl(resetInputValues);*/

            yield return StartCoroutine(ScreenFader.FadeSceneIn(ScreenFader.FadeType.Loading));
            //PersistentDataManager.ClearPersisters();
            // 这里不能使用协程，因为要等待场景加载完毕才能继续设置人物的位置信息
            yield return ScreenFader.LoadScene(newSceneName);
            /*            m_PlayerInput = FindObjectOfType<PlayerInput>();
                        m_PlayerInput.ReleaseControl(resetInputValues);
                        PersistentDataManager.LoadAllData();*/
            SceneTransitionDestination entrance = GetDestinationFromName(destinationName);
            SetEnteringGameObjectLocation(entrance);
            SetupNewScene(transitionType, entrance);
            if (entrance != null) {
                entrance.OnReachDestination.Invoke();
            }
            yield return StartCoroutine(ScreenFader.FadeSceneOut());
            // m_PlayerInput.GainControl();
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
        }

        /// <summary>
        /// 更新当前场景信息
        /// </summary>
        /// <param name="transitionType"></param>
        /// <param name="entrance"></param>
        protected void SetupNewScene(TransitionPoint.TransitionType transitionType, SceneTransitionDestination entrance) {
            if (entrance == null) {
                Debug.LogWarning("Restart information has not been set.");
                return;
            }

            if (transitionType == TransitionPoint.TransitionType.DifferentZone) {
                currentZoneScene = entrance.gameObject.scene;
                zoneRestartDestinationName = entrance.destinationName;
            }
        }


        private static IEnumerator CallWithDelay<T>(float delay, Action<T> call, T parameter) {
            yield return new WaitForSeconds(delay);
            call(parameter);
        }

    }
}

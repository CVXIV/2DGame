using UnityEngine;
namespace CVXIV {

    [RequireComponent(typeof(BoxCollider2D))]
    public class TransitionPoint : MonoBehaviour {
        public enum TransitionType {
            DifferentZone, DifferentNonGameplayScene, SameScene,
        }


        public enum TransitionWhen {
            ExternalCall, // 外部调用
            InteractPressed, // 鼠标点击
            OnTriggerEnter, // 触发器自动触发
        }


        public GameObject transitioningGameObject;
        [Tooltip("传送的类型")]
        public TransitionType transitionType;
        [SceneName]
        public string newSceneName;
        [Tooltip("异场景传送目的地")]
        public string transitionDestinationName;
        [Tooltip("同场景传送目的地")]
        public TransitionPoint destinationTransform;
        [Tooltip("触发传送的时机")]
        public TransitionWhen transitionWhen;
        [Tooltip("传送的过程中玩家会取消控制，是否重置输入信号")]
        public bool resetInputValuesOnTransition = true;



        private bool transitioningGameObjectPresent;

        private void Start() {
            if (transitionWhen == TransitionWhen.ExternalCall) {
                transitioningGameObjectPresent = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (transitionWhen != TransitionWhen.OnTriggerEnter) {
                return;
            }

            if (other.gameObject == transitioningGameObject) {
                transitioningGameObjectPresent = true;

                if (ScreenFader.IsFading || SceneController.Transitioning) {
                    return;
                }

                TransitionInternal();
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.gameObject == transitioningGameObject) {
                transitioningGameObjectPresent = false;
            }
        }

        private void Update() {
            if (ScreenFader.IsFading || SceneController.Transitioning) {
                return;
            }

            if (!transitioningGameObjectPresent) {
                return;
            }

            if (transitionWhen == TransitionWhen.InteractPressed) {
                if (PlayerInput.Instance.Interact.Down) {
                    TransitionInternal();
                }
            }
        }

        protected void TransitionInternal() {
            if (transitionType == TransitionType.SameScene) {
                GameObjectTeleporter.Teleport(transitioningGameObject, destinationTransform.transform);
            } else {
                SceneController.TransitionToScene(this);
            }
        }

        public void Transition() {
            if (!transitioningGameObjectPresent) {
                return;
            }

            if (transitionWhen == TransitionWhen.ExternalCall) {
                TransitionInternal();
            }
        }
    }
}

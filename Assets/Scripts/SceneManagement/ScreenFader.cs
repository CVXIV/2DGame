using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CVXIV {
    public class ScreenFader : Singleton<ScreenFader> {
        /// <summary>
        /// 黑屏、加载、游戏结束
        /// </summary>
        public enum FadeType {
            Black, Loading, GameOver,
        }

        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public CanvasGroup faderCanvasGroup;
        public CanvasGroup loadingCanvasGroup;
        public CanvasGroup gameOverCanvasGroup;
        public float fadeDuration = 1f;
        protected CanvasGroup currentActivateCanvasGroup;
        protected bool isFading;
        protected AsyncOperation info;


        public static bool IsFading {
            get { return Instance.isFading; }
        }

        public static void SetAlpha(float alpha) {
            Instance.faderCanvasGroup.alpha = alpha;
        }

        protected IEnumerator Fade(float finalAlpha, CanvasGroup canvasGroup) {
            isFading = true;
            canvasGroup.blocksRaycasts = true;
            float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / fadeDuration;
            // 由于浮点数不精确，不建议直接使用等于号来比较
            while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha)) {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
                yield return null;
            }
            canvasGroup.alpha = finalAlpha;
            isFading = false;
            canvasGroup.blocksRaycasts = false;
        }

        public static AsyncOperation LoadScene(string sceneName) {
            Instance.info = SceneManager.LoadSceneAsync(sceneName);
            Instance.currentActivateCanvasGroup.GetComponent<LoadScenePanel>().Show(Instance.info);
            return Instance.info;
        }

        public static IEnumerator FadeSceneIn(FadeType fadeType = FadeType.Black) {
            switch (fadeType) {
                case FadeType.Black:
                    Instance.currentActivateCanvasGroup = Instance.faderCanvasGroup;
                    break;
                case FadeType.GameOver:
                    Instance.currentActivateCanvasGroup = Instance.gameOverCanvasGroup;
                    break;
                default:
                    Instance.currentActivateCanvasGroup = Instance.loadingCanvasGroup;
                    break;
            }

            Instance.currentActivateCanvasGroup.gameObject.SetActive(true);

            yield return Instance.StartCoroutine(Instance.Fade(1f, Instance.currentActivateCanvasGroup));
        }

        public static IEnumerator FadeSceneOut() {
            if (Instance.currentActivateCanvasGroup == null) {
                yield break;
            }
            yield return Instance.StartCoroutine(Instance.Fade(0f, Instance.currentActivateCanvasGroup));
            Instance.currentActivateCanvasGroup.gameObject.SetActive(false);
            Instance.currentActivateCanvasGroup = null;
        }

    }
}

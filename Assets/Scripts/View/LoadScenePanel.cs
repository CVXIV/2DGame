using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScenePanel : MonoBehaviour {
    #region
    private Slider progress;
    private AsyncOperation info;
    #endregion

    private void Awake() {
        progress = GetComponentInChildren<Slider>();
    }

    private void Update() {
        if (info != null) {
            UpdateProgress(info.progress);
        }
    }

    private void UpdateProgress(float value) {
        progress.value = value;
        if (value >= 1) {
            Hide();
        }
    }

    public void Show(AsyncOperation asyncOperation) {
        this.gameObject.SetActive(true);
        this.info = asyncOperation;
    }

    public void Hide() {
        this.gameObject.SetActive(false);
        progress.value = 0;
        this.info = null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScenePanel : Singleton<LoadScenePanel> {
    #region
    private Slider progress;
    private AsyncOperation info;
    #endregion

    protected override void Awake() {
        base.Awake();
        progress = GetComponentInChildren<Slider>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if(info != null) {
            UpdateProgress(info.progress);
        }
    }

    private void UpdateProgress(float value) {
        progress.value = value;
        if(value >= 1) {
            Invoke(nameof(Hide), 1);
        }
    }

    public void Show(AsyncOperation asyncOperation) {
        this.gameObject.SetActive(true);
        this.info = asyncOperation;
    }

    public void Hide() {
        this.gameObject.SetActive(false);
        this.info = null;
    }
}
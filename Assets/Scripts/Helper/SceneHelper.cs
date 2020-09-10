using BTAI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHelper : PureSingleton<SceneHelper> {
    private AsyncOperation info;

    public void LoadScene(int index) {
        // 展示加载信息
        // 加载场景
        info = SceneManager.LoadSceneAsync(index);
        LoadScenePanel.Instance.Show(info);
    }

    private void LoadScene(int index, Action<AsyncOperation> complete) {
        this.LoadScene(index);
        // 将新的委托complete加入到原先info.completed后面执行
        info.completed += complete;
    }

    public void LoadScene(int index, string objName, string posName) {
        // lambda定义新委托的内容
        this.LoadScene(index, (asy) => {
            GameObject obj = GameObject.Find(objName);
            GameObject pos = GameObject.Find(posName);
            obj.transform.position = pos.transform.position;
        });
    }

}
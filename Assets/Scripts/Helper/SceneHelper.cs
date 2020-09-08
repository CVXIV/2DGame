using BTAI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHelper {
    private static readonly SceneHelper _instance = new SceneHelper();
    private AsyncOperation info;
    /// 显式的静态构造函数用来告诉C#编译器在其内容实例化之前不要标记其类型
    static SceneHelper() { }

    private SceneHelper() { }

    public static SceneHelper Instance {
        get {
            return _instance;
        }
    }

    public void LoadScene(int index) {
        // 展示加载信息
        if (LoadScenePanel._instance == null) {
            GameObject.Instantiate(Resources.Load<GameObject>("prefab/View/LoadScenePanel"));
        }
        // 加载场景
        info = SceneManager.LoadSceneAsync(index);
        LoadScenePanel._instance.Show(info);
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
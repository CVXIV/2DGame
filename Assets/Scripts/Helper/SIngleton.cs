using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    private static T _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                Instantiate(Resources.Load<GameObject>("prefab/View/" + typeof(T).Name));
            }
            return _instance;
        }
    }

    protected virtual void Awake() {
        _instance = this as T;
    }
}
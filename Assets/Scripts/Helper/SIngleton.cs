using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {

    private static T _instance;
    protected static readonly string prePath = "Singleton/";

    public static T Instance {
        get {
            if (_instance != null) {
                return _instance;
            }

            _instance = FindObjectOfType<T>();
            if (_instance != null) {
                return _instance;
            }

            _instance = Create();
            return _instance;
        }
    }

    private static T Create() {
        GameObject obj = Instantiate(Resources.Load<GameObject>(prePath + typeof(T).Name));
        return obj.GetComponent<T>();
    }

    protected virtual void Awake() {
        if (this != Instance) {
            Destroy(gameObject);
            return;
        }
    }
}
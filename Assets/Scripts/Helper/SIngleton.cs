using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    public static T _instance;
    protected virtual void Awake() {
        _instance = this as T;
    }
}
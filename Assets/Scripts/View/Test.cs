using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    Coroutine c;

    void Start() {
        c = StartCoroutine(Method1());
    }

    

    IEnumerator Method() {
        Debug.Log("协程停止");
        yield return new WaitForSeconds(2);
    }

    IEnumerator Method1() {
        yield return StartCoroutine(Method());
        Debug.Log("协程停止11");
    }
}
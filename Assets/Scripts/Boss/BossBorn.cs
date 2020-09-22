using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBorn : MonoBehaviour {
    public GameObject toShowObj;

    private void Awake() {
        if (toShowObj != null) {
            toShowObj.SetActive(false);
        }
    }

    public void OnPlayOver() {
        if (toShowObj != null) {
            toShowObj.SetActive(true);
        }
        gameObject.SetActive(false);
    }
}
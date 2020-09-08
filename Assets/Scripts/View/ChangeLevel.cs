using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour {

    public int level;
    public string objName = "Player";
    public string posName;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag(ConstantVar.PlayTag)) {
            if (string.IsNullOrEmpty(objName) || string.IsNullOrEmpty(posName)) {
                SceneHelper.Instance.LoadScene(level);
            } else {
                SceneHelper.Instance.LoadScene(level, objName, posName);
            }
        }
    }
}
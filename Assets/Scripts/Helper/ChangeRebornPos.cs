using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRebornPos : MonoBehaviour {
    public Damage[] toChange;
    public string targetPos;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (toChange != null) {
            foreach (var item in toChange) {
                item.resetPos = targetPos;
            }
        }
    }
}
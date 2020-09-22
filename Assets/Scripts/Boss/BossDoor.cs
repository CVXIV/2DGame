using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour {
    private Animator door;
    private Animator boss;
    private bool isPlayed = false;

    private void Awake() {
        door = GetComponent<Animator>();
        boss = transform.Find("Gunner_Born").GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!isPlayed && collision.CompareTag(ConstantVar.PlayTag)) {
            OpenDoor();
            isPlayed = true;
        }
    }

    private void OpenDoor() {
        door.Play("born_background");
    }

    public void OnPlayOver() {
        boss.Play("born");
    }
}
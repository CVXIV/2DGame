using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum HubDoorStatus {
    zero = 0,
    one = 1,
    two = 2,
    three = 3
}
public class HubDoor : MonoBehaviour {
    public Sprite[] status;
    private SpriteRenderer sr;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetStatus(HubDoorStatus s) {
        sr.sprite = status[(int)s];
    }

}
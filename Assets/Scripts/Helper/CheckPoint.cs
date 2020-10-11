using CVXIV;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CheckPoint : MonoBehaviour {
    private BoxCollider2D boxCollider2D;

    private void Awake() {
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        PlayerController characterControl = collision.GetComponent<PlayerController>();
        if (characterControl != null) {
            characterControl.CheckPoint(this);
        }
    }

}
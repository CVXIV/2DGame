using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour {

    Damage damage;

    private void Awake() {
        damage = GetComponent<Damage>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // 对游戏物体造成伤害
        damage.Attack(collision.gameObject);
    }
}
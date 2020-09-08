using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour {
    public int damage;

    public void Attack(GameObject gb) {
        BeDamage beDamage = gb.GetComponent<BeDamage>();
        if (beDamage != null) {
            beDamage.TakeDamage(damage);
        }
    }

    public void Attack(GameObject[] gbs) {
        foreach (GameObject gb in gbs) {
            Attack(gb);
        }
    }
}
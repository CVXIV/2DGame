using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour {

    private readonly HashSet<BeDamage> objs = new HashSet<BeDamage>();

    private void OnTriggerEnter2D(Collider2D collision) {
        BeDamage beDamage = collision.GetComponent<BeDamage>();
        if (beDamage != null) {
            objs.Add(beDamage);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        objs.Remove(collision.GetComponent<BeDamage>());
    }

    public HashSet<BeDamage> GetDamageObjs() {
        return objs;
    }
}
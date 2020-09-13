using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour {

    private readonly HashSet<GameObject> objs = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag(ConstantVar.PlayTag)) {
            BeDamage beDamage = collision.GetComponent<BeDamage>();
            if (beDamage != null) {
                objs.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        objs.Remove(collision.gameObject);
    }

    public List<GameObject> GetDamageObjs() {
        return new List<GameObject>(objs);
    }
}
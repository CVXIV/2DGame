using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType {
    Normal,
    Dead
}

public class Damage : MonoBehaviour {
    public int damage;
    public DamageType damageType;
    public string resetPos;
    public LayerMask hittableLayers;
    [HideInInspector]
    public ContactFilter2D attackContactFilter;

    private void Awake() {
        attackContactFilter.layerMask = hittableLayers;
        attackContactFilter.useLayerMask = true;
    }

    public void Attack(GameObject gb) {
        BeDamage beDamage = gb.GetComponent<BeDamage>();
        if (beDamage != null) {
            beDamage.TakeDamage(damage, damageType, resetPos);
        }
    }

    public void Attack(GameObject[] gbs) {
        foreach (GameObject gb in gbs) {
            Attack(gb);
        }
    }
}
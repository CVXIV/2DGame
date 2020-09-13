using System;
using UnityEngine;

public class BeDamage : MonoBehaviour {
    [SerializeField]
    protected int health;
    public int Health {
        get {
            return health;
        }
        set {
            health = value;
        }
    }

    public Action<DamageType, string> onHurt;
    public Action<string> onDead;
    private bool isEnable = true;

    public void Enable() {
        isEnable = true;
    }

    public void Disable() {
        isEnable = false;
    }

    public virtual void TakeDamage(int value, DamageType damageType, string resetPos) {
        if (isEnable && health > 0) {
            health -= value;
            if (health <= 0) {
                onDead?.Invoke(resetPos);
            } else {
                onHurt?.Invoke(damageType, resetPos);
            }
        }
    }
}
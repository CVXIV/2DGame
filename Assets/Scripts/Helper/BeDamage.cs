using System;
using UnityEngine;

public class BeDamage : MonoBehaviour {
    [SerializeField]
    protected int health;
    protected int maxHealth;
    public int Health {
        get {
            return health;
        }
    }

    public Action<DamageType, string, int> onHurt;
    public Action<string, int> onDead;
    private bool isEnable = true;

    protected virtual void Awake() {
        maxHealth = health;
    }

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
                onDead?.Invoke(resetPos, value);
            } else {
                onHurt?.Invoke(damageType, resetPos, value);
            }
        }
    }

    public virtual void RecoverHealth(int value) {
        health += value;
    }

    public virtual void ResetHealth() {
        health = maxHealth;
    }

    public void UpdateMaxHealth(int value) {
        health = maxHealth = value;
    }

}
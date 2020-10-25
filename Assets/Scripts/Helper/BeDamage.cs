using System;
using UnityEngine;

public class BeDamage : MonoBehaviour {
    [SerializeField]
    protected int health;
    protected int maxHealth;
    public int MaxHealth {
        get {
            return maxHealth;
        }
    }
    public int Health {
        get {
            return health;
        }
    }

    public Action<DamageType, int> onHurt;
    public Action<int> onDead;
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

    public virtual void TakeDamage(int value, DamageType damageType) {
        if (isEnable && health > 0) {
            health -= value;
            if (health <= 0) {
                onDead?.Invoke(value);
            } else {
                onHurt?.Invoke(damageType, value);
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
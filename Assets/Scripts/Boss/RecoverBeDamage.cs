using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverBeDamage : BeDamage {
    private int maxHealth;

    private void Awake() {
        maxHealth = health;
    }

    public void ResetHealth() {
        health = maxHealth;
    }
}
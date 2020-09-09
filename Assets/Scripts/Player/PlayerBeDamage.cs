using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBeDamage : BeDamage {
    private void Start() {
        health = ConstantVar.MAXHP;
        GamePanel.Instance.InitHP(health);
    }

    public void Reset() {
        health = ConstantVar.MAXHP;
        GamePanel.Instance.ResetHP();
        Enable();
    }

    public override void TakeDamage(int value, DamageType damageType, string resetPos) {
        base.TakeDamage(value, damageType, resetPos);
        if (health >= 0) {
            GamePanel.Instance.UpdateHP(health);
        }
    }
}
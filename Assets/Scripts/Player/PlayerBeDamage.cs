using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBeDamage : BeDamage {
    private void Start() {
        if (!(DataManager.Instance.GetData(ConstantVar.player_hp) is Data1Value<int> data)) {
            health = ConstantVar.MAXHP;
            DataManager.Instance.SaveData(ConstantVar.player_hp, new Data1Value<int> {
                Value1 = health
            });
        } else {
            health = data.Value1;
        }
        GamePanel.Instance.InitHP(health, ConstantVar.MAXHP);
    }

    public void Reset() {
        health = ConstantVar.MAXHP;
        DataManager.Instance.SaveData(ConstantVar.player_hp, new Data1Value<int> {
            Value1 = health
        });
        GamePanel.Instance.ResetHP();
        Enable();
    }

    public override void TakeDamage(int value, DamageType damageType, string resetPos) {
        base.TakeDamage(value, damageType, resetPos);
        DataManager.Instance.SaveData(ConstantVar.player_hp, new Data1Value<int> {
            Value1 = health
        });
        if (health >= 0) {
            GamePanel.Instance.UpdateHP(health);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {
    public Sprite hasWeapon, noWeapon;
    private bool isWeapon;
    private SpriteRenderer render;

    private void Awake() {
        render = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        Init();
    }

    private void Init() {
        isWeapon = DataManager.Instance.GetData(ConstantVar.weapon_key) is Data1Value<bool> data && data.Value1;
        render.sprite = isWeapon ? hasWeapon : noWeapon;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(ConstantVar.PlayTag)) {
            DataManager.Instance.SaveData(ConstantVar.weapon_key, new Data1Value<bool> {
                Value1 = true
            });
            render.sprite = hasWeapon;
            GetComponent<BoxCollider2D>().enabled = false;
            collision.GetComponent<CharacterControl>().IsHasWeapon = true;
            TipMessagePanel.Instance.Show("啊哈，拿到武器了！", TipStyle.Buttom);
            Invoke(nameof(HideTip), 1.5f);
        }
    }

    private void HideTip() {
        TipMessagePanel.Instance.Hide(TipStyle.Buttom);
    }
}
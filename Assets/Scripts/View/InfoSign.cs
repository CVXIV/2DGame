using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoSign : MonoBehaviour {

    private SpriteRenderer sprite;
    public string content;
    public Sprite normal;
    public Sprite highlight;

    private void Awake() {
        this.sprite = GetComponent<SpriteRenderer>();
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        sprite.sprite = highlight;
        // 展示提示
        TipMessagePanel._instance.Show(content, TipStyle.Buttom);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        sprite.sprite = normal;
        // 隐藏提示
        TipMessagePanel._instance.Hide(TipStyle.Buttom);
    }
}
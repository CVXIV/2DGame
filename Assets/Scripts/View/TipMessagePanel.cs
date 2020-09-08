using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TipStyle {
    Buttom
}

public class TipMessagePanel : Singleton<TipMessagePanel> {
    #region 字段
    public GameObject style;
    private Text content;
    #endregion

    private void Start() {
        style.SetActive(false);
        content = style.transform.Find("Content").GetComponent<Text>();
    }

    public void Show(string text, TipStyle tipStyle) {
        switch (tipStyle) {
            case TipStyle.Buttom:
                content.text = text;
                style.SetActive(true);
                break;
        }

    }

    public void Hide(TipStyle tipStyle) {
        switch (tipStyle) {
            case TipStyle.Buttom:
                style.SetActive(false);
                break;
        }
    }

}
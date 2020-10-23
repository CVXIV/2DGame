using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TipStyle {
    Buttom,
    FullScreen,
    GameOver
}

public class TipMessagePanel : Singleton<TipMessagePanel> {
    #region 字段
    private GameObject style1;
    private Text content;
    #endregion

    private void Start() {
        style1 = transform.Find("Style1").gameObject;
        style1.SetActive(false);
        content = style1.transform.Find("Content").GetComponent<Text>();
    }

    public void Show(string text, TipStyle tipStyle) {
        switch (tipStyle) {
            case TipStyle.Buttom:
                content.text = text;
                style1.SetActive(true);
                break;
        }

    }

    private void HideFullScreen() {
        Hide(TipStyle.FullScreen);
    }

    public void Hide(TipStyle tipStyle) {
        switch (tipStyle) {
            case TipStyle.Buttom:
                style1.SetActive(false);
                break;
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyButtons : BaseButtons {
    #region 字段
    private Transform logo;
    #endregion

    #region 初始化
    private void Awake() {
        Init();
    }

    public override void Init() {
        base.Init();
        Image[] images = GetComponentsInChildren<Image>();
        foreach (var image in images) {
            if (image.name == "btn_logo") {
                logo = image.transform;
                break;
            }
        }
        logo.gameObject.SetActive(false);
    }
    #endregion

    #region 事件监听
    public override void OnPointerEnter() {
        base.OnPointerEnter();
        logo.gameObject.SetActive(true);
    }

    public override void OnPointerExit() {
        base.OnPointerExit();
        logo.gameObject.SetActive(false);
    }

    public override void OnPointerUp() {
        base.OnPointerUp();
        logo.gameObject.SetActive(false);
    }
    #endregion



}
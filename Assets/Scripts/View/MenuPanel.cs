﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : BasePanel {

    #region 字段
    public OptionPanel optionPanel;
    #endregion

    #region 点击事件
    public void OnStartClick() {
        SceneHelper.Instance.LoadScene(ConstantVar.sceneLeven1);
    }

    public void OnSetClick() {
        this.Hide();
        optionPanel.Show();
    }

    public void OnExitClick() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanel : BasePanel {


    #region 字段
    public GameObject defaultPanel;
    public MenuPanel menuPanel;
    public OptionAudioPanel optionAudioPanel;
    public OptionControlPanel optionControlPanel;
    #endregion

    #region 点击事件
    public void OnAudioClick() {
        defaultPanel.SetActive(false);
        optionAudioPanel.Show();
    }

    public void OnControlClick() {
        defaultPanel.SetActive(false);
        optionControlPanel.Show();
    }

    public void OnReturnClick() {
        if (optionAudioPanel.IsActive()|| optionControlPanel.IsActive()) {
            optionAudioPanel.Hide();
            optionControlPanel.Hide();
            defaultPanel.SetActive(true);
        } else {
            this.Hide();
            menuPanel.Show();
        }
        
    }

    #endregion
}
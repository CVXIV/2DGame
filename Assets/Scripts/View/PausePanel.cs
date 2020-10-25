using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CVXIV {
    public class PausePanel : BasePanel {

        public OptionPanel optionPanel;

        public void ExitPause() {
            PlayerController.Instance.Unpause();
        }

        public void RestartLevel() {
            ExitPause();
            SceneController.RestartZone();
        }

        public void OnSetClick() {
            optionPanel.Show();
        }

        public void OnExitClick() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}


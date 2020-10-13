using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CVXIV {
    public class PausePanel : BasePanel {

        public OptionPanel optionPanel;
        private PlayerController playerController;

        private void Awake() {
            playerController = FindObjectOfType<PlayerController>();
        }

        public void ExitPause() {
            if (playerController == null) {
                playerController = FindObjectOfType<PlayerController>();
            }
            playerController.Unpause();
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


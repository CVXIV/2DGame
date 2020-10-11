using System;
using UnityEditor;
using UnityEngine;
namespace CVXIV {

    /// <summary>
    /// 实现scene下拉框
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameDrawer : PropertyDrawer {
        int sceneIndex = -1;
        // 下拉框展示的内容
        GUIContent[] sceneNames;
        readonly string[] scenePathSplitters = { "/", ".unity" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // 使用BeginProperty/EndProperty表示预制体修改逻辑也适用于这个属性
            //EditorGUI.BeginProperty(position, label, property);
            if (EditorBuildSettings.scenes.Length == 0) {
                return;
            }
            if (sceneIndex == -1) {
                Setup(property);
            }
            
            sceneIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);
            property.stringValue = sceneNames[sceneIndex].text;
            //EditorGUI.EndProperty();
        }

        private void Setup(SerializedProperty property) {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            sceneNames = new GUIContent[scenes.Length];

            for (int i = 0; i < sceneNames.Length; i++) {
                string path = scenes[i].path;
                string[] splitPath = path.Split(scenePathSplitters, StringSplitOptions.RemoveEmptyEntries);
                string sceneName = splitPath.Length > 0 ? splitPath[splitPath.Length - 1] : "(Deleted Scene)";
                sceneNames[i] = new GUIContent(sceneName);
            }

            if (sceneNames.Length == 0) {
                sceneNames = new[] { new GUIContent("[No Scenes In Build Settings]") };
            }

            // 更新展示的值，为了使得编辑时和运行时的值一致
            if (!string.IsNullOrEmpty(property.stringValue)) {
                bool sceneNameFound = false;
                for (int i = 0; i < sceneNames.Length; i++) {
                    if (sceneNames[i].text == property.stringValue) {
                        sceneIndex = i;
                        sceneNameFound = true;
                        break;
                    }
                }
                if (!sceneNameFound) {
                    sceneIndex = 0;
                }
            } else { 
                sceneIndex = 0; 
            }

            property.stringValue = sceneNames[sceneIndex].text;
        }
    }

}


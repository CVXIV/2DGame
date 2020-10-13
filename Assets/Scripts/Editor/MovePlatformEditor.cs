using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 自定义MovePlatform编辑页面
/// </summary>
[CustomEditor(typeof(MovePlatform))]
public class MovePlatformEditor : Editor {
    MovePlatform movePlatform;


    private void OnEnable() {
        movePlatform = target as MovePlatform;
    }

    /// <summary>
    /// Undo用来保存需要撤销的操作顺序
    /// </summary>
    public override void OnInspectorGUI() {

        EditorGUI.BeginChangeCheck();
        bool isMoving = EditorGUILayout.Toggle("isMoving", movePlatform.isMoving);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Changed isMoving");
            movePlatform.isMoving = isMoving;
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUI.BeginChangeCheck();
        MovePlatform.MovePlatformType platformType = (MovePlatform.MovePlatformType)EditorGUILayout.EnumPopup("Looping", movePlatform.movePlatformType);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Changed Moving Platform type");
            movePlatform.movePlatformType = platformType;
        }

        EditorGUI.BeginChangeCheck();
        float newSpeed = EditorGUILayout.FloatField("Speed", movePlatform.speed);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Changed Speed");
            movePlatform.speed = newSpeed;
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (GUILayout.Button("Add Node")) {
            Undo.RecordObject(target, "added node");
            Vector3 position = movePlatform.localNodes[movePlatform.localNodes.Length - 1];

            ArrayUtility.Add(ref movePlatform.localNodes, position);
            ArrayUtility.Add(ref movePlatform.waitTimes, 0);
        }

        // 设置label的宽度
        EditorGUIUtility.labelWidth = 64;
        int delete = -1;
        for (int i = 0; i < movePlatform.localNodes.Length; ++i) {
            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();

            int size = 64;
            EditorGUILayout.BeginVertical(GUILayout.Width(size));
            EditorGUILayout.LabelField("Node " + i, GUILayout.Width(size));
            // 当i>0时才有delete按钮
            if (i != 0 && GUILayout.Button("Delete", GUILayout.Width(size))) {
                delete = i;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            Vector3 newPosition;
            if (i == 0)
                // 第一个地点是默认初始位置，不用修改
                newPosition = movePlatform.localNodes[i];
            else
                newPosition = EditorGUILayout.Vector3Field("Destination", movePlatform.localNodes[i]);

            float newTime = EditorGUILayout.FloatField("Wait Time", movePlatform.waitTimes[i]);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "changed time or position");
                movePlatform.waitTimes[i] = newTime;
                movePlatform.localNodes[i] = newPosition;
            }
        }
        EditorGUIUtility.labelWidth = 0;

        if (delete != -1) {
            Undo.RecordObject(target, "Removed point moving platform");

            ArrayUtility.RemoveAt(ref movePlatform.localNodes, delete);
            ArrayUtility.RemoveAt(ref movePlatform.waitTimes, delete);
        }
    }

    private void OnSceneGUI() {
        for (int i = 0; i < movePlatform.localNodes.Length; ++i) {
            Vector3 worldPos;
            if (Application.isPlaying) {
                worldPos = movePlatform.WorldNode[i];
            } else {
                worldPos = movePlatform.transform.TransformPoint(movePlatform.localNodes[i]);
            }

            // 展示坐标
            Vector3 newWorld = worldPos;
            if (i != 0)
                newWorld = Handles.PositionHandle(worldPos, Quaternion.identity);

            Handles.color = Color.red;

            if (i == 0) {
                if (movePlatform.movePlatformType != MovePlatform.MovePlatformType.LOOP)
                    continue;

                if (Application.isPlaying) {
                    Handles.DrawDottedLine(worldPos, movePlatform.WorldNode[movePlatform.WorldNode.Length - 1], 10);
                } else {
                    Handles.DrawDottedLine(worldPos, movePlatform.transform.TransformPoint(movePlatform.localNodes[movePlatform.localNodes.Length - 1]), 10);
                }
            } else {
                if (Application.isPlaying) {
                    Handles.DrawDottedLine(worldPos, movePlatform.WorldNode[i - 1], 10);
                } else {
                    Handles.DrawDottedLine(worldPos, movePlatform.transform.TransformPoint(movePlatform.localNodes[i - 1]), 10);
                }

                if (worldPos != newWorld) {
                    Undo.RecordObject(target, "moved point");
                    movePlatform.localNodes[i] = movePlatform.transform.InverseTransformPoint(newWorld);
                }
            }
        }
    }
}
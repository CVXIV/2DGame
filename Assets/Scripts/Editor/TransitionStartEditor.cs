using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CVXIV {

    [CustomEditor(typeof(TransitionPoint))]
    public class TransitionStartEditor : Editor {
        SerializedProperty transitioningGameObjectProp;
        SerializedProperty transitionTypeProp;
        SerializedProperty newSceneNameProp;
        SerializedProperty transitionDestinationNameProp;
        SerializedProperty destinationTransformProp;
        SerializedProperty transitionWhenProp;
/*        SerializedProperty m_ResetInputValuesOnTransitionProp;
        SerializedProperty m_RequiresInventoryCheckProp;
        SerializedProperty m_InventoryControllerProp;
        SerializedProperty m_InventoryCheckProp;
        SerializedProperty m_InventoryItemsProp;
        SerializedProperty m_OnHasItemProp;
        SerializedProperty m_OnDoesNotHaveItemProp;*/

        GUIContent[] inventoryControllerItems = new GUIContent[0];

        // serializedObject表示TransitionPoint类的一个实例
        void OnEnable() {
            transitioningGameObjectProp = serializedObject.FindProperty("transitioningGameObject");
            transitionTypeProp = serializedObject.FindProperty("transitionType");
            newSceneNameProp = serializedObject.FindProperty("newSceneName");
            transitionDestinationNameProp = serializedObject.FindProperty("transitionDestinationName");
            destinationTransformProp = serializedObject.FindProperty("destinationTransform");
            transitionWhenProp = serializedObject.FindProperty("transitionWhen");
            //m_ResetInputValuesOnTransitionProp = serializedObject.FindProperty("resetInputValuesOnTransition");
            //m_RequiresInventoryCheckProp = serializedObject.FindProperty("requiresInventoryCheck");
            //m_InventoryControllerProp = serializedObject.FindProperty("inventoryController");
            //m_InventoryCheckProp = serializedObject.FindProperty("inventoryCheck");
            //m_InventoryItemsProp = m_InventoryCheckProp.FindPropertyRelative("inventoryItems");
            //m_OnHasItemProp = m_InventoryCheckProp.FindPropertyRelative("OnHasItem");
            //m_OnDoesNotHaveItemProp = m_InventoryCheckProp.FindPropertyRelative("OnDoesNotHaveItem");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(transitioningGameObjectProp);

            EditorGUILayout.PropertyField(transitionTypeProp);
            // indentLevel++表示进行缩进
            EditorGUI.indentLevel++;
            if ((TransitionPoint.TransitionType)transitionTypeProp.enumValueIndex == TransitionPoint.TransitionType.SameScene) {
                EditorGUILayout.PropertyField(destinationTransformProp);
            } else {
                EditorGUILayout.PropertyField(newSceneNameProp);
                EditorGUILayout.PropertyField(transitionDestinationNameProp);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(transitionWhenProp);
            //EditorGUILayout.PropertyField(m_ResetInputValuesOnTransitionProp);

            /*EditorGUILayout.PropertyField(m_RequiresInventoryCheckProp);
            if (m_RequiresInventoryCheckProp.boolValue) {
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_InventoryControllerProp);
                if (EditorGUI.EndChangeCheck() || (m_InventoryControllerProp.objectReferenceValue != null && inventoryControllerItems.Length == 0)) {
                    SetupInventoryItemGUI();
                }

                if (m_InventoryControllerProp.objectReferenceValue != null) {
                    InventoryController controller = m_InventoryControllerProp.objectReferenceValue as InventoryController;
                    m_InventoryItemsProp.arraySize = EditorGUILayout.IntField("Inventory Items", m_InventoryItemsProp.arraySize);
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < m_InventoryItemsProp.arraySize; i++) {
                        SerializedProperty elementProp = m_InventoryItemsProp.GetArrayElementAtIndex(i);

                        int itemIndex = ItemIndexFromController(controller, elementProp.stringValue);
                        if (itemIndex == -1) {
                            EditorGUILayout.LabelField("No items found in controller");
                        } else if (itemIndex == -2) {
                            elementProp.stringValue = inventoryControllerItems[0].text;
                        } else if (itemIndex == -3) {
                            Debug.LogWarning("Previously listed item to check not found, resetting to item index 0");
                            elementProp.stringValue = inventoryControllerItems[0].text;
                        } else {
                            itemIndex = EditorGUILayout.Popup(new GUIContent("Item " + i), itemIndex, inventoryControllerItems);
                            elementProp.stringValue = inventoryControllerItems[itemIndex].text;
                        }

                    }
                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(m_OnHasItemProp);
                    EditorGUILayout.PropertyField(m_OnDoesNotHaveItemProp);
                } else {
                    for (int i = 0; i < m_InventoryItemsProp.arraySize; i++) {
                        SerializedProperty elementProp = m_InventoryItemsProp.GetArrayElementAtIndex(i);
                        elementProp.stringValue = "";
                    }
                }

                EditorGUI.indentLevel--;
            }*/

            serializedObject.ApplyModifiedProperties();
        }

/*        void SetupInventoryItemGUI() {
            if (m_InventoryControllerProp.objectReferenceValue == null)
                return;

            InventoryController inventoryController = m_InventoryControllerProp.objectReferenceValue as InventoryController;

            inventoryControllerItems = new GUIContent[inventoryController.inventoryEvents.Length];
            for (int i = 0; i < inventoryController.inventoryEvents.Length; i++) {
                inventoryControllerItems[i] = new GUIContent(inventoryController.inventoryEvents[i].key);
            }
        }*/

/*        int ItemIndexFromController(InventoryController controller, string itemName) {
            if (controller.inventoryEvents.Length == 0)
                return -1;

            if (string.IsNullOrEmpty(itemName))
                return -2;

            for (int i = 0; i < controller.inventoryEvents.Length; i++) {
                if (controller.inventoryEvents[i].key == itemName)
                    return i;
            }
            return -3;
        }*/
    }
}


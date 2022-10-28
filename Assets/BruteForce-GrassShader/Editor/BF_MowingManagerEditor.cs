using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

public class BF_MowingManagerEditor : Editor
{
    [CustomEditor(typeof(BF_MowingManager))]
    class MowingEditor : Editor
    {
        SerializedProperty precisionValue;
        SerializedProperty marginError;
        private GUIStyle style;
        private GUIStyle styleDebug;
        private bool isDebugShown = false;
        void OnEnable()
        {
            precisionValue = serializedObject.FindProperty("precisionValue");
            marginError = serializedObject.FindProperty("marginError");
        }
        public override void OnInspectorGUI()
        {
            BF_MowingManager myTarget = (BF_MowingManager)target;
            EditorGUILayout.PropertyField(precisionValue);
            EditorGUILayout.PropertyField(marginError);

            if (style == null)
            {
                style = new GUIStyle(GUI.skin.button);
                styleDebug = new GUIStyle(GUI.skin.button);
            }
            if (GUILayout.Button("Generate Markers", style))
            {
                myTarget.CreateMowingMarker(isDebugShown, precisionValue.intValue);
                style.normal.background = Texture2D.linearGrayTexture;
            }

            if (!isDebugShown)
            {
                if (GUILayout.Button("Show Debug", styleDebug))
                {
                    styleDebug.normal.background = Texture2D.whiteTexture;
                    isDebugShown = true;
                }
            }
            else
            {
                if (GUILayout.Button("Hide Debug", styleDebug))
                {
                    styleDebug.normal.background = Texture2D.linearGrayTexture;
                    isDebugShown = false;
                }
            }
            myTarget.changeDebugState(isDebugShown);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

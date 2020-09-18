using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GamemodeManager)), CanEditMultipleObjects]
public class GamemodeManagerEditor : Editor {

    public SerializedProperty
        type,
        forTVT,
        forFFA;

    void OnEnable() {
        // Setup the SerializedProperties
        type = serializedObject.FindProperty("type");
        forTVT = serializedObject.FindProperty("forTVT");
        forFFA = serializedObject.FindProperty("forFFA");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(type);

        GamemodeType st = (GamemodeType)type.enumValueIndex;

        switch (st) {
            case GamemodeType.TeamVsTeam:
            EditorGUILayout.IntSlider(forFFA, 0, 10, new GUIContent("forFFA"));
            EditorGUILayout.IntSlider(forTVT, 0, 100, new GUIContent("forTVT"));
            break;

            case GamemodeType.FreeForAll:
            EditorGUILayout.IntSlider(forTVT, 0, 100, new GUIContent("valForAB"));
            break;
        }


        serializedObject.ApplyModifiedProperties();
    }
}
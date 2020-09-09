using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractablesList : MonoBehaviour {

    public static InteractablesList single_IaList;
    [Header("HideInInspector")]
    public List<Interactable> interactables = new List<Interactable>();

    private void Awake() {
        single_IaList = this;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractablesList))]
public class InteractablesListEditor : Editor {

    InteractablesList target_IaList;

    public void OnEnable() {
        target_IaList = (InteractablesList)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Set interactable list")) {
            Interactable[] iaArray = FindObjectsOfType<Interactable>();
            List<Interactable> iaList = new List<Interactable>();
            int i;
            for (i = 0; i < iaArray.Length; i++) {
                Interactable interactable = iaArray[i];
                interactable.index = i;
                iaList.Add(interactable);
                EditorUtility.SetDirty(interactable);
            }
            target_IaList.interactables = iaList;
            Debug.LogWarning($"Successfully set {i} interactables, don't forget to save!");
        }
    }
}
#endif
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractablesInSceneList : MonoBehaviour {
    public static InteractablesInSceneList single_PPL;
    [HideInInspector] public List<InteractableObject> interactableProductList = new List<InteractableObject>();
    private void Awake() {
        single_PPL = this;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractablesInSceneList))]
public class PhotonProductListEditor : Editor {
    InteractablesInSceneList iaTarget;
    public void OnEnable() {
        iaTarget = (InteractablesInSceneList)target;
    }
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Set Interactable List")) {
            SetInteractableList();
            Debug.LogWarning("Executed, don't forget to save!");
        }
    }

    void SetInteractableList() {
        InteractableObject[] iaInScene = FindObjectsOfType<InteractableObject>();
        List<InteractableObject> interactList = new List<InteractableObject>();
        iaTarget.interactableProductList.Clear();
        for (int i = 0; i < iaInScene.Length; i++) {
            InteractableObject tempIaInScene = iaInScene[i];
            interactList.Add(tempIaInScene);
            tempIaInScene.colliders = GetColliders(tempIaInScene.transform);
            if (tempIaInScene.scriptableObject) {
                tempIaInScene.scriptableObject = CreateInstance(tempIaInScene.scriptableObject.GetType()) as Interactable_SOBJ;
                tempIaInScene.scriptableObject.index = i;
            }
            //tempIaInScene.gameObject.layer = 11;
            EditorUtility.SetDirty(tempIaInScene);
        }
        iaTarget.interactableProductList = interactList;
    }

    Collider[] GetColliders(Transform ia_Transform) {
        Collider[] colliders;
        colliders = ia_Transform.GetComponents<Collider>();
        if(ia_Transform.childCount > 0) {
            Collider[] childColliders = ia_Transform.GetComponentsInChildren<Collider>();
            if(childColliders.Length > 0) {
                List<Collider> allColliders = new List<Collider>();
                List<Collider[]> colliderArrayList = new List<Collider[]>();
                colliderArrayList.Add(colliders);
                colliderArrayList.Add(childColliders);
                for (int i = 0; i < colliderArrayList.Count; i++) {
                    Collider[] cArr = colliderArrayList[i];
                    for (int iB = 0; iB < cArr.Length; iB++) {
                        allColliders.Add(cArr[i]);
                    }
                }
                colliders = allColliders.ToArray();
            }
        }
        return colliders;
    }    
}
#endif
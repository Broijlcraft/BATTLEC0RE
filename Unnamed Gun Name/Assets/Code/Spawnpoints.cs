using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;

public class Spawnpoints : MonoBehaviourPunCallbacks {
    public static Spawnpoints sp_Single;
    [HideInInspector] public Transform[] spawnpoints;

    private void Awake() {
        sp_Single = this;
    }

    public void SetSpPositionAndRotation(Transform modelTransform, int index) {
        modelTransform.position = spawnpoints[index].position;
        modelTransform.rotation = spawnpoints[index].rotation;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Spawnpoints))]
    public class SpawnPointEditor : Editor {
        Spawnpoints target_SpawnPoints;
        public void OnEnable() {
            target_SpawnPoints = (Spawnpoints)target;
        }
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (GUILayout.Button("Set spawnpoints(from top to bottom)")) {
                List<Transform> spawnpoints = new List<Transform>();
                for (int i = 0; i < target_SpawnPoints.transform.childCount; i++) {
                    Transform sp = target_SpawnPoints.transform.GetChild(i);
                    sp.name = $"Spawnpoint ({i})";
                    spawnpoints.Add(sp);
                    EditorUtility.SetDirty(spawnpoints[spawnpoints.Count-1]);
                }
                target_SpawnPoints.spawnpoints = spawnpoints.ToArray();
                Debug.LogWarning($"Successfully set {spawnpoints.Count} spawnpoints, don't forget to save!");
            }
        }
    }
#endif
}
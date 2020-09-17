using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartInRig : MonoBehaviour {
    public BodypartType bodypartType;

    [HideInInspector] public int index;
    public CustomRobotPart robotPart;
    public SkinnedMeshRenderer skinMeshRenderes;

    void Start() {
        if (robotPart) {
            skinMeshRenderes.sharedMesh = robotPart.meshFilter.sharedMesh;
            skinMeshRenderes.materials = robotPart.meshRenderer.sharedMaterials;
        }
    }
}
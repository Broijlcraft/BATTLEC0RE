using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartsManager : MonoBehaviour {
    public List<CustomRobotPart> customRobotParts = new List<CustomRobotPart>();

    [Space(30)]
    public Material normalMat;
    public Material normalMatEm;
    public Material heavyMat;
    public Material heavyMatEm;

    private void Start() {
        //for (int i = 0; i < 20; i++) {
        //    CustomRobotPart part = new CustomRobotPart();
        //    part.bodypartType = (BodypartType)i;
        //    for (int iB = 0; iB < 2; iB++) {
        //        Material mat = normalMat;
        //        if (iB == 1) {
        //            mat = heavyMat;
        //        }
        //        MeshAndMats meshAndMats = new MeshAndMats();
        //        meshAndMats.mats.Add(mat);
        //        part.variants.Add(meshAndMats);
        //    }
        //    customRobotParts.Add(part);
        //}
    }
}

[Serializable]
public class CustomRobotPart {
    [HideInInspector] public string partName;
    public BodypartType bodypartType;

    public List<Mesh> meshes = new List<Mesh>();
}

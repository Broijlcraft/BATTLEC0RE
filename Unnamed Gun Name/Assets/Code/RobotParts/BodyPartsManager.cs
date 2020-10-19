using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartsManager : MonoBehaviour {
    public List<CustomRobotPart> customRobotParts = new List<CustomRobotPart>();
}

[Serializable]
public class CustomRobotPart {
    public BodypartType bodypartType;

    public MeshAndMats[] variants;
}

[Serializable]
public class MeshAndMats {
    public Mesh mesh;
    public Material mat;
}

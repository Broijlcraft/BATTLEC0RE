using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BodyPartsManager : MonoBehaviour {
    public static BodyPartsManager single_bpm;
    public Sprite[] robotIcons;
    public RobotMats[] robotMats;
    public List<CustomRobotPart> customRobotParts = new List<CustomRobotPart>();

    public Button switchButton;

    [Space(15)]
    public Material normalMat;
    public Material normalMatEm;
    public Material heavyMat;
    public Material heavyMatEm;

    public int currentSelectedRobot;

    private void Awake() {
        single_bpm = this;
    }

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
        if (switchButton) {
            switchButton.image.sprite = robotIcons[0];
            switchButton.onClick.AddListener(ChangeSelected);
        }
    }

    public void ChangeSelected() {
        currentSelectedRobot++;
        if (currentSelectedRobot >= robotIcons.Length) {
            currentSelectedRobot = 0;
        }
        switchButton.image.sprite = robotIcons[currentSelectedRobot];
    }

}

[Serializable]
public class CustomRobotPart {
    [HideInInspector] public string partName;
    public BodypartType bodypartType;
    public int[] matsPerMesh = new int[] { 2, 2 };
    public List<Mesh> meshes = new List<Mesh>();
}

[Serializable]
public class RobotMats {
    public Material mat, matEm;
}
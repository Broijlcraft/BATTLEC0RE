using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BodyPartsList : MonoBehaviour {

    [HideInInspector] public BodyPartInRig[] parts;
    public string jump, run, walk;
    public int GetPartIndex(BodypartType bodypartType) {
        int i = -1;
        if(parts.Length > 0) {
            for (i = 0; i < parts.Length; i++) {
                if (parts[i].bodypartType == bodypartType) {
                    break;
                }
            }
        }
        return i;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BodyPartsList))]
public class BodyPartsListEditor : Editor {

    BodyPartsList target_BpList;
    string endMsg = "All parts accounted for and stored, don't forget to save!", errStartMsg = "STOPPED SAVING LIST: ", errEndMsg = " make sure you set all bodypart holders";
    BodyPartInRig[] parts;
    List<BodyPartInRig> robotParts = new List<BodyPartInRig>();
    List<string> partNamesInList = new List<string>(), doublePartsNameList = new List<string>();
    bool hasDoubles = false;

    public void OnEnable() {
        target_BpList = (BodyPartsList)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Set bodypart holders")) {
            parts = target_BpList.GetComponentsInChildren<BodyPartInRig>();

            robotParts.Clear();
            partNamesInList.Clear();
            doublePartsNameList.Clear();
            hasDoubles = false;

            if(parts.Length > 0) {
                Debug.Log(hasDoubles);
                if (parts.Length == System.Enum.GetValues(typeof(BodyPart)).Length) {
                    for (int i = 0; i < parts.Length; i++) {
                        BodyPartInRig part = parts[i];
                        string partname = part.bodypartType.ToString();
                        part.index = i;
                        robotParts.Add(part);
                        partNamesInList.Add(partname);
                        EditorUtility.SetDirty(part);
                        if (IsPartAlreadyInList(partNamesInList, partname) && !IsPartAlreadyInList(doublePartsNameList, partname)) {
                            hasDoubles = true;
                            doublePartsNameList.Add(partname);
                        }
                    }

                    if (!hasDoubles) {
                        target_BpList.parts = robotParts.ToArray();
                    } else {
                        string partnames = GetDoubleNames(doublePartsNameList);
                        endMsg = errStartMsg + $"Detected multiple instances of ({partnames})" + errEndMsg + " only once";
                    }

                } else {
                    endMsg = errStartMsg + " You did not set enough bodypart holders," + errEndMsg + " first!";
                }
            } else {
                endMsg = errStartMsg + errEndMsg + " first!";
            }

            Debug.LogWarning($"({parts.Length}) bodyparts found. " + endMsg);
        }
    }

    string GetDoubleNames(List<string> names) {
        string partnames = names[0];
        if (names.Count > 1) {
            for (int i = 1; i < names.Count; i++) {
                partnames += ", " + names[i];
            }
        }
        return partnames;
    }    

    bool IsPartAlreadyInList(List<string> nameList, string partname) {
        bool inList = false;
        if (nameList.Contains(partname)) {
            inList = true;
        }
        return inList;
    }
}
#endif

public enum BodypartType {
    Head,
    UpperChest,
    Chest,
    Pelvis,

    RightShoulder,
    RightUpperArm,
    RightLowerArm,
    PrimaryWeaponHolder,
    RightHip,
    RightUpperLeg,
    RightLowerLeg,
    RightFoot,

    LeftShoulder,
    LeftUpperArm,
    LeftLowerArm,
    PowerWeaponHolder,
    LeftHip,
    LeftUpperLeg,
    LeftLowerLeg,
    LeftFoot
}
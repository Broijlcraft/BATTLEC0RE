using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartsList : MonoBehaviour {
    
    public List<BodyPartInRig> parts = new List<BodyPartInRig>();

    public int GetPartIndex(BodypartType bodypartType) {
        int i = -1;
        if(parts.Count > 0) {
            for (i = 0; i < parts.Count; i++) {
                if (parts[i].bodypartType == bodypartType) {
                    break;
                }
            }
        }
        return i;
    }
}

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
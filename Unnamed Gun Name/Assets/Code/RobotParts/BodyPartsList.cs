using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BodyPartsList : MonoBehaviourPun {
    
    public List<BodyPartInRig> parts = new List<BodyPartInRig>();
    BodyPartsManager bpm;

    public void Init() {
        bpm = BodyPartsManager.single_bpm;
    }

    public void SetMeshes(int index) {
        photonView.RPC(nameof(RPC_SetMeshes), RpcTarget.All, index);
    }

    [PunRPC]
    void RPC_SetMeshes(int index) {
        for (int i = 0; i < parts.Count; i++) {
            BodyPartInRig partInRig = parts[i];
            for (int iB = 0; iB < bpm.customRobotParts.Count; iB++) {
                CustomRobotPart crp = bpm.customRobotParts[iB];

                if (crp.bodypartType == partInRig.bodypartType) {
                    List<Material> mats = new List<Material>();
                    mats.Add(bpm.robotMats[index].mat);
                    if (crp.matsPerMesh[index]>1) {
                        mats.Add(bpm.robotMats[index].matEm);
                    }

                    if (photonView.IsMine) {
                        partInRig.skinnedMeshRenderer.shadowCastingMode = partInRig.castShadows;
                    }

                    partInRig.skinnedMeshRenderer.sharedMesh = crp.meshes[index];
                    partInRig.skinnedMeshRenderer.materials = mats.ToArray();
                    break;
                }
            }
        }
    }

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
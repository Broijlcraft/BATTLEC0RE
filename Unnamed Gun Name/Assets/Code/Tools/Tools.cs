using Photon.Pun;
using UnityEngine;

public static class Tools {

    public static void EnableDisableGameObjectsFromArray(GameObject[] obs, bool newState) {
        for (int i = 0; i < obs.Length; i++) {
            obs[i].SetActive(newState);
        }
    }

    public static void SetLocalOrGlobalLayers(GameObject[] gameObjects, bool global) {
        Debug.Log("Layers");
        int index = TagsAndLayersManager.single_TLM.localPlayerLayerInfo.index;
        SetLayers(gameObjects, index, global);
    }

    static void SetLayers(GameObject[]obs, int index, bool global) {
        if (global) {
            index = 0;
        }

        for (int i = 0; i < obs.Length; i++) {
            GameObject go = obs[i];
            go.layer = index;
        }
    }

    public static int BoolToInt(bool b) {
        int i = 0;
        if (b) {
            i = 1;
        }
        return i;
    }

    public static bool IntToBool(int i) {
        bool b = false;
        if(i == 1) {
            b = true;
        }
        return b;
    }

    public static bool OwnerCheck(PhotonView view) {
        bool hasOwnerAndIsLocal = false;

        if (view && view.IsMine) {
            hasOwnerAndIsLocal = true;
        }

        return hasOwnerAndIsLocal;
    }
}
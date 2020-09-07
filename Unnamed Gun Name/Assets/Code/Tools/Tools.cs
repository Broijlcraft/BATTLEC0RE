using UnityEngine;

public static class Tools {

    public static void EnableDisableGameObjectsFromArray(GameObject[] obs, bool newState) {
        for (int i = 0; i < obs.Length; i++) {
            obs[i].SetActive(newState);
        }
    }

    public static void SetLocalOrGlobalLayers(GameObject[] gameObjects, bool global) {
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
}
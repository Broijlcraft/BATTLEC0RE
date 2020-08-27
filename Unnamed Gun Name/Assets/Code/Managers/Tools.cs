using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools {

    public static void EnableDisableGameObjectsFromArray(GameObject[] obs, bool newState) {
        for (int i = 0; i < obs.Length; i++) {
            obs[i].SetActive(newState);
        }
    }
}
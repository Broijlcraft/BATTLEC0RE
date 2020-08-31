using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssetsList : MonoBehaviour {
    static GameAssetsList gal;
    public static GameAssetsList instance {
        get {
            if (!gal) {
                gal = (Instantiate(Resources.Load("GameAssetsList")) as GameObject).GetComponent<GameAssetsList>();
            }
            return gal;
        }
    }

    public GameObject player, devPrim, devPower;
}
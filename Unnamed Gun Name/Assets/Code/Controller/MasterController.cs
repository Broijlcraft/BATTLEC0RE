using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour {

    public Interactable[] interactablesToInstantiaAndSync;

    public bool ready;

    private void Awake() {
        for (int iB = 0; iB < MultiplayerSetting.single_MPS.maxPlayers; iB++) {
            for (int i = 0; i < interactablesToInstantiaAndSync.Length; i++) {
            }           
        }
    }

    public Dictionary<int, GameObject> itemLists = new Dictionary<int, GameObject>();


}
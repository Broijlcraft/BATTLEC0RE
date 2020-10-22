using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootUpScript : MonoBehaviour {

    public bool isPlaying;
    private void OnDisable() {
        if (isPlaying) {
            CanvasComponents.single_CC.healthBar.Init(Controller.single_CLocal);
            isPlaying = false;
            gameObject.SetActive(false);
        }
    }
}
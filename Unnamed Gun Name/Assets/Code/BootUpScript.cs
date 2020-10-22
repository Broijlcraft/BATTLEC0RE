using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootUpScript : MonoBehaviour {

    public Transform canvas;
    public bool isPlaying, enableOther;

    private void Awake() {
        canvas.transform.SetAsFirstSibling();
        enableOther = false;
    }

    private void OnDisable() {
        if (isPlaying) {
            CanvasComponents.single_CC.healthBar.Init(Controller.single_CLocal);
            isPlaying = false;
            gameObject.SetActive(false);
        }
    }
}
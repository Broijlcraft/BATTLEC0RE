using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootUpScript : MonoBehaviour {

    public bool isPlaying;
    private void OnDisable() {
        if (isPlaying) {
            //transform.localRotation = Quaternion.Euler(firstEndRot);
            //StartCoroutine(SetRot());
            isPlaying = false;
        }
    }
}
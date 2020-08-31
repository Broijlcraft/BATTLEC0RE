using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Manager : MonoBehaviour {
    public static Manager single_M;

    public bool dev;
    public Text devText;

    private void Awake() {
        single_M = this;
        EnableDisableDevText();
    }

    private void Update() {
        if (Input.GetButtonDown("DevMode")) {
            dev = !dev;
            EnableDisableDevText();
        }
    }

    void EnableDisableDevText() {
        if (devText) {
            devText.gameObject.SetActive(dev);
        }
    }
}

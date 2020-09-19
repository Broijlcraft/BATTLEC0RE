using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviour {
    public static Manager single_M;

    public bool dev;
    public Text devText;

    string devCheck = "DevCheck";

    private void Awake() {
        single_M = this;
        EnableDisableDevText();
    }

    private void Update() {
        string nickname = PhotonNetwork.LocalPlayer.NickName;
        nickname = Tools.RemoveIdFromNickname(nickname);
        if (Input.GetButtonDown("DevMode") && Devs.IsDev(nickname)) { 
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {
    public static Manager single_M;

    bool dev;

    private void Awake() {
        if (Manager.single_M) {
            if(Manager.single_M != this) {
                Destroy(gameObject);
            }
        } else {
            DontDestroyOnLoad(gameObject);
            Manager.single_M = this;
        }
    }

    public void SceneFinishedLoading() {
        Init();
        MenuManager.single_MM.Init();
        UiManager.single_UM.Init();
    }

    public void Init() {
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

    public bool IsDev() {
        return dev;
    }

    void EnableDisableDevText() {
        CanvasComponents cc = CanvasComponents.single_CC;
        if (cc.devText) {
            cc.devText.gameObject.SetActive(dev);
        }
    }
}

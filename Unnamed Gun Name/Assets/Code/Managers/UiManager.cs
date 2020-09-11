using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UiManager : MonoBehaviour {
    public static UiManager single_UM;

    public GameObject respawnUiHolder;
    public Animator respawnAnim;
    public Text respawnTimer;

    private void Awake() {
        single_UM = this;
        respawnUiHolder.SetActive(false);
    }
}
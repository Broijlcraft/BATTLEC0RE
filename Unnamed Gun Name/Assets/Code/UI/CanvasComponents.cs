using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CanvasComponents : MonoBehaviour {
    public static CanvasComponents single_CC;

    public Text devText, respawnTimer;
    public Transform killFeedHolder;
    public GameObject respawnUiHolder;
    public Animator respawnAnim;
    public Image ingameHealthBar;
    public HealthBarScript healthBar;

     [Header("Settings")]
    public Button quitReturnButton;
    public Dropdown resolutionsDropdown;
    public GameObject videoSettingsHolder;
    public SettingSlider[] settingSliders;
    public SettingToggle[] settingToggles;

    private void Awake() {
        single_CC = this;
    }
}
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using System;

public class OptionsManager : MonoBehaviour {
    public static OptionsManager single_OM;
    public AudioMixer audioMixer;

    Resolution[] resolutions;

    private void Awake() {
        if (!OptionsManager.single_OM) {
            OptionsManager.single_OM = this;
        }
    }

    private void Start() {
        Init();
    }

    public void Init() {
        if (CanvasComponents.single_CC) {
            CanvasComponents cc = CanvasComponents.single_CC;

            AudioManager.audioMixer = audioMixer;
            for (int i = 0; i < cc.settingSliders.Length; i++) {
                cc.settingSliders[i].Init();
            }

            for (int i = 0; i < cc.settingToggles.Length; i++) {
                cc.settingToggles[i].Init(this);
            }

            SetResolutions();
        }
    }

    public void SetResolutions() {
        if (CanvasComponents.single_CC) {
            CanvasComponents cc = CanvasComponents.single_CC;
            if (cc.resolutionsDropdown) {
                int index = 0;
                resolutions = Screen.resolutions;
                List<string> resolutionStringList = new List<string>();
                List<Resolution> tempResolutions = new List<Resolution>();

                for (int i = resolutions.Length - 1; i > 0; i--) {
                    tempResolutions.Add(resolutions[i]);
                    string option = resolutions[i].width + "x" + resolutions[i].height;
                    resolutionStringList.Add(option);
                    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) {
                        index = resolutions.Length - 1 - i;
                    }
                }

                resolutions = tempResolutions.ToArray();
                cc.resolutionsDropdown.ClearOptions();
                cc.resolutionsDropdown.AddOptions(resolutionStringList);
                cc.resolutionsDropdown.value = index;
                cc.resolutionsDropdown.onValueChanged.AddListener(ChangeResolution);
                cc.resolutionsDropdown.RefreshShownValue();
            }

            for (int i = 0; i < cc.settingToggles.Length; i++) {
                cc.settingToggles[i].Init(this);
            }

            if (cc.videoSettingsHolder) {
                cc.videoSettingsHolder.SetActive(true);
                cc.videoSettingsHolder.SetActive(false);
            }

            if (cc.quitReturnButton) {
                cc.quitReturnButton.onClick.RemoveAllListeners();
                if (MenuManager.single_MM.isMainMenu) {
                    cc.quitReturnButton.onClick.AddListener(QuitGame);
                } else {
                    cc.quitReturnButton.onClick.AddListener(ReturnToMenuFromGame);
                }
            }
        }
    }

    void ChangeResolution(int index) {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
    }

    public void FullScreen(bool value) {
        Screen.fullScreen = value;
    }

    void QuitGame() {
        Application.Quit();
    }

    void ReturnToMenuFromGame() {

    }
}

[Serializable]
public class RangeF {
    public float min = 0.0001f, max = 1f;
}


[Serializable]
public class RangeI {
    public int min = 0, max = 1;
}

public enum SliderType {
    AudioSlider,
    SensitivitySlider,
    Else
}

public enum SensitivitySlider {
    MouseSensitivity
}

[System.Serializable]
public class SettingSlider {
    public Slider slider;
    public RangeF range;

    public SliderType sliderType;
    [Space]
    //Audio Sliders
    public AudioGroups audioGroup;
    [Space]
    //Sensitivity Slider
    public SensitivitySlider sensitivityType;

    public void Init() {
        slider.maxValue = range.max;
        slider.minValue = range.min;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        float value = 0.12f;

        if (sliderType == SliderType.AudioSlider) {
            value = PlayerPrefs.GetFloat(audioGroup.ToString());
        } else if (sliderType == SliderType.SensitivitySlider) {
            value = PlayerPrefs.GetFloat(sensitivityType.ToString());
        }

        slider.value = value;
        OnSliderValueChanged(value);
    }

    public void OnSliderValueChanged(float value) {
        if (sliderType == SliderType.AudioSlider) {
            string group = audioGroup.ToString();
            AudioManager.audioMixer.SetFloat(group, Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat(group, value);
        } else {
            PlayerPrefs.SetFloat(sensitivityType.ToString(), value);
        }
    }
}

public enum ToggleType {
    InvertCam,
    Fullscreen
}

[System.Serializable]
public class SettingToggle {

    OptionsManager optionsManager;
    public ToggleType toggleType;
    public Toggle toggle;

    public void Init(OptionsManager om) {
        optionsManager = om;
        bool isOn = false;
        if (PlayerPrefs.GetInt(toggleType.ToString()) == 1) {
            isOn = true;
        }
        toggle.isOn = isOn;
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnToggleValueChanged(bool value) {
        int intValue;
        if (toggleType == ToggleType.InvertCam) {
            if (value) {
                intValue = -1;
            } else {
                intValue = 1;
            }
            if (Controller.single_CLocal) {
                Controller.single_CLocal.InvertCamMovement();
            }
        } else {
            optionsManager.FullScreen(value);
            intValue = Tools.BoolToInt(value);
        }
        PlayerPrefs.SetInt(toggleType.ToString(), intValue);
    }
}
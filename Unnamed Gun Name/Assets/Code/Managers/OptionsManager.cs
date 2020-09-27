using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using System;

public class OptionsManager : MonoBehaviour {
    public AudioMixer audioMixer;
    public Toggle fullscreenToggle;
    public GameObject dropdownHolder;
    public Dropdown resolutionsDropdown;
    public SettingSlider[] settingSliders;

    Resolution[] resolutions;

    private void Start() {
        AudioManager.audioMixer = audioMixer;
        for (int i = 0; i < settingSliders.Length; i++) {
            settingSliders[i].Init();
        }
        SetResolutions();
    }

    public void SetResolutions() {
        dropdownHolder.SetActive(true);
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
        fullscreenToggle.isOn = Screen.fullScreen;
        resolutionsDropdown.ClearOptions();
        resolutionsDropdown.AddOptions(resolutionStringList);
        resolutionsDropdown.value = index;
        resolutionsDropdown.RefreshShownValue();

        dropdownHolder.SetActive(false);
    }

    public void QuitGame() {
        Application.Quit();
    }
}

[Serializable]
public class Range {
    public float min, max;
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
    public Range range;

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

        //if (sliderType == SliderType.AudioSlider) {
        //    value = PlayerPrefs.GetFloat(audioGroup.ToString());
        //} else if (sliderType == SliderType.SensitivitySlider) {
        //    value = PlayerPrefs.GetFloat(sensitivityType.ToString());
        //}

        slider.value = value;
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
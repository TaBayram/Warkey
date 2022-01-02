using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    GameObject toggleObj;
    public Toggle fullscreenToggle;

    [SerializeField] private Slider FPS_Slider;
    [SerializeField] private TextMeshProUGUI FPS_Value;

    void Start()
    {
        //Check the device's available resolutions and add them to the dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolution = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }

        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();

        //Check if current screen is full and modify the default value for toggle
        toggleObj = GameObject.Find("FullscreenToggle");
        fullscreenToggle = toggleObj.GetComponent<Toggle>();

        if (Screen.fullScreen)
        {
            fullscreenToggle.isOn = true;
        }
        else
        {
            fullscreenToggle.isOn = false;
        }

        FPS_Value.text = "" + Application.targetFrameRate;
        FPS_Slider.value = Application.targetFrameRate;

    }
    public void Awake()
    {
        FPS_Slider.onValueChanged.AddListener((v) =>
        {
            FPS_Value.text = v.ToString();
            Application.targetFrameRate = (int)FPS_Slider.value;
            PlayerPrefs.SetInt("FrameRate", Application.targetFrameRate);
        });
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }


}

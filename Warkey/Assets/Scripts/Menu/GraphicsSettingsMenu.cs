using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphicsSettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    void Start()
    {
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

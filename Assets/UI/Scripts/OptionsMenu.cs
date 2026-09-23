using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer mixer;

    Resolution[] resolutions;

    public TMP_Dropdown dropdown;

    void Start()
    {
        resolutions = Screen.resolutions;
        dropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
        
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        dropdown.AddOptions(options);
        dropdown.value = currentResolutionIndex;
        dropdown.RefreshShownValue();
    }

    public void SetMVolume (float volume)
    { 
        mixer.SetFloat("Master", volume);
    }

    public void SetMusic(float volume)
    {
        mixer.SetFloat("Music", volume);
    }

    public void SetSounds(float volume)
    {
        mixer.SetFloat("Sounds", volume);
    }


    public void setQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

    }
}


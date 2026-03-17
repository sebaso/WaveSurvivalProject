using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider ambienceSlider;

    [Header("Graphics")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;

    private void Start()
    {
        InitializeResolutions();
        LoadSettings();
    }

    private void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio.value.ToString("F0") + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void LoadSettings()
    {
        // Load Audio
        float master = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        float ambience = PlayerPrefs.GetFloat("AmbienceVolume", 0.75f);

        if (masterSlider) masterSlider.value = master;
        if (musicSlider) musicSlider.value = music;
        if (sfxSlider) sfxSlider.value = sfx;
        if (ambienceSlider) ambienceSlider.value = ambience;

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);
        SetAmbienceVolume(ambience);

        // Graphics are handled in InitializeResolutions for the dropdown, 
        // but we'll ensure they are applied here too if needed.
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    private void SetMixerVolume(string parameter, float sliderValue)
    {
        // Convert 0-1 slider value to decibels (-80 to 20)
        // Logarithmic scale is better for volume
        float dB = sliderValue > 0.0001f ? Mathf.Log10(sliderValue) * 20 : -80f;
        audioMixer.SetFloat(parameter, dB);
    }

    public void SetMasterVolume(float volume)
    {
        SetMixerVolume("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetMixerVolume("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetMixerVolume("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetAmbienceVolume(float volume)
    {
        SetMixerVolume("AmbientVolume", volume);
        PlayerPrefs.SetFloat("AmbientVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    private void OnDisable()
    {
        SaveSettings();
    }
}

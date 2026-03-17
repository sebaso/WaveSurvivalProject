using System.Collections;
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
    public AudioSource menuMusicSource;
    public float musicFadeDuration = 2.0f;
    public GameObject settingsMenu;

    [Header("Graphics")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;

    private void Start()
    {
        ValidateAssignments();
        InitializeResolutions();
        HookUpListeners();
        LoadSettings();
        StartCoroutine(FadeInMusic());
    }

    private void ValidateAssignments()
    {
        if (audioMixer == null) Debug.LogError("SettingsMenu: AudioMixer is not assigned!");
        if (masterSlider == null) Debug.LogWarning("SettingsMenu: MasterSlider is not assigned.");
        if (musicSlider == null) Debug.LogWarning("SettingsMenu: MusicSlider is not assigned.");
        if (sfxSlider == null) Debug.LogWarning("SettingsMenu: SFXSlider is not assigned.");
        if (ambienceSlider == null) Debug.LogWarning("SettingsMenu: AmbienceSlider is not assigned.");
        if (resolutionDropdown == null) Debug.LogError("SettingsMenu: ResolutionDropdown is not assigned!");
        if (fullscreenToggle == null) Debug.LogError("SettingsMenu: FullscreenToggle is not assigned!");
    }

    private void HookUpListeners()
    {
        if (masterSlider) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        if (ambienceSlider) ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);

        if (fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        if (resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private IEnumerator FadeInMusic()
    {
        if (menuMusicSource == null) yield break;

        float targetVolume = menuMusicSource.volume;
        menuMusicSource.volume = 0f;
        menuMusicSource.Play();

        float currentTime = 0;
        while (currentTime < musicFadeDuration)
        {
            currentTime += Time.deltaTime;
            menuMusicSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / musicFadeDuration);
            yield return null;
        }
        menuMusicSource.volume = targetVolume;
    }

    private void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new();
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
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    private void SetMixerVolume(string parameter, float sliderValue)
    {
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
    public void BackButton()
    {
        settingsMenu.SetActive(false);

    }
    public void ToggleMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }


    private void OnDisable()
    {
        SaveSettings();
    }
}

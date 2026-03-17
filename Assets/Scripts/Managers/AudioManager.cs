using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    public AudioMixer audioMixer;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void SetMasterVolume(float volume)
    {
        if (audioMixer)
        {
            float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat("MasterVolume", dB);
        }
    }
    public void SetSFXVolume(float volume)
    {
        if (audioMixer)
        {
            float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat("SFXVolume", dB);
        }
    }
    public void SetMusicVolume(float volume)
    {
        if (audioMixer)
        {
            float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat("MusicVolume", dB);
        }
    }
}

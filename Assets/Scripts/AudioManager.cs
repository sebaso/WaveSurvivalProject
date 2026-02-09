using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private AudioMixer _audioMixer;

    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume",Mathf.Log10(volume) *20);
    }
    public void SetEffectsVolume(float volume)
    {
        _audioMixer.SetFloat("EffectsVolume",Mathf.Log10(volume) *20);
    }
        public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume",Mathf.Log10(volume) *20);
    }
}
